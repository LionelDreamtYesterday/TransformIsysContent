using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;
using System.Web;
using System.Text.RegularExpressions;
using TransformIsysContent.Compression;

namespace TransformIsysContent.XML_Node_Replacement
{
    public class XmlOperations
    {
        private static string xhtml_namespace = "xmlns=\"http://www.w3.org/1999/xhtml\"";
        private static string div_empty = "<div " + xhtml_namespace + " />";
        private static string str_error_message = "<b><u> <span style=\"color: red;\">"
                 + "CAREFUL ! THERE WAS AN ERROR WHEN THIS ITEM WAS IMPORTED INTO INTEGRITY !"
        +"<br/>" + "Please ask your administrator to check the Isys importer logs in order to find the original RTF Content."
        +"<br/>" + "The path where a copy has been made is at the following location :"
        +"<br/>" +"</span></u></b>";

        /// <summary>
        /// Create the third argument from the second one.
        /// Create the error directory if this one does not exist.
        /// </summary>
        /// <param name="inputFilePath">"inputFilePath.isys"</param>
        /// <param name="outputFilePath">"outputFilePath.xml"</param>
        public static void NodeContentReplacement(string inputFilePath, string outputFilePath)
        {
            string errorDirectoryPath = Path.GetDirectoryName(outputFilePath) + Path.DirectorySeparatorChar + "errors" ;
            bool exists = System.IO.Directory.Exists(errorDirectoryPath);
            if (!exists)
            {
                System.IO.Directory.CreateDirectory(errorDirectoryPath);
            }
            NodeContentReplacement(inputFilePath, outputFilePath, errorDirectoryPath);
        }

        /// <summary>
        /// Find the content node of the isys file.
        /// Take the node and put it in a string in order to avoir XML content problems.
        ///     1) Find the content for each node http://www.csharp-examples.net/xml-nodes-by-attribute-value/
        ///     2) Decode it
        ///     3) Ungzip it
        ///     4) Convert it from RTF to HTML
        ///     5) Replace it in the XML (with a good namespace and without error)
        /// </summary>
        /// <param name="inputFilePath">"inputFilePath.isys"</param>
        /// <param name="outputFilePath">"outputFilePath.xml"</param>
        /// <param name="errorDirectoryPath">"C:\errorDirectoryPath"</param>
        public static void NodeContentReplacement(string inputFilePath, string outputFilePath, string errorDirectoryPath)
        {
            int nodeNumber = 0; int nb_ok = 0; int nb_erreurs = 0;
            int nb_images = 0; int nb_images_imported = 0;
            string directoryName = Path.GetDirectoryName(outputFilePath);
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(inputFilePath);

            // 1) Extract all the tags "Content" from the Isys XML
            XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("Content");
            Console.WriteLine("Number of nodes to treat=" + elementsByTagName.Count);
            foreach (XmlNode xmlNode in elementsByTagName) // Find each node "Content"
            {
                Console.Write(++nodeNumber + ".");
                try
                {
                    // 2) We Decode from Base64 the content of the node and then put it in a gzip file
                    byte[] gz_bytes = Convert.FromBase64String(xmlNode.InnerText);
                    File.WriteAllBytes(outputFilePath + ".gzip", gz_bytes);
                    // 3) We translate the gzip file to rtf and then from rtf to html
                    byte[] decompressed = Utilitaries.DecompressGzipFile(File.ReadAllBytes(outputFilePath + ".gzip"));
                    File.WriteAllBytes(outputFilePath + ".rtf", decompressed);

                    // 4) Convert it from RTF to HTML thanks to the Office Interop library
                    // We simply save the file and ask word to convert it.
                    Rtf2Html.ConvertRtfToHTML(outputFilePath + ".rtf");

                    // 5) We take the div element from the html file and put it back in the node
                    HtmlDocument htmlDocument = new HtmlDocument();
                    htmlDocument.OptionAutoCloseOnEnd = true;
                    htmlDocument.OptionFixNestedTags = true;
                    htmlDocument.OptionOutputAsXml = true;
                    htmlDocument.OptionWriteEmptyNodes = true;
                    htmlDocument.Load(outputFilePath + ".html");

                    HtmlNode htmlNodeBody = htmlDocument.DocumentNode
                            .Element("html").Element("body"); // .Element("div") 

                    HtmlNode htmlNodeSpan = htmlNodeBody.SelectSingleNode("//div/p/b/span");
                    
                    if (htmlNodeSpan != null)
                    {
                        HtmlNode parentNode = htmlNodeSpan.ParentNode.ParentNode;
                        parentNode.ParentNode.RemoveChild(parentNode, false);
                    }

                    // Conversion of all the pictures
                    HtmlNodeCollection htmlNodeCollection = htmlNodeBody.SelectNodes("//img[@src and @height and @width]");
                    if (htmlNodeCollection != null)
                    {
                        foreach (HtmlNode htmlNodeImg in (IEnumerable<HtmlNode>) htmlNodeCollection)
                        { 
                            nb_images++;
                            string attributeValue = htmlNodeImg.GetAttributeValue("src", null);
                            string sourceFileName = directoryName + Path.DirectorySeparatorChar + Path.GetDirectoryName(attributeValue) + Path.DirectorySeparatorChar + Path.GetFileName(attributeValue);
                            string imageFileName = "Image-" + nodeNumber + "_" + nb_images + Path.GetExtension(attributeValue);
                            string imagePathName = Path.GetDirectoryName(attributeValue) + Path.DirectorySeparatorChar + imageFileName;
                            string destFileName = directoryName + (object)Path.DirectorySeparatorChar + imagePathName;
                            htmlNodeImg.SetAttributeValue("src", imagePathName);
                            try
                            {
                                File.Copy(sourceFileName, destFileName, true);
                                nb_images_imported++;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("The move of the image number" + nb_images + " of the node number " + nodeNumber + " has failed.");
                                Console.WriteLine(ex);
                            }
                        }
                    }

                    string str_divHTML = htmlNodeBody.InnerHtml;
                    // In XML we cannot use any '<' or '>' signs.
                    str_divHTML = str_divHTML.Replace("&gt;", "\"supérieur à\"").Replace("&lt;", "\"inférieur à\"").Replace("&gt;=", "\"supérieur ou égal à\"").Replace("&lt;=", "\"inférieur ou égal à\"");

                    // We transform the namespaces from the Isys ones to the HTML ones
                    str_divHTML = str_divHTML.Replace("<div class=\"WordSection1\">", "<div " + xhtml_namespace + " >");
                    str_divHTML = str_divHTML.Replace("<div class=\"Section1\">", "<div " + xhtml_namespace + " >");
                    str_divHTML = str_divHTML.Replace("name=\" ", "name=\""); // This line is a bug repairing because a name cannot begin by a space
                    str_divHTML = HttpUtility.HtmlDecode(str_divHTML);
                    str_divHTML = HttpUtility.HtmlDecode(str_divHTML); //TODO : Superflu ?

                    xmlNode.InnerXml = str_divHTML;
                    nb_ok++;
                }
                catch (Exception exception)
                {
                    nb_erreurs++;
                    String errorPath = errorDirectoryPath + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(outputFilePath) + "-" + nodeNumber + ".rtf";
                    xmlNode.InnerXml = XmlOperations.ElementNotConvertedError(nodeNumber, exception, outputFilePath + ".rtf", errorPath);
                }
            }
            Console.WriteLine("");
            Console.WriteLine("Number of imported elements : " + nb_ok + "/" + elementsByTagName.Count);
            Console.WriteLine("Number of non-imported elements : " + nb_erreurs + "/" + elementsByTagName.Count);

            Console.WriteLine("Number of images that will be imported: " + nb_images_imported + " (on a total number of " + nb_images + " images).");
            xmlDocument.Save(outputFilePath);
        }

        /// <summary>
        /// A function whose work is to log what is the error about and keep a trace of what has gone wrong.
        /// If we cannot translate the node then we replace it by one that explicitely states that there is an error.
        /// TODO : Add the original content in a temporary folder.
        /// </summary>
        /// <param name="nodeNumber">The number of the element that count not be imported</param>
        /// <param name="exception">The stack of the exception for error analysis</param>
        /// <param name="filePath">The filepath of the element that could not be converted</param>
        /// <returns></returns>
        private static string ElementNotConvertedError(int nodeNumber, Exception exception, String filePath, String errorPath)
        {
            Console.WriteLine("");
            Console.WriteLine("The element number " + nodeNumber + " has not been imported because there was an error.");
            Console.WriteLine("The error was: " + exception);
            try
            {
                // Console.WriteLine("filePath=" + filePath);
                // Console.WriteLine("errorPath=" + errorPath);
                File.Copy(filePath, errorPath, true);
                Console.WriteLine("A copy of the RTF element node number " + nodeNumber + " was made at this path :" + errorPath);
            }
            catch(Exception ex)
            {
                Console.WriteLine("ERROR: There was a try to save a copy of the RTF element Node but there was an error : " + ex);
            }

            // errorPath

            return divErrorMessage(errorPath);
        }

        private static string divErrorMessage(String errorPath)
        {
            String div_error = "<div " + xhtml_namespace + ">" + str_error_message + errorPath + "</div>";

            return div_error;
        }

    }// end of the XmlOperations class
}

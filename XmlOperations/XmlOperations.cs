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

namespace TransformIsysContent.XML_Node_Replacement
{
    public class XmlOperations
    {

        /// <summary>
        /// Find the content node of the isys file.
        /// Take the node and put it in a string in order to avoir XML content problems.
        ///     1) Find the content for each node http://www.csharp-examples.net/xml-nodes-by-attribute-value/
        ///     2) Decode it
        ///     3) Ungzip it
        ///     4) Convert it from RTF to HTML
        ///     5) Replace it in the XML (with a good namespace and without error)
        /// </summary>
        /// <param name="filePath">"file.xml"</param>
        public static void NodeContentReplacement(string inputFilePath, string outputFilePath)
        {
            int i = 0; int nb_ok = 0; int nb_erreurs = 0;
            string xhtml_namespace = "xmlns=\"http://www.w3.org/1999/xhtml\"";
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(inputFilePath);

            // 1) Extract all the tags "Content" of the Isys XML
            XmlNodeList xmlNodes = xmlDocument.GetElementsByTagName("Content");
            Console.WriteLine("Number of nodes to treat=" + xmlNodes.Count);
            foreach (XmlNode xn in xmlNodes) // Find each node "Content"
            {
                ++i;
                Console.Write(i + ".");
                try
                {
                    // 2) We Decode from Base64 the content of the node and then put it in a gzip file
                    byte[] gz_bytes = System.Convert.FromBase64String(xn.InnerText);
                    System.IO.File.WriteAllBytes(outputFilePath + ".gzip", gz_bytes);
                    byte[] file = File.ReadAllBytes(outputFilePath + ".gzip");

                    // 3) We translate the gzip file to rtf and then from rtf to html
                    byte[] decompressed = Compression.Utilitaries.DecompressGzipFile(file);
                    System.IO.File.WriteAllBytes(outputFilePath + ".rtf", decompressed);

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

                    HtmlNode divElement = htmlDocument.DocumentNode
                            .Element("html").Element("body"); // .Element("div") 

                    HtmlNode spanNode = divElement.SelectSingleNode("//div/p/b/span");
                    HtmlNode pNode = spanNode.ParentNode.ParentNode;
                    pNode.ParentNode.RemoveChild(pNode, false);

                    string divHTML = divElement.InnerHtml;
                    // In XML we cannot use any '<' or '>' signs.
                    divHTML = divHTML.Replace("&gt;", "\"supérieur à\"").Replace("&lt;", "\"inférieur à\"").Replace("&gt;=", "\"supérieur ou égal à\"").Replace("&lt;=", "\"inférieur ou égal à\"");
                    divHTML = HttpUtility.HtmlDecode(divHTML);

                    // We noticy that this tag is now an html tag and not an isys one
                    divHTML = divHTML.Replace("<div class=\"WordSection1\">", "<div " + xhtml_namespace + " >");
                    divHTML = divHTML.Replace("<div class=\"Section1\">", "<div " + xhtml_namespace + " >");
                    divHTML = divHTML.Replace("name=\" ", "name=\""); // This line is a bug repairing because a name cannot begin by a space
                    divHTML = HttpUtility.HtmlDecode(divHTML);

                    xn.InnerXml = divHTML;
                    nb_ok++;
                }
                catch (Exception exception)
                {
                    // If we cannot translate the node then we replace it by an empty one
                    nb_erreurs++;
                    Console.WriteLine("The element number " + i + "has not been imported because there was an error.");
                    Console.WriteLine("The error was: " + exception);
                    xn.InnerXml = "<div " + xhtml_namespace + " />";
                }
            }
            Console.WriteLine("");
            Console.WriteLine("Number of imported elements : " + nb_ok + "/" + xmlNodes.Count);
            Console.WriteLine("Number of non-imported elements : " + nb_erreurs + "/" + xmlNodes.Count);
            xmlDocument.Save(outputFilePath);
        }
    }

}

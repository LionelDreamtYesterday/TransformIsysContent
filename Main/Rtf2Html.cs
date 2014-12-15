using eShopping.Common.Converter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransformIsysContent
{
    public class Rtf2Html
    {

        /// <summary>
        /// Create an .html file that is the transformed rtf file.
        /// </summary>
        /// <param name="documentPath">Complete path of the rtf file that will be converted</param>
        public static void ConvertRtfToHTML(string documentPath)
        {
            try
            {
                string generatedPath = Path.ChangeExtension(documentPath, "html");
                ConvertRtfToHTML(documentPath, generatedPath);
            }
            catch (Exception exception)
            {
                Console.WriteLine("A problem has occured : " + exception.ToString());
                Console.WriteLine("A problem has occured : The program will terminate now");
            }
        }

        /// <summary>
        /// Create an .html file that is the transformed rtf file.
        /// </summary>
        /// <param name="documentPath">Complete path of the rtf file that will be converted.</param>
        /// <param name="generatedPath">Complete path of the html file that will be created.</param>
        public static void ConvertRtfToHTML(string documentPath, string generatedPath)
        {
            try
            {
                IConverter doc = ConverterLocator.Converter(documentPath, generatedPath);
                doc.Convert();
            }
            catch (Exception exception)
            {
                Console.WriteLine("A problem has occured : " + exception.ToString());
                Console.WriteLine("A problem has occured : The program will terminate without converting the file now");
            }
        }

    }
}

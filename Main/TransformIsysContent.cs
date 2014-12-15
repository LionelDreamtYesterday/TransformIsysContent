using TransformIsysContent.XML_Node_Replacement;
using eShopping.Common.Converter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace TransformIsysContent
{
    /*
     * Test of git 2
     */ 
    public class Program
    { 
        /**
         * Entry of the program. Test of git.
         */
        public static void Main(string[] args)
        {
            DateTime startTime = DateTime.Now;
            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Console.WriteLine("TransformIsysContent v " + version + " --- Convert the content of an isys file to html.");
            printVersion();
            Console.WriteLine("You have entered " + args.Length + " arguments.");
            if (args.Length == 3)
            {
                if (args[0] == "rtf2html")
                {
                    Rtf2Html.ConvertRtfToHTML(args[1]);
                }
                else if (args[0] == "content2html")
                {
                    XmlOperations.NodeContentReplacement(args[1], args[2]);
                }
            }
            else if (args.Length == 2)
            {
                XmlOperations.NodeContentReplacement(args[0], getRootPath(args[1]));
            }
            else
            {
                Console.WriteLine("The command takes one or two file paths as arguments.");
                Console.WriteLine("Usage :  TransformIsysContent file.isys [file.xml]");
            }
            DateTime endTime = DateTime.Now;
            TimeSpan totalTimeTaken = endTime.Subtract(startTime);
        }

        private static void printVersion()
        {
            string bitVersion = "";
            if (IntPtr.Size == 4) // 32-bit
            {
                bitVersion = "32";
            }
            else if (IntPtr.Size == 8) // 64-bit
            {
                bitVersion = "64";
            }
            Console.WriteLine("Running as a " + bitVersion + " bits application");
            Console.WriteLine("Running with the DotNet framework v" + Environment.Version);
        }

        private static string getRootPath(string path)
        {
            if (System.IO.Path.IsPathRooted(path))
                return path;
            else
                return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\" + path;
        }

        private static String GetTimestamp()
        {
            DateTime now = DateTime.Now;
            return now.ToString("yyyy_MM_dd_HH_mm_ss_ffff");
        }

    }
}

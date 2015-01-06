// Decompiled with JetBrains decompiler
// Type: TransformIsysContent.Program
// Assembly: TransformIsysContent, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B7B55B2-4BEE-4296-A03C-F1D93238EF33
// Assembly location: D:\Users\lreveillere\Desktop\Shared_Folder\Projects\DCNS\Import_Isys\DCNS-Isys-Bin-2014_06_18\bin\TransformIsysContent.exe

using System;
using System.IO;
using System.Reflection;
using TransformIsysContent.XML_Node_Replacement;

namespace TransformIsysContent
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DateTime now = DateTime.Now;
            Console.WriteLine("TransformIsysContent v " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " --- Convert the content of an isys file to html.");
            Program.printVersion();
            Console.WriteLine("You have entered " + (object)args.Length + " arguments.");
            if (args.Length == 3)
            {
                /*
                if (args[0] == "rtf2html")
                    Rtf2Html.ConvertRtfToHTML(args[1]);
                else if (args[0] == "content2html")
                    XmlOperations.NodeContentReplacement(args[1], args[2]);
                 */
                XmlOperations.NodeContentReplacement(args[0], Program.getRootPath(args[1]), Program.getRootPath(args[2]));
            }
            else if (args.Length == 2)
            {
                XmlOperations.NodeContentReplacement(args[0], Program.getRootPath(args[1]));
            }
            // We can use this tool for converting only one RTF file to HTML
            // (Useful for debugging)
            else if (args.Length == 1)
            {
                Rtf2Html.ConvertRtfToHTML(args[1]);
            }
            else
            {
                Console.WriteLine("The command takes one, two or three file paths as arguments.");
                Console.WriteLine("Usage :  TransformIsysContent file.isys [file.xml] [error_folder]");
            }
            DateTime.Now.Subtract(now);
        }

        private static void printVersion()
        {
            string str = "";
            if (IntPtr.Size == 4)
                str = "32";
            else if (IntPtr.Size == 8)
                str = "64";
            Console.WriteLine("Running as a " + str + " bits application");
            Console.WriteLine("Running with the DotNet framework v" + (object)Environment.Version);
        }

        private static string getRootPath(string path)
        {
            if (Path.IsPathRooted(path))
                return path;
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + path;
        }

        private static string GetTimestamp()
        {
            return DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_ffff");
        }
    }
}

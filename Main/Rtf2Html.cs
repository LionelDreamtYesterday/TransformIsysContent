// Decompiled with JetBrains decompiler
// Type: TransformIsysContent.Rtf2Html
// Assembly: TransformIsysContent, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B7B55B2-4BEE-4296-A03C-F1D93238EF33
// Assembly location: D:\Users\lreveillere\Desktop\Shared_Folder\Projects\DCNS\Import_Isys\DCNS-Isys-Bin-2014_06_18\bin\TransformIsysContent.exe

using eShopping.Common.Converter;
using System;
using System.IO;

namespace TransformIsysContent
{
    public class Rtf2Html
    {
        public static void ConvertRtfToHTML(string documentPath)
        {
            try
            {
                string generatedPath = Path.ChangeExtension(documentPath, "html");
                Rtf2Html.ConvertRtfToHTML(documentPath, generatedPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("A problem has occured : " + ex.ToString());
                Console.WriteLine("A problem has occured : The program will terminate now");
            }
        }

        public static void ConvertRtfToHTML(string documentPath, string generatedPath)
        {
            try
            {
                ConverterLocator.Converter(documentPath, generatedPath).Convert();
            }
            catch (Exception ex)
            {
                Console.WriteLine("A problem has occured : " + ex.ToString());
                Console.WriteLine("A problem has occured : The program will terminate without converting the file now");
            }
        }
    }
}

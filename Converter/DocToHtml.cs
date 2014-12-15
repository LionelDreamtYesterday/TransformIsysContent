// -----------------------------------------------------------------------
// <copyright file="DocToHtml.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace eShopping.Common.Converter
{
    using System;
    using System.IO;
    using System.Text;
    using Application = Microsoft.Office.Interop.Word.Application;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DocToHtml : FileConverter, IConverter
    {
        public StringBuilder Convert()
        {
            Application objWord = new Application();
            try
            {
                objWord.Documents.Open(FileName: FullFilePath);
                objWord.Visible = false;
                if (objWord.Documents.Count > 0)
                {
                    Microsoft.Office.Interop.Word.Document oDoc = objWord.ActiveDocument;
                    oDoc.SaveAs(FileName: FileToSave, FileFormat: 10);

                    ((Microsoft.Office.Interop.Word._Document)oDoc).Close(SaveChanges: false);
                    return base.ReadConvertedFile();
                }
            }
            catch (DirectoryNotFoundException directoryNotFoundException)
            {
                Console.WriteLine("The file has not been found at the path specified : " + directoryNotFoundException.Message);
                Console.WriteLine("The program will terminate without converting the file");
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                Console.WriteLine("The file has not been found at the path specified : " + fileNotFoundException.FileName);
                Console.WriteLine("The program will terminate without converting the file");
            }
            catch (IOException ioexception)
            {
                Console.WriteLine(ioexception.Message);
                Console.WriteLine("The program will terminate without converting the file");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine("The program will terminate without converting the file");
            }
            finally
            {
                ((Microsoft.Office.Interop.Word._Application)objWord.Application).Quit(SaveChanges: false);
            }
            return null;
        }

        /*
         * The original code was made for a webserver 
         * The files produced were only displayed and not stored on it
         * We need to keep them now, so this portion of code is commented.
        ~DocToHtml()
        {
            base.DeleteFiles();
        }
         */
    }
}

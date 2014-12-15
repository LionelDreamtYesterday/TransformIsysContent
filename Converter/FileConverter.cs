using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace eShopping.Common.Converter
{
    public class FileConverter
    {
        public string FullFilePath { get; set; }
        public string FileToSave { get; set; }
        public string Url;
        StringBuilder ConvertedResult = new StringBuilder();

        public void DeleteFiles()
        {
            if (File.Exists(FileToSave))
                File.Delete(FileToSave);
            if (File.Exists(FullFilePath))
                File.Delete(FullFilePath);
        }
        public StringBuilder ReadConvertedFile()
        {
            int Count = 0;
        ReadFileAgain:
            try
            {
                string ext = Path.GetExtension(FullFilePath);
                if (ext == ".xls" || ext == ".xlsx")
                {
                    return ReadXlsxFile();
                }
                else
                {
                    using (StreamReader reader = new StreamReader(FileToSave))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            ConvertedResult.Append(line);
                        }
                        reader.Dispose();
                    }
                }

            }
            catch (Exception ex)
            {
                System.Threading.Thread.Sleep(100);
                if (++Count == 3)
                {
                    throw ex;
                }
                goto ReadFileAgain;
            }
            return ConvertedResult;
        }

        private StringBuilder ReadXlsxFile()
        {
            string fileName = Path.GetFileName(FileToSave);
            string directory = Path.GetDirectoryName(FileToSave) + "\\" + fileName.Split('.')[0] + "_files";
            string[] files = Directory.GetFiles(directory);
            for (int i = 0; i < files.Length - 1; i++)
            {
                if (Path.GetExtension(files[i]) == ".html")
                {
                    ConvertedResult.Append(File.ReadAllText(files[i]).Replace("<![endif]>", ""));
                }
            }
            return ConvertedResult;
        }
    }
}

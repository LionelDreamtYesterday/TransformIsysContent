// Decompiled with JetBrains decompiler
// Type: eShopping.Common.Converter.FileConverter
// Assembly: TransformIsysContent, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B7B55B2-4BEE-4296-A03C-F1D93238EF33
// Assembly location: D:\Users\lreveillere\Desktop\Shared_Folder\Projects\DCNS\Import_Isys\DCNS-Isys-Bin-2014_06_18\bin\TransformIsysContent.exe

using System;
using System.IO;
using System.Text;
using System.Threading;

namespace eShopping.Common.Converter
{
    public class FileConverter
    {
        private StringBuilder ConvertedResult = new StringBuilder();
        public string Url;
        public string FullFilePath { get; set; }
        public string FileToSave { get; set; }

        public void DeleteFiles()
        {
            if (File.Exists(this.FileToSave))
                File.Delete(this.FileToSave);
            if (!File.Exists(this.FullFilePath))
                return;
            File.Delete(this.FullFilePath);
        }

        public StringBuilder ReadConvertedFile()
        {
            int num = 0;
        ReadFileAgain:
            try
            {
                string extension = Path.GetExtension(this.FullFilePath);
                if (extension == ".xls" || extension == ".xlsx")
                    return this.ReadXlsxFile();
                using (StreamReader streamReader = new StreamReader(this.FileToSave))
                {
                    string str;
                    while ((str = streamReader.ReadLine()) != null)
                        this.ConvertedResult.Append(str);
                    streamReader.Dispose();
                }
            }
            catch (Exception ex)
            {
                Thread.Sleep(100);
                if (++num == 3)
                    throw ex;
                goto ReadFileAgain;
            }
            return this.ConvertedResult;
        }

        private StringBuilder ReadXlsxFile()
        {
            string[] files = Directory.GetFiles(Path.GetDirectoryName(this.FileToSave) + "\\" + Path.GetFileName(this.FileToSave).Split('.')[0] + "_files");
            for (int index = 0; index < files.Length - 1; ++index)
            {
                if (Path.GetExtension(files[index]) == ".html")
                    this.ConvertedResult.Append(File.ReadAllText(files[index]).Replace("<![endif]>", ""));
            }
            return this.ConvertedResult;
        }
    }
}

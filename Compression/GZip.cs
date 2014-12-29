// Decompiled with JetBrains decompiler
// Type: TransformIsysContent.Gzip.GZip
// Assembly: TransformIsysContent, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B7B55B2-4BEE-4296-A03C-F1D93238EF33
// Assembly location: D:\Users\lreveillere\Desktop\Shared_Folder\Projects\DCNS\Import_Isys\DCNS-Isys-Bin-2014_06_18\bin\TransformIsysContent.exe

using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace TransformIsysContent.Gzip
{
  internal class GZip
  {
    private static void CompressFile(string sDir, string sRelativePath, GZipStream zipStream)
    {
      char[] chArray = sRelativePath.ToCharArray();
      zipStream.Write(BitConverter.GetBytes(chArray.Length), 0, 4);
      foreach (char ch in chArray)
        zipStream.Write(BitConverter.GetBytes(ch), 0, 2);
      byte[] buffer = File.ReadAllBytes(Path.Combine(sDir, sRelativePath));
      zipStream.Write(BitConverter.GetBytes(buffer.Length), 0, 4);
      zipStream.Write(buffer, 0, buffer.Length);
    }

    private static bool DecompressFile(string sDir, GZipStream zipStream, GZip.ProgressDelegate progress)
    {
      byte[] buffer1 = new byte[4];
      if (zipStream.Read(buffer1, 0, 4) < 4)
        return false;
      int num = BitConverter.ToInt32(buffer1, 0);
      byte[] buffer2 = new byte[2];
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < num; ++index)
      {
        zipStream.Read(buffer2, 0, 2);
        char ch = BitConverter.ToChar(buffer2, 0);
        stringBuilder.Append(ch);
      }
      string str = stringBuilder.ToString();
      if (progress != null)
        progress(str);
      byte[] buffer3 = new byte[4];
      zipStream.Read(buffer3, 0, 4);
      int count = BitConverter.ToInt32(buffer3, 0);
      byte[] buffer4 = new byte[count];
      zipStream.Read(buffer4, 0, buffer4.Length);
      string path = Path.Combine(sDir, str);
      string directoryName = Path.GetDirectoryName(path);
      if (!Directory.Exists(directoryName))
        Directory.CreateDirectory(directoryName);
      using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
        fileStream.Write(buffer4, 0, count);
      return true;
    }

    private static void CompressDirectory(string sInDir, string sOutFile, GZip.ProgressDelegate progress)
    {
      string[] files = Directory.GetFiles(sInDir, "*.*", SearchOption.AllDirectories);
      int startIndex = (int) sInDir[sInDir.Length - 1] == (int) Path.DirectorySeparatorChar ? sInDir.Length : sInDir.Length + 1;
      using (FileStream fileStream = new FileStream(sOutFile, FileMode.Create, FileAccess.Write, FileShare.None))
      {
        using (GZipStream zipStream = new GZipStream((Stream) fileStream, CompressionMode.Compress))
        {
          foreach (string str1 in files)
          {
            string str2 = str1.Substring(startIndex);
            if (progress != null)
              progress(str2);
            GZip.CompressFile(sInDir, str2, zipStream);
          }
        }
      }
    }
    
    /// <summary>
    /// NON IMPLEMENTED METHOD
    /// </summary>
    /// <param name="sCompressedFile"></param>
    /// <param name="sDir"></param>
    /// <param name="progress"></param>
    private static void DecompressToDirectory(string sCompressedFile, string sDir, GZip.ProgressDelegate progress)
    {
      using (FileStream fileStream = new FileStream(sCompressedFile, FileMode.Open, FileAccess.Read, FileShare.None))
      {
        using (GZipStream zipStream = new GZipStream((Stream) fileStream, CompressionMode.Decompress, true))
        {
            throw new NotImplementedException("This method has not been implemented yet.");
            // while (GZip.DecompressFile(sDir, zipStream, progress));
            // do
            // ;
        }
      }
    }

    public static void directoryCompress(string arg0, string arg1)
    {
      try
      {
        string str1;
        string str2;
        bool flag;
        if (Directory.Exists(arg0))
        {
          str1 = arg0;
          str2 = arg1;
          flag = true;
        }
        else if (File.Exists(arg0))
        {
          str2 = arg0;
          str1 = arg1;
          flag = false;
        }
        else
        {
          Console.Error.WriteLine("Wrong arguments");
          return;
        }
        if (flag)
          GZip.CompressDirectory(str1, str2, (GZip.ProgressDelegate) (fileName => Console.WriteLine("Compressing {0}...", (object) fileName)));
        else
          GZip.DecompressToDirectory(str2, str1, (GZip.ProgressDelegate) (fileName => Console.WriteLine("Decompressing {0}...", (object) fileName)));
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine(ex.Message);
      }
    }

    private delegate void ProgressDelegate(string sMessage);
  }
}

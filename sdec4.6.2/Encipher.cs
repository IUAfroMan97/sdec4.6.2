// Decompiled with JetBrains decompiler
// Type: SAMDdec.Encipher
// Assembly: sdec2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89A9C1B1-FD6D-42DE-9B68-F5653B302358
// Assembly location: C:\Users\jgood\Desktop\sdec2.exe

using System;
using System.IO;
using System.Security.Cryptography;

namespace SAMDdec
{
  internal class Encipher
  {
    public static string sn = Environment.NewLine;
    public static int chunkSize = 1048576;
    public static int headerSize = 3072;

    public static byte[] GetBytesFromFile(string fullFilePath, long from, out int readCount)
    {
      using (FileStream fileStream = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
      {
        byte[] buffer = new byte[Encipher.chunkSize];
        fileStream.Seek(from, SeekOrigin.Begin);
        readCount = fileStream.Read(buffer, 0, buffer.Length);
        return buffer;
      }
    }

    public static byte[] GetHeaderBytesFromFile(string fullFilePath)
    {
      using (FileStream fileStream = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
      {
        byte[] buffer = new byte[Encipher.headerSize];
        fileStream.Seek(0L, SeekOrigin.Begin);
        fileStream.Read(buffer, 0, buffer.Length);
        return buffer;
      }
    }

    public static bool WriteBytesToFile(string _FileName, byte[] _ByteArray)
    {
      FileStream fileStream = new FileStream(_FileName, FileMode.Append, FileAccess.Write);
      try
      {
        fileStream.Write(_ByteArray, 0, _ByteArray.Length);
        fileStream.Close();
        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine("Exception caught in process: {0}", (object) ex.ToString());
        fileStream.Close();
        if (File.Exists(_FileName))
          File.Delete(_FileName);
      }
      return false;
    }

    public static void DecryptFile(string encryptedFilePath, string decryptedFilePath, byte[] key, byte[] iv, long lOrgFileSize)
    {
      if (File.Exists(decryptedFilePath))
        File.Delete(decryptedFilePath);
      long length = new FileInfo(encryptedFilePath).Length;
      long num1 = (long) (int) (length / (long) Encipher.chunkSize + 1L);
      bool flag = true;
      try
      {
        for (long index = 0; index < num1; ++index)
        {
          int readCount;
          byte[] bytesFromFile = Encipher.GetBytesFromFile(encryptedFilePath, (long) Encipher.headerSize + index * (long) Encipher.chunkSize, out readCount);
          if (readCount > 0)
          {
            byte[] cipherText = new byte[readCount];
            Buffer.BlockCopy((Array) bytesFromFile, 0, (Array) cipherText, 0, readCount);
            byte[] _ByteArray = Encipher.DecryptStringFromBytes(cipherText, key, iv, readCount);
            flag = Encipher.WriteBytesToFile(decryptedFilePath, _ByteArray);
            if (!flag)
              break;
          }
        }
        if (!flag)
          return;
        long num2 = length - (long) Encipher.headerSize - lOrgFileSize;
        FileInfo fileInfo = new FileInfo(decryptedFilePath);
        FileStream fileStream = fileInfo.Open(FileMode.Open);
        fileStream.SetLength(Math.Max(0L, fileInfo.Length - num2));
        fileStream.Close();
      }
      catch (Exception ex)
      {
      }
    }

    public static byte[] DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV, int size)
    {
      byte[] buffer = new byte[size];
      if (cipherText == null || cipherText.Length == 0)
        throw new ArgumentNullException(nameof (cipherText));
      if (Key == null || Key.Length == 0)
        throw new ArgumentNullException("AAA");
      if (IV == null || IV.Length == 0)
        throw new ArgumentNullException("AA");
      using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
      {
        rijndaelManaged.KeySize = 128;
        rijndaelManaged.FeedbackSize = 8;
        rijndaelManaged.Key = Key;
        rijndaelManaged.IV = IV;
        rijndaelManaged.Padding = PaddingMode.Zeros;
        ICryptoTransform decryptor = rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV);
        using (MemoryStream memoryStream = new MemoryStream(cipherText))
        {
          using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, decryptor, CryptoStreamMode.Read))
            cryptoStream.Read(buffer, 0, size);
        }
        return buffer;
      }
    }

    public static byte[] RSADescryptBytes(byte[] datas, string keyXml)
    {
      byte[] numArray = (byte[]) null;
      using (RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider(2048))
      {
        cryptoServiceProvider.FromXmlString(keyXml);
        try
        {
          numArray = cryptoServiceProvider.Decrypt(datas, true);
        }
        catch (Exception ex)
        {
        }
      }
      return numArray;
    }

    public static string GetStringFromBytes(byte[] bytes)
    {
      char[] chArray = new char[bytes.Length / 2];
      Buffer.BlockCopy((Array) bytes, 0, (Array) chArray, 0, bytes.Length);
      return new string(chArray);
    }
  }
}

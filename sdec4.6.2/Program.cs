// Decompiled with JetBrains decompiler
// Type: SAMDdec.Program
// Assembly: sdec2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89A9C1B1-FD6D-42DE-9B68-F5653B302358
// Assembly location: C:\Users\jgood\Desktop\sdec2.exe

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Threading;
using System.Xml;

namespace SAMDdec {
    internal class Program {
        private static string[] ext_enc = new string[1] {
            ".weapologize"
        };
        //private static string helpfile = "SORRY-FOR-FILES";
        //private static string helpfileext = ".html";
        //private static string wi_ndo_ws_d_r_iv_e_ = Path.GetPathRoot(Environment.SystemDirectory);
        //private static List<string> mylist = new List<string>();
        private static string selfname = Process.GetCurrentProcess ().ProcessName + ".exe";
        private static string privkey = "";
        //private static List<string> bad_dec = new List<string>();

        private static void Main (string[] args) {
            if (args.Length < 1) {
                Console.WriteLine ("\r\n[+] Usage:\r\n\t" + Program.selfname + " private.keyxml\r\n");
            } else {
                if (args.Length == 1) {
                    Thread.Sleep (3000);
                    try {
                        Program.privkey = File.ReadAllText (args[0]);
                        foreach (DriveInfo drive in DriveInfo.GetDrives ()) {
                            try {
                                if (drive.IsReady)
                                    Program.recursivegetfiles (drive.Name);
                            } catch { }
                        }
                        Console.WriteLine ("\r\n[+] All File Decrypted.");
                        Thread.Sleep (3000);
                    } catch { }
                }
                if (args.Length != 3)
                    return;
                if (args[0] == "-f") {
                    Program.privkey = File.ReadAllText (args[1]);
                    Program.recursivegetfiles (args[2]);

                    Console.WriteLine ("\r\n[+] All File Decrypted.");
                    Thread.Sleep (3000);
                }
                if (!(args[0] == "-k"))
                    return;
                foreach (string file in Directory.GetFiles (args[2])) {
                    Program.privkey = File.ReadAllText (file);
                    try {
                        string s1 = "";
                        string s2 = "";
                        string stringFromBytes = Encipher.GetStringFromBytes (Encipher.GetHeaderBytesFromFile (args[1]));
                        XmlDocument xmlDocument = new XmlDocument ();
                        xmlDocument.LoadXml (stringFromBytes);
                        foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName ("AAA"))
                            s1 = xmlNode.InnerText;
                        foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName ("AA"))
                            s2 = xmlNode.InnerText;
                        foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName ("AAAAAAAAAAAAAAAAAA"))
                            Convert.ToInt64 (xmlNode.InnerText);
                        byte[] numArray = Encipher.RSADescryptBytes (Convert.FromBase64String (s1), Program.privkey);
                        Encipher.RSADescryptBytes (Convert.FromBase64String (s2), Program.privkey);
                        if (numArray.Length != 0)
                            Console.WriteLine ("\r\nCORRECT KEY IS:" + file);
                    } catch { }
                }
            }
        }

        private static string MakePath (string plainFilePath, string newSuffix) {
            string path2 = Path.GetFileNameWithoutExtension (plainFilePath) + newSuffix;
            return Path.Combine (Path.GetDirectoryName (plainFilePath), path2);
        }

        public static void decryptFile (string encryptedFilePath) {
            string str = Program.MakePath (encryptedFilePath, "");
            try {
                string s1 = "";
                string s2 = "";
                long lOrgFileSize = 0;
                string stringFromBytes = Encipher.GetStringFromBytes (Encipher.GetHeaderBytesFromFile (encryptedFilePath));
                XmlDocument xmlDocument = new XmlDocument ();
                xmlDocument.LoadXml (stringFromBytes);
                foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName ("AAA"))
                    s1 = xmlNode.InnerText;
                foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName ("AA"))
                    s2 = xmlNode.InnerText;
                foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName ("AAAAAAAAAAAAAAAAAA"))
                    lOrgFileSize = Convert.ToInt64 (xmlNode.InnerText);
                byte[] key = Encipher.RSADescryptBytes (Convert.FromBase64String (s1), Program.privkey);
                byte[] iv = Encipher.RSADescryptBytes (Convert.FromBase64String (s2), Program.privkey);
                Encipher.DecryptFile (encryptedFilePath, str, key, iv, lOrgFileSize);
            } catch (FormatException ex) {
                Console.WriteLine ("\r\n[-] Decryption key is not correct -> " + encryptedFilePath + ex.Message);
                if (!File.Exists (str))
                    return;
                File.Delete (str);
            } catch (XmlException ex) {
                Console.WriteLine ("\r\n[-] Encrypted data is not correct -> " + encryptedFilePath + ex.Message);
                if (!File.Exists (str))
                    return;
                File.Delete (str);
            }
        }

        private static bool hasWriteAccessToFolder (string path) {
            try {
                System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl (path);
                return true;
            } catch (UnauthorizedAccessException) {

                return false;
            }
        }

        public static Boolean checkHasBeenDecrypted (FileInfo f, ArrayList fs) {

            if (f.Extension == ".weapologize") {
                FileInfo fMinusExt = new FileInfo (f.FullName.Substring (0, f.FullName.Length - Program.ext_enc[0].Length));
                foreach (FileInfo file in fs) {
                    if (file.Name.ToString () == fMinusExt.Name.ToString ()) {
                        if (f.Length - fMinusExt.Length == 3072) {
                            Console.WriteLine (f.FullName + " has previously been decrypted...");
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static void recursivegetfiles (string path) {
            while (true) {

                if (Program.hasWriteAccessToFolder (path)) {
                    DirectoryInfo directoryInfo = new DirectoryInfo (path);

                    try {
                        FileInfo[] files = directoryInfo.GetFiles ();

                        ArrayList fs = new ArrayList ();
                        foreach (FileInfo file in files) {
                            if (file.FullName.Length >= 260) {
                                string log = @"c:\logs\pathtoolong.txt";
                                File.AppendAllText (log, "File : " + file.FullName + Environment.NewLine);
                                continue;
                            }
                            try {
                                fs.Add (file); // add each file in a folder into the List
                                if (!Program.checkHasBeenDecrypted (file, fs)) {
                                    string ext = Path.GetExtension (file.FullName); //get file extension

                                    // if file extension == .weapologize
                                    if (ext == Program.ext_enc[0]) {
                                        Console.WriteLine ("Decrypting : " + file.FullName);
                                        decryptFile (file.FullName);
                                    }
                                }
                                continue;
                            } catch (PathTooLongException ex) {
                                Console.WriteLine (ex);
                            }
                        }
                        // empty the ArrayList
                        fs.Clear ();
                        foreach (DirectoryInfo directory in directoryInfo.GetDirectories ()) {
                            if (directory.FullName.Length >= 260) {
                                File.AppendAllText (@"c:\logs\pathtoolong.txt", "Directory : " + directory.FullName + Environment.NewLine);
                                continue;
                            }
                            try {
                                Program.recursivegetfiles (directory.FullName);
                            } catch (PathTooLongException ex) {
                                Console.WriteLine (ex);
                            }
                        }
                        break;
                    } catch (UnauthorizedAccessException) { }
                } else {
                    Console.WriteLine ("You do not have access to the following path: " + path);
                    return;
                }
            }
        }

        //public static bool isValidFilePath(string strFilePath)
        //{
        //  bool flag = false;
        //  try
        //  {
        //    if (File.Exists(strFilePath))
        //      flag = true;
        //  }
        //  catch (Exception ex)
        //  {
        //    Console.WriteLine(ex);
        //  }
        //  return flag;
        //}
        //public static void dec(List<string> ll)
        //{
        //  Console.WriteLine("\r\n[+] " + (object) ll.Count + " Files Found.");
        //  Thread.Sleep(3000);
        //  for (int currElementIndex = 0; currElementIndex < ll.Count; ++currElementIndex)
        //  {
        //    try
        //    {
        //      FileInfo fileInfo = new FileInfo(ll[currElementIndex]);
        //      Program.ShowPercentProgress("[+] Path: " + fileInfo.FullName + " Length: " + (object) fileInfo.Length + " \r\nProgress: ", currElementIndex, ll.Count);
        //      Program.myddeecc(ll[currElementIndex]);
        //      if (File.Exists(ll[currElementIndex]))
        //      {
        //        if (File.Exists(fileInfo.Directory.ToString() + "\\" + Program.helpfile + Program.helpfileext))
        //          File.Delete(fileInfo.Directory.ToString() + "\\" + Program.helpfile + Program.helpfileext);
        //        for (int index = 0; index < 10; ++index)
        //        {
        //          if (File.Exists(fileInfo.Directory.ToString() + "\\00" + index.ToString() + "-" + Program.helpfile + Program.helpfileext))
        //            File.Delete(fileInfo.Directory.ToString() + "\\00" + index.ToString() + "-" + Program.helpfile + Program.helpfileext);
        //        }
        //      }
        //    }
        //    catch
        //    {
        //    }
        //  }
        //}

        //public static void dec2(List<string> ll)
        //{
        //  Console.WriteLine("\r\n[+] " + (object) ll.Count + " Files Found.");
        //  Thread.Sleep(3000);
        //  for (int currElementIndex = 0; currElementIndex < ll.Count; ++currElementIndex)
        //  {
        //    try
        //    {
        //      FileInfo fileInfo = new FileInfo(ll[currElementIndex]);
        //      Program.ShowPercentProgress("[+] Path: " + fileInfo.FullName + " Length: " + (object) fileInfo.Length + " \r\nProgress: ", currElementIndex, ll.Count);
        //      Program.myddeecc2(ll[currElementIndex]);
        //      if (File.Exists(ll[currElementIndex]))
        //      {
        //        if (File.Exists(fileInfo.Directory.ToString() + "\\" + Program.helpfile + Program.helpfileext))
        //          File.Delete(fileInfo.Directory.ToString() + "\\" + Program.helpfile + Program.helpfileext);
        //        for (int index = 0; index < 10; ++index)
        //        {
        //          if (File.Exists(fileInfo.Directory.ToString() + "\\00" + index.ToString() + "-" + Program.helpfile + Program.helpfileext))
        //            File.Delete(fileInfo.Directory.ToString() + "\\00" + index.ToString() + "-" + Program.helpfile + Program.helpfileext);
        //        }
        //      }
        //    }
        //    catch
        //    {
        //    }
        //  }
        //}

        //public static void myddeecc(string pathfile)
        //{
        //  if (!Program.isValidFilePath(pathfile) || new FileInfo(pathfile).Length <= 0L)
        //    return;
        //  Program.decryptFile(pathfile);
        //  if (File.Exists(pathfile.Replace(Program.ext_enc[0], "")))
        //    return;
        //  Program.bad_dec.Add(pathfile);
        //}

        //public static void myddeecc2(string pathfile)
        //{
        //  if (!Program.isValidFilePath(pathfile) || new FileInfo(pathfile).Length <= 0L)
        //    return;
        //  Program.decryptFile(pathfile);
        //}

        //  public static void delete_desktop_helps()
        //  {
        //    foreach (string directory in Directory.GetDirectories(Directory.GetParent(Environment.GetEnvironmentVariable("userprofile")).FullName))
        //    {
        //      if (Directory.Exists(directory + "\\Desktop"))
        //      {
        //        try
        //        {
        //          foreach (FileInfo file in new DirectoryInfo(directory + "\\Desktop").GetFiles())
        //          {
        //            if (file.Name.Contains(Program.helpfile))
        //              File.Delete(file.FullName);
        //          }
        //        }
        //        catch (Exception ex)
        //        {
        //          Console.WriteLine(ex);
        //        }
        //      }
        //    }
        //  }

        //  private static void ShowPercentProgress(string message, int currElementIndex, int totalElementCount)
        //  {
        //    if (currElementIndex < 0 || currElementIndex >= totalElementCount)
        //      throw new InvalidOperationException("currElement out of range");
        //    ++currElementIndex;
        //    double num = (double) (currElementIndex * 100) / Convert.ToDouble(totalElementCount);
        //    Console.Write("\r{0}{1} %", (object) message, (object) num);
        //    if (currElementIndex != totalElementCount - 1)
        //      return;
        //    Console.WriteLine(Environment.NewLine);
        //  }
    }
}
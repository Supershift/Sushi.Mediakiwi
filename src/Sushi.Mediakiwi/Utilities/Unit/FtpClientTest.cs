//#if DEBUG
//using System;
//using System.Net;
//using System.IO;
//using System.Collections.Generic;
//using System.Text;
//using NUnit.Framework;

//namespace Wim.Utilities.Unit
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    [TestFixture]
//    public class FtpClientTest
//    {
//        FtpClient Client;

//        /// <summary>
//        /// 
//        /// </summary>
//        [TestFixtureSetUp()]
//        public void CreateConnection()
//        {
//            Console.Out.Flush(); ;
//            Client = new FtpClient();
//            Client.setDebug(true);
//            Client.setRemoteHost("ftp.supershift.net");
//            Client.setRemoteUser("supershift.ftp");
//            Client.setRemotePass("superftp4usall");
//            Client.login();
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        [Test(Description = "Create a local file, upload it, download it and remove it locally and remote.")]
//        public void PushFile()
//        {
//            Client.setBinaryMode(true);

//            string filename = "FtpClientTest.txt";
//            string filepath = string.Concat(@"c:\", filename);

//            System.IO.TextWriter writer = new StreamWriter(filepath);
//            writer.WriteLine("This is a test");
//            writer.Close();

//            Console.Out.WriteLine(string.Format("\n\nCreated a local file: '{0}'", filepath));

//            Assert.IsTrue(System.IO.File.Exists(filepath), "Local file could not be created, this test can not be completed.");

//            Console.Out.WriteLine("Upload the local file to the ftp server\n");
//            Client.upload(filepath);

//            Console.Out.WriteLine("\n\nRemoving the local file\n");
//            System.IO.File.Delete(filepath);

//            Assert.IsFalse(System.IO.File.Exists(filepath), "Local file has not been removed.");

//            Console.Out.WriteLine("Verifying the upload (list all files on server with extention txt)\n");
//            string foundfile = null;
//            foreach (string item in Client.getFileList("*.txt"))
//            {
//                if (item.Trim().Length > 0)
//                {
//                    Console.Out.WriteLine(string.Concat("\nFile: ", item, "\r"));
//                    if (item.Trim().Equals(filename, StringComparison.OrdinalIgnoreCase)) foundfile = item.Trim();
//                }
//            }

//            Assert.IsTrue(foundfile != null, "The file was not found on the FTP server");

//            Console.Out.WriteLine("\nDownload the remote file\n");
//            Client.download(foundfile, filepath);

//            Assert.IsTrue(System.IO.File.Exists(filepath), "Local file has not downloaded.");

//            Console.Out.WriteLine("\n\nRemove the remote file\n");
//            Client.deleteRemoteFile(foundfile);
//            Client.close();

//            Console.Out.WriteLine("\nRemoving the downloaded file\n");
//            System.IO.File.Delete(filepath);
//        }
//    }
//}
//#endif
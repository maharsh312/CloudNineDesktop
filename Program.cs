using System;
using System.IO;
using System.Collections;
using System.Net;

namespace CloudNineDesktop
{
    class Program
    {
        public static void Main(string[] args)
        {
            string sourceDirectory = @"C:\Users\mahar\Desktop\test";
            string targetDirectory = @"C:\Users\mahar\Desktop\copy\test";
            Copy(sourceDirectory, targetDirectory);
            Copy(targetDirectory, sourceDirectory);
            Console.WriteLine();
            Console.WriteLine("Task Completed. Press any key to continue");
            Console.ReadLine();

        }

        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {

            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {

                DateTime created = fi.CreationTime;
                DateTime lastmodified = fi.LastWriteTime;

                using (WebClient client = new WebClient())
                {
                    client.UploadFile("http://192.168.1.120:8080/cloudnine/FileUpload", fi.ToString());
                }

                if (File.Exists(Path.Combine(target.FullName, fi.Name)))
                {
                    string tFileName = Path.Combine(target.FullName, fi.Name);
                    FileInfo f2 = new FileInfo(tFileName);
                    DateTime lm = f2.LastWriteTime;
                    Console.WriteLine(@"File {0}\{1} Already Exist {2} last modified {3}", target.FullName, fi.Name, tFileName, lm);

                    try
                    {
                        if (lastmodified > lm)
                        {
                            Console.WriteLine(@"Source file {0}\{1} last modified {2} is newer than the target file {3}\{4} last modified {5}",
                            fi.DirectoryName, fi.Name, lastmodified.ToString(), target.FullName, fi.Name, lm.ToString());
                            fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                        }
                        else
                        {
                            Console.WriteLine(@"Destination File {0}\{1} Skipped", target.FullName, fi.Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
                else
                {
                    Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                    fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                }

            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

    }
}
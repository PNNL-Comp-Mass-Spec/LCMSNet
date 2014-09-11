using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FindModified
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] filesShare = Directory.GetFiles(@"\\proto-5\bionetxfer\Source\LCMSNet-3-02-2012\LCMSNet\", "*.cs", SearchOption.AllDirectories);
            string[] filesLocal = Directory.GetFiles(@"C:\development\Proteomics\lcms\trunk\LCMSNet\", "*.cs", SearchOption.AllDirectories);


            using (TextWriter writer = File.CreateText(@"changed.txt"))
            {
                List<string> share = new List<string>();
                foreach (string file in filesShare)
                {
                    FileInfo info = new FileInfo(file);
                    if (DateTime.Now.Subtract(info.LastWriteTime).TotalDays < 7)
                    {
                        Console.WriteLine("{0}", file);
                        writer.WriteLine("{0}", file);

                        string newPath = file.Replace(@"\\proto-5\bionetxfer\Source\LCMSNet-3-02-2012", @"C:\development\Proteomics\lcms\trunk");

                        Console.WriteLine("{0} to {1}", file, newPath);
                        FileInfo localInfo = new FileInfo(newPath);
                        if (DateTime.Now.Subtract(localInfo.LastWriteTime).TotalDays < 7)
                        {
                            Console.WriteLine("\t\tChanged...");

                            string serverPath = Path.Combine("diff", Path.GetFileName(file));
                            string xpath      = newPath.Replace(".cs", "-local.cs");
                            string localPath  = Path.Combine("diff", Path.GetFileName(xpath));
                            
                            
                            File.Copy(file, serverPath);
                            File.Copy(newPath, localPath);
                            

                            string [] dataServer = File.ReadAllLines(serverPath);
                            string [] dataLocal  = File.ReadAllLines(localPath);

                            bool equals = true;
                            if (dataServer.Length != dataLocal.Length)
                            {
                                equals = false;
                            }
                            else
                            {
                                for(int i = 0; i < dataServer.Length; i++)
                                {
                                    if (dataServer[i].CompareTo(dataLocal[i]) != 0)
                                    {
                                        equals = false;
                                        break;
                                    }
                                }
                            }
                            if (equals)
                            {
                                File.Delete(localPath);
                                File.Delete(serverPath);
                            }
                            else
                            {
                                Console.WriteLine("Not equals");
                            }
                        }
                    }
                }
            }

            Console.ReadLine();
        }
    }

    class Hammer
    {
        public string Share;
        public string Local;
    }
}

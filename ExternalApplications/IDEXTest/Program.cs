using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace IDEXTest
{
    class Program
    {
        static void UnWrapException(Exception ex)
        {
            if (ex == null)
                return;

            Console.WriteLine(ex.Message);
            UnWrapException(ex.InnerException);
        }
        static void Main(string[] args)
        {
            ASUTGen.Devices.ExternalMessageBuilder builder = new ASUTGen.Devices.ExternalMessageBuilder();

            string message = "";
            string address = "";

            try
            {
                System.Diagnostics.Debug.Assert(args.Length > 0);
                    
                using (SerialPort port = new SerialPort(args[0]))
                {
                    // Milliseconds
                    port.WriteTimeout = 500;
                    port.Open();

                    // User interaction loop
                    while (message.ToLower() != "quit" && address.ToLower() != "quit")
                    {
                        //Console.WriteLine("Enter Address: ");
                        //address = Console.ReadLine();
                        Console.WriteLine("Enter Message");
                        Console.ReadLine();

                        //message = Console.ReadLine();
                        message = "W6E0350013F";

                        string externalMessage = builder.BuildWriteMessage(address, message);
                        Console.WriteLine("Built message: {0}", externalMessage);
                        port.Write(externalMessage);

                        System.Threading.Thread.Sleep(10);
                        string returnedMessage = port.ReadExisting();
                        Console.WriteLine("Read status: {0}", returnedMessage);

                        // Do a readback                        
                    }
                }
            }
            catch (Exception ex)
            {
                UnWrapException(ex);
                Console.ReadLine();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Agilent.Licop;

namespace LicopDemo
{
    /// <summary>
    /// This program demonstrates the use of Agilent Licop Library.
    /// After entering the address of an LC device it connects,
    /// displays device events and allows to send instructions.
    /// Proper error handling is omitted to keep the example short.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Instrument i = new Instrument(10000, 10000); // increase timeouts when debugging !!!
            i.ConfigurationChanged += new EventHandler<ConfigurationEventArgs>(ConfigurationChanged);
            i.ErrorOccurred += new EventHandler<ErrorEventArgs>(ErrorOccurred);

            Console.Write("Enter instrument address: ");
            string address = Console.ReadLine();

            // connect to instrument
            if (i.TryConnect(address, 5000) == false)
            {
                Console.WriteLine("Could not connect.");
                return;
            }            

            // get the module that has the LAN/RS232 access
            Module m = i.CreateModule(i.GetAccessPointIdentifier());

            // open EV channel
            Channel evChannel = m.CreateChannel("EV");
            evChannel.DataReceived += new EventHandler<DataEventArgs>(DataReceived);
            if (evChannel.TryOpen(ReadMode.Events) == false)
            {
                Console.WriteLine("Could not open EV channel.");
                return;
            }

            // open IN channel
            Channel inChannel = m.CreateChannel("IN");
            if (inChannel.TryOpen(ReadMode.Polling) == false)
            {
                Console.WriteLine("Could not open IN channel.");
                return;
            }

            Console.WriteLine("Enter instruction to send or press enter to disconnect.");
            string instruction = "";
            while (true)
            {
                instruction = Console.ReadLine();
                if (instruction != "")
                {
                    // send instruction
                    if (inChannel.TryWrite(instruction, 10000) == false)
                    {
                        Console.WriteLine("Could not send instruction.");
                        break;
                    }

                    // read reply
                    string reply = "";
                    if (inChannel.TryRead(out reply, 10000) == false)
                    {
                        Console.WriteLine("Could not read reply.");
                        break;
                    }
                    
                    Console.WriteLine("Reply: " + reply);
                }
                else
                {
                    break;
                }
            }

            // close EV channel
            evChannel.DataReceived -= DataReceived;
            evChannel.Close();

            // close IN channel
            inChannel.Close();

            // disconnect
            i.ErrorOccurred -= ErrorOccurred;
            i.ConfigurationChanged -= ConfigurationChanged;
            i.Disconnect();
        }

        static void ConfigurationChanged(object sender, ConfigurationEventArgs e)
        {
            // the instrument configuration has changed

            foreach (string id in e.RemovedModuleIdentifiers)
                Console.WriteLine("Removed modules: " + id);

            foreach (string id in e.NewModuleIdentifiers)
                Console.WriteLine("New module: " + id);
        }

        static void ErrorOccurred(object sender, ErrorEventArgs e)
        {
            // an error occurred, connection is closed
            Console.WriteLine("Error occurred: " + e.Message);
        }

        static void DataReceived(object sender, DataEventArgs e)
        {
            // data received from the EV channel
            Console.WriteLine("Data from EV channel: " + e.AsciiData);
        }
    }
}

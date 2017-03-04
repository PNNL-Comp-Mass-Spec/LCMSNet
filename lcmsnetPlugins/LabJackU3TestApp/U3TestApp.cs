/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 *
 *********************************************************************************************************/
using System;
using LcmsNet.Devices.ContactClosure;

namespace LabJackU3TestApp
{
    class U3TestApp
    {
        static void Main(string[] args)
        {
            var myU3 = new classLabjackU3();
            myU3.Initialize();

            PrintMenu();
            while (true)
            {
                try
                {
                    Console.Write("What is your command? ");
                    var input = Console.ReadLine();
                    if (input == null)
                        break;

                    var tokens = input.Split();

                    var cmdStr = tokens[0];
                    var channelNum = -1;
                    if (tokens.Length > 1)
                    {
                        channelNum = Convert.ToInt32(tokens[1]);
                    }
                    double readValue;
                    if (string.IsNullOrWhiteSpace(cmdStr))
                    {
                        break;
                    }

                    if (cmdStr == "DI")
                    {

                        Console.WriteLine("Digital read channel: " + channelNum);
                        readValue = myU3.Read((enumLabjackU3InputPorts)channelNum);
                        Console.WriteLine("Value read: " + readValue.ToString("0.00"));
                    }
                    else if (cmdStr == "DO")
                    {
                        Console.WriteLine("Digital write (1) to channel: " + channelNum);
                        myU3.Write((enumLabjackU3OutputPorts)channelNum, 1);
                        Console.WriteLine("Written.");
                    }
                    else if (cmdStr == "DO2")
                    {
                        Console.WriteLine("Digital write (0) to channel: " + channelNum);
                        myU3.Write((enumLabjackU3OutputPorts)channelNum, 0);
                        Console.WriteLine("Written.");
                    }
                    else if (cmdStr == "AI")
                    {
                        Console.WriteLine("Analog Read from channel: " + channelNum);
                        readValue = myU3.Read((enumLabjackU3InputPorts)channelNum);
                        Console.WriteLine("Value read: " + readValue.ToString("0.00"));
                    }
                    else if (cmdStr == "AO")
                    {
                        Console.WriteLine("Analog write (5v) to channel: " + channelNum);
                        myU3.Write((enumLabjackU3OutputPorts)channelNum, 5);
                        Console.WriteLine("Written.");
                    }
                    else if (cmdStr == "AO2")
                    {
                        Console.WriteLine("Analog write (0v) to channel: " + channelNum);
                        myU3.Write((enumLabjackU3OutputPorts)channelNum, 0);
                        Console.WriteLine("Written.");
                    }
                    else if (cmdStr == "help")
                    {
                        PrintMenu();
                    }
                    else if (cmdStr == "channels")
                    {
                        PrintChannels();
                    }
                    else
                    {
                        Console.WriteLine("Invalid command: " + cmdStr);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
          
        }

        private static void PrintMenu()
        {
            Console.WriteLine("Commands:");
            Console.WriteLine("DI <channel number>: Read digital input from channel");
            Console.WriteLine("DO <channel number>: Write digital value (1) to channel");
            Console.WriteLine("DO2 <channel number>: Write digital value (0) to channel");
            Console.WriteLine("AI <channel number>: Read analog input from channel");
            Console.WriteLine("AO <channel number>: Write 5volts to channel");
            Console.WriteLine("AO2 <channel number>: Write 0volts to channel");
            Console.WriteLine("help: view this command list");
            Console.WriteLine("channels: list channels");
        }

        private static void PrintChannels()
        {
            Console.WriteLine("Digital input:");
            Console.WriteLine("\tFIO0-FIO7 = 0-7");
            Console.WriteLine("\tEIO0-EIO7 = 8-15");
            Console.WriteLine("\tCIO0-CIO3 = 16-19");
            Console.WriteLine("Analog input :");
            Console.WriteLine("\tFIO0-FIO7 = 20-27");
            Console.WriteLine("\tEIO0-EIO7 = 28-35");
            Console.WriteLine("Digital Output:");
            Console.WriteLine("\tFIO0-FIO7 = 0-7");
            Console.WriteLine("\tEIO0-EIO7 = 8-15");
            Console.WriteLine("\tCIO0-CIO3 = 16-19");
            Console.WriteLine("Analog Output:");
            Console.WriteLine("\tDAC0-DAC1 = 20-21");

        }
    }
}

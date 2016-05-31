/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute * 
 * 
 * Last Modified 6/5/2014 By Christopher Walters 
 *********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newport.ESP300;

namespace NewportTestApp
{
    class newportTestApp
    {
        static void Main(string[] args)
        {
            classNewportStage stage = new classNewportStage();
            stage.OpenPort();
            PrintMenu();
            string input;
            while (true)
            {
                Console.Write("What is your command?");
                input = Console.ReadLine();
                string[] tokens = input.Split();
                if (tokens[0] == "fh")
                {
                    for (int axis = 1; axis < stage.NumAxes + 1; axis++)
                    {
                        stage.FindHome(axis);
                    }
                }
                else if (tokens[0] == "re")
                {
                    int err = -1;
                    long timestamp = -1;
                    string description = string.Empty;
                    stage.ReadErrorMessage(ref err, ref timestamp, ref description);
                    Console.WriteLine(description);
                }
                else if(tokens[0] == "h")
                {
                    PrintMenu();
                }
                else if(tokens[0] == "mv")
                {
                    int axis = Convert.ToInt32(tokens[1]);
                    float position = Convert.ToSingle(tokens[2]);
                    stage.MoveToPosition(axis, position);
                }
                else if (tokens[0] == "md")
                {
                    int axis = Convert.ToInt32(tokens[1]);
                    bool done = stage.MotionDone(axis);
                    Console.WriteLine(done.ToString());
                }
                else if(tokens[0] == "mo")
                {
                    int axis = Convert.ToInt32(tokens[1]);
                    stage.MotorOn(axis);
                    Console.WriteLine(axis.ToString() + "Motor ON!");
                }
                else if(tokens[0] == "mf")
                {
                    int axis = Convert.ToInt32(tokens[1]);
                    stage.MotorOff(axis);
                    Console.WriteLine(axis.ToString() + "Motor OFF!");
                }
                else if (tokens[0] == "sm")
                {
                    int axis = Convert.ToInt32(tokens[1]);
                    stage.StopMotion(axis);
                    Console.WriteLine("Stopped Axis " + axis.ToString());
                }
                else if (tokens[0] == "mva")
                {
                    int axis = Convert.ToInt32(tokens[1]);
                    bool reverse = Convert.ToBoolean(tokens[2]);
                    bool slow = Convert.ToBoolean(tokens[3]);
                    stage.MoveAxis(axis, reverse, slow);
                }
                else if (tokens[0] == "qp")
                {
                    int axis = Convert.ToInt32(tokens[1]);
                    Console.WriteLine("Axis " + axis.ToString() + " is at position: " + stage.QueryPosition(axis).ToString());
                }
                else if (tokens[0] == "sp")
                {
                    int position = Convert.ToInt32(tokens[1]);
                    float axis1 = Convert.ToSingle(stage.QueryPosition(1));
                    float axis2 = Convert.ToSingle(stage.QueryPosition(2));
                    float axis3 = 0;
                    stage.SetPositionCoordinates(tokens[1], axis1, axis2, axis3);
                }
                else if (tokens[0] == "gp")
                {
                    int position = Convert.ToInt32(tokens[1]);
                    stage.GoToPosition(0.0d, tokens[1]);
                }
                else if (tokens[0] == string.Empty)
                {
                    break;
                }
               
            }
        }

        private static void PrintMenu()
        {
            Console.WriteLine("fh--Find Home for all axes");
            Console.WriteLine("re--read errors");
            Console.WriteLine("h--print this menu");
            Console.WriteLine("mv Axis# position--move axis# to position <float>");
            Console.WriteLine("md Axis#--Check if axis has finished moving");
            Console.WriteLine("sm Axis#--Stop axis");
            Console.WriteLine("mo Axis#--Axis# turn motor on");
            Console.WriteLine("mf Axis#--Axis# turn motor off");
            Console.WriteLine("mva Axis# moveInReverse moveSlow--int boolean boolean");
            Console.WriteLine("qp Axis#--query position of Axis#");
            Console.WriteLine("sp Pos#--Set position# to current coordinates of axes");
            Console.WriteLine("gp pos#--Send each axis to the position defined for it by pos#");
            Console.WriteLine("press enter on any blank command line to exit");
        }
    }
}

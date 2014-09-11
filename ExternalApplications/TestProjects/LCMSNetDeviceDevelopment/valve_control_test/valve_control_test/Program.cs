using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            /// 
            /// Created a form to put the control interface on.
            /// 
            Form myForm = new Form();

            /// 
            /// Created a control interface
            /// 
            controlValveVICI2Pos ctl = new controlValveVICI2Pos();
            
            /// 
            /// Created a valve that actually interfaces the 
            /// hardware.
            /// 
            classValveVICI2Pos valve = new classValveVICI2Pos();

            /// 
            /// Construct a  new port for SAG
            /// 
            valve.Port = new System.IO.Ports.SerialPort();
            ctl.Valve = valve;
            
            /// 
            /// Add the control to the dialog form.
            /// 
            myForm.Text = "SAG";
            ctl.Dock = DockStyle.Fill;
            myForm.Controls.Add(ctl);

            /*/// 
            /// This would be within the fluidics designer "valve" class
            /// 
            valve.setPosition(enumValvePosition2Pos.A);
            ctl.Valve.setPosition(enumValvePosition2Pos.B);*/
                        
            Application.Run(myForm);
        }
    }
}

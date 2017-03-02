using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LcmsNetDataClasses.Data
{
    public abstract partial class classDMSBaseControl : UserControl
    {
        public classDMSBaseControl()
        {
            InitializeComponent();
        }

        public int ID { get; set; }

        public abstract bool IsSampleValid { get; }

        public void SetFocusOn(DMSValidatorEventArgs args)
        {
            foreach (Control c in Controls)
            {
                if (c.Name == args.Name)
                {
                    if (c.Enabled == false)
                    {
                        if (EnterPressed != null)
                        {
                            EnterPressed(this, args);
                        }
                    }
                    c.Focus();
                    return;
                }
            }
        }

        public event EventHandler<DMSValidatorEventArgs> EnterPressed;

        public virtual void OnEnterPressed(object sender, DMSValidatorEventArgs e)
        {
            var handler = EnterPressed;
            if (handler != null)
            {
                handler(sender, e);
            }
        }
    }
}
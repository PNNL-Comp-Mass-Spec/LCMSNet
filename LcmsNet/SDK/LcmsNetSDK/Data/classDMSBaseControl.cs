using System;
using System.Windows.Forms;

namespace LcmsNetDataClasses.Data
{
    public abstract partial class classDMSBaseControl : UserControl
    {
        protected classDMSBaseControl()
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
                        EnterPressed?.Invoke(this, args);
                    }
                    c.Focus();
                    return;
                }
            }
        }

        public event EventHandler<DMSValidatorEventArgs> EnterPressed;

        public virtual void OnEnterPressed(object sender, DMSValidatorEventArgs e)
        {
            EnterPressed?.Invoke(sender, e);
        }
    }
}
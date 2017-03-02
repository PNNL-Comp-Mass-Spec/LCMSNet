using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using LcmsNetDataClasses.Method;
using System.Windows.Forms;

namespace LcmsNet.Method.Forms
{
    public partial class controlParameterNumericUpDown : UserControl, ILCEventParameter
    {
        public controlParameterNumericUpDown()
        {
            InitializeComponent();
            mnum_value.Minimum = Convert.ToDecimal(0.0);
            mnum_value.Maximum = Convert.ToDecimal(10000000000.0);
            mnum_value.ValueChanged += new EventHandler(mnum_value_ValueChanged);

            Range = null;
        }

        /// <summary>
        /// Gets or sets the range for this numeric up down.
        /// </summary>
        public classLCMethodParameterNumericRange Range { get; set; }

        public int DecimalPlaces
        {
            get { return mnum_value.DecimalPlaces; }
            set
            {
                if (mnum_value != null)
                    mnum_value.DecimalPlaces = value;
            }
        }

        public event EventHandler EventChanged;

        #region IParameterBase Members

        /// <summary>
        /// Gets the number of the control as a double.
        /// </summary>
        public object ParameterValue
        {
            get { return Convert.ToDouble(mnum_value.Value); }
            set
            {
                if (value == null)
                {
                    mnum_value.Value = 1;
                    return;
                }
                mnum_value.Value = Convert.ToDecimal(value);
            }
        }

        #endregion

        void mnum_value_ValueChanged(object sender, EventArgs e)
        {
            OnEventChanged();
        }

        /// <summary>
        /// Fires the event changed event.
        /// </summary>
        private void OnEventChanged()
        {
            if (EventChanged != null)
            {
                EventChanged(this, null);
            }
        }

        private void mbutton_conversion_Click(object sender, EventArgs e)
        {
            using (
                formConvertToSeconds conversion = new formConvertToSeconds(Convert.ToInt32(mnum_value.Value),
                    DecimalPlaces))
            {
                conversion.StartPosition = FormStartPosition.Manual;
                conversion.Location = this.PointToScreen(this.Location);

                if (ParentForm != null && ParentForm.Icon != null)
                {
                    conversion.Icon = ParentForm.Icon;
                }


                if (conversion.ShowDialog() == DialogResult.OK)
                {
                    switch (conversion.ConversionType)
                    {
                        case ConversionType.Time:
                            mnum_value.Value = Math.Min(mnum_value.Maximum,
                                Math.Max(mnum_value.Minimum, Convert.ToDecimal(conversion.GetTimeInSeconds())));
                            break;
                        case ConversionType.Precision:
                            DecimalPlaces = conversion.GetDecimalPlaces();
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
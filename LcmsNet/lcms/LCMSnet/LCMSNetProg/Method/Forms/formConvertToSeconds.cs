using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LcmsNet.Method.Forms
{
    public partial class formConvertToSeconds : Form
    {
        public formConvertToSeconds()
        {
            InitializeComponent();
        }

        public formConvertToSeconds(int seconds, int precision)
        {
            InitializeComponent();

            try
            {
                mnum_minutes.Value = Convert.ToDecimal(Math.Round(Convert.ToDouble(seconds) / 60.0, 0));
                mnum_seconds.Value = Convert.ToDecimal(seconds % 60);

                mnum_decimalPlaces.Value = Convert.ToDecimal(precision);
            }
            catch
            {
            }
        }

        public ConversionType ConversionType
        {
            get
            {
                if (radioButton1.Checked)
                {
                    return Forms.ConversionType.Time;
                }
                return Forms.ConversionType.Precision;
            }
        }

        public int GetTimeInSeconds()
        {
            int minutes = Convert.ToInt32(mnum_minutes.Value);
            int seconds = Convert.ToInt32(mnum_seconds.Value);

            return minutes * 60 + seconds;
        }

        public int GetDecimalPlaces()
        {
            return Convert.ToInt32(mnum_decimalPlaces.Value);
        }
    }

    public enum ConversionType
    {
        Time,
        Precision
    }
}
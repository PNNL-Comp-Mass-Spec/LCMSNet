using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using System.ComponentModel;

namespace ASIpump
{
    public static class ExtensionMethod
    {
        public static void SafeThreadAction<T>(this T control, Action<T> call) where T : Control
        {
            try
            {
                if (control.InvokeRequired)
                {
                    control.BeginInvoke(call, control);
                }
                else
                {
                    call(control);
                }

            }
            catch{}
        }

        public static Y SafeThreadGet<Y, T>(this T control, Func<T, Y> call) where T : Control
        {
            var result = control.BeginInvoke(call, control);
            var result2 = control.EndInvoke(result);
            return (Y)result2;
        }

        public static void RaiseEvent(this object sender, EventHandler ToRaise, EventArgs args)
        {
            if (ToRaise != null)
            {
                foreach (EventHandler ev in ToRaise.GetInvocationList())
                {
                    //Take a look at the object "receiving" the event.  If it is a Control, invoke on the control.
                    var target = ev.Target as System.Windows.Forms.Control;
                    try
                    {
                        if (target != null)
                        {
                            target.BeginInvoke(ev, sender, args, null, null);
                        }
                        else
                        {
                            ev.BeginInvoke(sender, args, null, null);
                        }
                    }
                    catch { }
                }
            }
        }

        public static void RaisePropertiesChanged(this object sender, PropertyChangedEventHandler ToRaise, PropertyChangedEventArgs args)
        {
            if (ToRaise != null)
            {
                foreach (PropertyChangedEventHandler ev in ToRaise.GetInvocationList())
                {
                    //Take a look at the object "receiving" the event.  If it is a Control, invoke on the control.
                    var target = ev.Target as System.Windows.Forms.Control;
                    try
                    {
                        if (target != null)
                        {
                            target.BeginInvoke(ev, sender, args);
                        }
                        else
                        {
                            ev.BeginInvoke(sender, args, null, null);
                        }
                    }
                    catch { }
                }
            }
        }

        public static double StdDev(this IEnumerable<double> values)
        {
            double ret = 0;

            if (values.Count() > 0)
            {
                //Compute the Average
                var avg = values.Average();

                //Perform the Sum of (value-avg)_2_2
                var sum = values.Sum(d => Math.Pow(d - avg, 2));

                var x2 = (sum) / (double) (values.Count()-1);

                //Put it all together
                ret = Math.Sqrt(x2);
            }
            return ret;
        }

        public static double Ave(this IEnumerable<double> values)
        {
            double ret = 0;

            if (values.Count() > 0)
            {
                //Compute the Average
                ret = values.Average();
            }
            return ret;
        }
    }
}

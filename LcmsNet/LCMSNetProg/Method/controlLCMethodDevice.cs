using System;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;


using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Method;

namespace LcmsNet.Method
{

    public partial class controlLCMethodDevice : UserControl
    {
        internal class MethSelection
        {
            public MethSelection(MethodInfo info, object val)
            {
                Method = info;
                Value  = val;
            }

            public MethodInfo Method { get; set; }
            public object Value { get; set; }
        }

        private IDevice mobj_device;
        private Dictionary<string, MethSelection> mdict_params;

        public controlLCMethodDevice(IDevice device)
        {
            InitializeComponent();

            mobj_device = device;
            mdict_params = new Dictionary<string, MethSelection>();

            ReflectDevice();
        }

        public classLCAction GetAction()
        {

            string key = mcomboBox_method.SelectedItem as string;
            if (mdict_params.ContainsKey(key) == false)
                return null;


            MethSelection sel = mdict_params[key];
            if (sel.Method == null || sel.Value == null)
                return null;

            classLCAction action = new classLCAction();
            action.Duration      = new TimeSpan(0, 0, 0, 0, 0);
            action.Start         = DateTime.Now.AddDays(1.0);
            action.Method        = sel.Method;
            object o = mcomboBox_parameters.SelectedItem;

            action.Method.Invoke(mobj_device, new object[] {o});

            return action;
        }

        public void ReflectDevice()
        {
            if (mobj_device == null)
                return;

            mcomboBox_method.Items.Clear();
            mcomboBox_parameters.Items.Clear();
            mdict_params.Clear();

            Type type = mobj_device.GetType();
            foreach (MethodInfo method in type.GetMethods())
            {
                object[] customs = method.GetCustomAttributes(typeof(classLCMethodAttribute), true);
                foreach (object objAttribute in customs)
                {
                    classLCMethodAttribute attr = objAttribute as classLCMethodAttribute;
                    if (attr != null)
                    {
                        mcomboBox_method.Items.Add(attr.Name);
                        mcomboBox_method.SelectedIndex = 0;

                        ParameterInfo[] info = method.GetParameters();
                        if (info.Length > 0)
                        {
                            foreach (ParameterInfo paramInfo in info)
                            {
                                if (paramInfo.ParameterType.IsEnum == true)
                                {
                                    ///
                                    /// Old reflection?
                                    ///
                                    System.Array aenums = Enum.GetValues(paramInfo.ParameterType);
                                    object [] enums = new object[aenums.Length];
                                    aenums.CopyTo(enums, 0);

                                    mcomboBox_parameters.Items.AddRange(enums);
                                    mcomboBox_parameters.SelectedIndex = 0;
                                    mdict_params.Add(attr.Name, new MethSelection(method, enums[0]));
                                }
                                else if (paramInfo.ParameterType.IsValueType == true)
                                {

                                }
                            }

                        }
                        else
                            mdict_params.Add(attr.Name, new MethSelection(null, null));
                    }
                }
            }
        }
    }
}

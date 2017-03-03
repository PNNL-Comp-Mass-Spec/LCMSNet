using System;
using System.ComponentModel;

using System.Windows.Forms;
using LcmsNetDataClasses.Devices;

namespace ASIpump
{
    [Serializable]
    public partial class PropertyGridPersist : PropertyGrid
    {
        public PropertyGridPersist()
        {
            //this.BrowsableAttributes =
            //    new AttributeCollection(new Attribute[] { new classPersistenceAttribute("Timeout"), new BrowsableAttribute(true) });
            //new AttributeCollection(new Attribute[] {new BrowsableAttribute(true) });
        }


        private object mDisplayedObject = null;
        public object DisplayedObject
        {
            get { return mDisplayedObject; }
            set
            {
                mDisplayedObject = value;
                var notify = value as INotifyPropertyChanged;
                if (notify != null)
                {
                    notify.PropertyChanged += new PropertyChangedEventHandler(notify_PropertyChanged);
                }

                this.SelectedObject = mDisplayedObject;
            }
        }

        private void notify_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            RefreshProperties();
        }

        public void RefreshProperties()
        {
            this.SelectedObject = DisplayedObject;
        }
    }
}


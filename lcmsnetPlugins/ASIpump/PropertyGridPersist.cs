using System;
using System.ComponentModel;

using System.Windows.Forms;

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


        private object mDisplayedObject;
        public object DisplayedObject
        {
            get { return mDisplayedObject; }
            set
            {
                mDisplayedObject = value;
                var notify = value as INotifyPropertyChanged;
                if (notify != null)
                {
                    notify.PropertyChanged += notify_PropertyChanged;
                }

                SelectedObject = mDisplayedObject;
            }
        }

        private void notify_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            RefreshProperties();
        }

        public void RefreshProperties()
        {
            SelectedObject = DisplayedObject;
        }
    }
}


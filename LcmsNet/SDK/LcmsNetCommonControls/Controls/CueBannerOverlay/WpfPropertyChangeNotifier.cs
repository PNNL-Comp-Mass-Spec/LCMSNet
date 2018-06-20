using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace LcmsNetCommonControls.Controls.CueBannerOverlay
{
    /// <summary>
    /// Class for monitoring propertyChanged/valueChanged dependency properties of WPF controls
    /// </summary>
    /// <remarks>This class takes advantage of the fact that bindings use weak references to manage associations so the
    /// class will not root the object who property changes it is watching. It also uses a WeakReference to maintain a
    ///  reference to the object whose property it is watching without rooting that object. In this way, you can
    /// maintain a collection of these objects so that you can unhook the property change later without worrying about
    /// that collection rooting the object whose values you are watching.</remarks>
    /// <remarks>Copied from https://stackoverflow.com/questions/23682232/how-can-i-fix-the-dependencypropertydescriptor-addvaluechanged-memory-leak-on-at and https://agsmith.wordpress.com/2008/04/07/propertydescriptor-addvaluechanged-alternative/ </remarks>
    public class WpfPropertyChangeNotifier : DependencyObject, IDisposable
    {
        #region Member Variables

        private readonly WeakReference<DependencyObject> _propertySource;

        #endregion // Member Variables

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="propertySource"></param>
        /// <param name="path"></param>
        public WpfPropertyChangeNotifier(DependencyObject propertySource, string path)
            : this(propertySource, new PropertyPath(path))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="propertySource"></param>
        /// <param name="property"></param>
        public WpfPropertyChangeNotifier(DependencyObject propertySource, DependencyProperty property)
            : this(propertySource, new PropertyPath(property))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="propertySource"></param>
        /// <param name="property"></param>
        public WpfPropertyChangeNotifier(DependencyObject propertySource, PropertyPath property)
        {
            if (null == propertySource)
                throw new ArgumentNullException(nameof(propertySource));
            if (null == property)
                throw new ArgumentNullException(nameof(property));
            _propertySource = new WeakReference<DependencyObject>(propertySource);
            Binding binding = new Binding
            {
                Path = property,
                Mode = BindingMode.OneWay,
                Source = propertySource
            };
            BindingOperations.SetBinding(this, ValueProperty, binding);
        }
        #endregion // Constructor

        #region PropertySource

        /// <summary>
        /// Get the source of the property, if available
        /// </summary>
        public DependencyObject PropertySource
        {
            get
            {
                if (_propertySource.TryGetTarget(out var target))
                {
                    return target;
                }

                return null;
            }
        }

        /// <summary>
        /// Returns false if the source has been garbage collected
        /// </summary>
        public bool IsSourceAlive
        {
            get { return _propertySource.TryGetTarget(out _); }
        }

        #endregion // PropertySource

        #region Value

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
        typeof(object), typeof(WpfPropertyChangeNotifier), new FrameworkPropertyMetadata(null, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WpfPropertyChangeNotifier notifier = (WpfPropertyChangeNotifier)d;
            if (null != notifier.ValueChanged)
                notifier.ValueChanged(notifier, EventArgs.Empty);
        }

        /// <summary>
        /// Returns/sets the value of the property
        /// </summary>
        /// <seealso cref="ValueProperty"/>
        [Description("Returns/sets the value of the property")]
        [Category("Behavior")]
        [Bindable(true)]
        public object Value
        {
            get
            {
                return GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        #endregion //Value

        #region Events

        /// <summary>
        /// Event handler for valueChanged
        /// </summary>
        public event EventHandler ValueChanged;

        #endregion // Events

        #region IDisposable Members

        /// <summary>
        /// Destructor
        /// </summary>
        ~WpfPropertyChangeNotifier()
        {
            Dispose();
        }

        /// <summary>
        /// Clean up
        /// </summary>
        public void Dispose()
        {
            BindingOperations.ClearBinding(this, ValueProperty);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

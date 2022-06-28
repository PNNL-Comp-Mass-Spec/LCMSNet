using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace LcmsNetCommonControls.Controls
{
    /// <summary>
    /// A repeat button for WPF that can fire a secondary command on release
    /// </summary>
    public class RepeatButtonCommands : RepeatButton
    {
        static RepeatButtonCommands()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(RepeatButtonCommands), new FrameworkPropertyMetadata(typeof(RepeatButtonCommands)));
            ClickModeProperty.OverrideMetadata(typeof(RepeatButtonCommands), new FrameworkPropertyMetadata(ClickMode.Press));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public RepeatButtonCommands() : base()
        {
        }

        /// <summary>
        /// Get or set the ReleaseCommand property
        /// </summary>
        public ICommand ReleaseCommand
        {
            get => (ICommand)GetValue(ReleaseCommandProperty);
            set => SetValue(ReleaseCommandProperty, value);
        }

        /// <summary>
        /// Reflects the parameter to pass to the ReleaseCommandProperty upon execution
        /// </summary>
        public object ReleaseCommandParameter
        {
            get => (object)GetValue(ReleaseCommandParameterProperty);
            set => SetValue(ReleaseCommandParameterProperty, value);
        }

        /// <summary>
        /// The target element on which to fire the release command
        /// </summary>
        public IInputElement ReleaseCommandTarget
        {
            get => (IInputElement)GetValue(ReleaseCommandTargetProperty);
            set => SetValue(ReleaseCommandTargetProperty, value);
        }

        /// <summary>
        /// ReleaseCommand Dependency Property
        /// </summary>
        public static readonly DependencyProperty ReleaseCommandProperty =
            DependencyProperty.Register("ReleaseCommand", typeof(ICommand), typeof(RepeatButtonCommands), new PropertyMetadata((ICommand)null));

        /// <summary>
        /// ReleaseCommandParameter Dependency Property
        /// </summary>
        public static readonly DependencyProperty ReleaseCommandParameterProperty =
            DependencyProperty.Register("ReleaseCommandParameter", typeof(object), typeof(RepeatButtonCommands), new PropertyMetadata(null));

        /// <summary>
        /// ReleaseCommandTarget Dependency Property
        /// </summary>
        public static readonly DependencyProperty ReleaseCommandTargetProperty =
            DependencyProperty.Register("ReleaseCommandTarget", typeof(IInputElement), typeof(RepeatButtonCommands), new PropertyMetadata((IInputElement)null));

        /// <summary>
        /// Actions to perform on left mouse button up
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            // Ignore when in hover-click mode
            if (ClickMode != ClickMode.Hover)
            {
                // Fire release command
                ExecuteReleaseCommand();
            }
        }

        /// <summary>
        /// Actions to perform on key up
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if ((e.Key == Key.Space) && (ClickMode != ClickMode.Hover))
            {
                // Fire release command
                ExecuteReleaseCommand();
            }
        }

        private void ExecuteReleaseCommand()
        {
            var command = ReleaseCommand;
            if (command != null)
            {
                var parameter = ReleaseCommandParameter;
                var target = ReleaseCommandTarget;

                if (command is RoutedCommand routed)
                {
                    if (target == null)
                    {
                        target = this;
                    }
                    if (routed.CanExecute(parameter, target))
                    {
                        routed.Execute(parameter, target);
                    }
                }
                else if (command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
            }
        }
    }
}

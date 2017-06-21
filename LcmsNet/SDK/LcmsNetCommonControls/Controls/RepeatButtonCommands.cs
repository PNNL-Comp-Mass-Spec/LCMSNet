using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace LcmsNetCommonControls.Controls
{
    public class RepeatButtonCommands : RepeatButton
    {
        static RepeatButtonCommands()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(RepeatButtonCommands), new FrameworkPropertyMetadata(typeof(RepeatButtonCommands)));
            ClickModeProperty.OverrideMetadata(typeof(RepeatButtonCommands), new FrameworkPropertyMetadata(ClickMode.Press));
        }

        public RepeatButtonCommands() : base()
        {
        }

        /// <summary>
        /// Get or set the ReleaseCommand property
        /// </summary>
        public ICommand ReleaseCommand
        {
            get { return (ICommand)GetValue(ReleaseCommandProperty); }
            set { SetValue(ReleaseCommandProperty, value); }
        }

        /// <summary>
        /// Reflects the parameter to pass to the ReleaseCommandProperty upon execution
        /// </summary>
        public object ReleaseCommandParameter
        {
            get { return (object)GetValue(ReleaseCommandParameterProperty); }
            set { SetValue(ReleaseCommandParameterProperty, value); }
        }

        /// <summary>
        /// The target element on which to fire the release command
        /// </summary>
        public IInputElement ReleaseCommandTarget
        {
            get { return (IInputElement)GetValue(ReleaseCommandTargetProperty); }
            set { SetValue(ReleaseCommandTargetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReleaseCommandProperty =
            DependencyProperty.Register("ReleaseCommand", typeof(ICommand), typeof(RepeatButtonCommands), new PropertyMetadata((ICommand)null));

        // Using a DependencyProperty as the backing store for ReleaseCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReleaseCommandParameterProperty =
            DependencyProperty.Register("ReleaseCommandParameter", typeof(object), typeof(RepeatButtonCommands), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for ReleaseCommandTarget.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReleaseCommandTargetProperty =
            DependencyProperty.Register("ReleaseCommandTarget", typeof(IInputElement), typeof(RepeatButtonCommands), new PropertyMetadata((IInputElement)null));

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

                var routed = command as RoutedCommand;
                if (routed != null)
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

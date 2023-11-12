using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Veldrid.Tests.Android.Forms.Utilities
{
    public class CommandViewCell : ViewCell
    {
        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(CommandViewCell), default(ICommand),
                  propertyChanging: (bindable, oldvalue, newvalue) =>
                  {
                      if (oldvalue is ICommand oldCommand)
                      {
                          oldCommand.CanExecuteChanged -= ((CommandViewCell)bindable).OnCommandCanExecuteChanged;
                      }
                  }, propertyChanged: (bindable, oldvalue, newvalue) =>
                  {
                      if (newvalue is ICommand newCommand)
                      {
                          CommandViewCell commandViewCell = (CommandViewCell)bindable;
                          commandViewCell.IsEnabled = newCommand.CanExecute(commandViewCell.CommandParameter);
                          newCommand.CanExecuteChanged += commandViewCell.OnCommandCanExecuteChanged;
                      }
                  });

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(CommandViewCell), default,
                   propertyChanged: (bindable, oldvalue, newvalue) =>
                   {
                       CommandViewCell commandViewCell = (CommandViewCell)bindable;
                       if (commandViewCell.Command != null)
                       {
                           commandViewCell.IsEnabled = commandViewCell.Command.CanExecute(newvalue);
                       }
                   });

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public CommandViewCell()
        {
        }

        protected override void OnTapped()
        {
            base.OnTapped();

            if (!IsEnabled)
            {
                return;
            }

            Command?.Execute(CommandParameter);
        }

        void OnCommandCanExecuteChanged(object? sender, EventArgs eventArgs)
        {
            IsEnabled = Command.CanExecute(CommandParameter);
        }
    }
}

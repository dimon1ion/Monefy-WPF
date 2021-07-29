using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Monefy_WPF.Service.Command
{
    public class Command : ICommand
    {
        public static Func<object, bool> defaultCanExecuteMethod = (obj) => true;
        private Func<object, bool> canExecuteMethod;
        private Action<object> executeMethod;

        public Command(Action<object> executeMethod) : this(executeMethod, defaultCanExecuteMethod)
        {

        }
        public Command(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            this.canExecuteMethod = canExecuteMethod;
            this.executeMethod = executeMethod;
        }

        public bool CanExecute(object parameter)
        {
            return canExecuteMethod(parameter);
        }

        public void Execute(object parameter)
        {
            executeMethod(parameter);
        }

        public void Check()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler CanExecuteChanged;
    }
}

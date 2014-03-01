using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RXL.WPFClient
{
    public class RelayCommand : ICommand
    {
        private readonly Predicate<Object> _canExecute;
        private readonly Action<Object> _execute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Predicate<Object> canExecute, Action<Object> execute)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        public bool CanExecute(Object parameter)
        {
            return _canExecute(parameter);
        }

        public void Execute(Object parameter)
        {
            _execute(parameter);
        }

        public void NotifyCanExecuteChanged(Object parameter)
        {
            if(CanExecuteChanged != null)
                CanExecuteChanged.Invoke(parameter, new EventArgs());
        }
    }
}

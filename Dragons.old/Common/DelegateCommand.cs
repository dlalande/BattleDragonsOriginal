using System;
using System.Windows.Input;

namespace Dragons.Common
{
    /// <summary>
    /// Represents commands that you can implement entirely in bindable 
    /// view model properties so that you don't have to create a separate 
    /// ICommand implementation for each command. 
    /// </summary>
    public sealed class DelegateCommand : ICommand
    {
        /// <summary>
        /// Gets or sets the action to perform when the command executes.
        /// </summary>
        public Action Execute { private get; set; }

        /// <summary>
        /// Gets or sets the function that determines whether the command can be executed.
        /// </summary>
        public Func<bool> CanExecute { private get; set; }

        /// <summary>
        /// Gets a value that indicates whether the command can execute.
        /// </summary>
        /// <param name="parameter">The parameter associated with the command, if any.</param>
        /// <returns>True if the command can execute; otherwise, false.</returns>
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute == null || CanExecute();
        }

        /// <summary>
        /// Occurs when the CanExecute method return value has potentially changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raises the CanExecuteChanged event.
        /// </summary>
        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null) CanExecuteChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Performs the action associated with the command.
        /// </summary>
        /// <param name="parameter">The parameter associated with the command, if any.</param>
        void ICommand.Execute(object parameter)
        {
            Execute();
        }
    }

    /// <summary>
    /// Represents commands that you can implement entirely in bindable 
    /// view model properties so that you don't have to create a separate 
    /// ICommand implementation for each command. This generic version
    /// supports method parameters for Execute and CanExecute. 
    /// </summary>
    public sealed class DelegateCommand<T> : ICommand
    {
        /// <summary>
        /// Gets or sets the action to perform when the command executes.
        /// </summary>
        public Action<T> Execute { private get; set; }

        /// <summary>
        /// Gets or sets the function that determines whether the command can be executed.
        /// </summary>
        public Predicate<T> CanExecute { private get; set; }

        /// <summary>
        /// Gets a value that indicates whether the command can execute.
        /// </summary>
        /// <param name="parameter">The parameter associated with the command, if any.</param>
        /// <returns>True if the command can execute; otherwise, false.</returns>
        bool ICommand.CanExecute(object parameter)
        {
            if (parameter == null) return false;
            return CanExecute == null || CanExecute((T)parameter);
        }

        /// <summary>
        /// Occurs when the CanExecute method return value has potentially changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raises the CanExecuteChanged event.
        /// </summary>
        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null) CanExecuteChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Performs the action associated with the command.
        /// </summary>
        /// <param name="parameter">The parameter associated with the command, if any.</param>
        void ICommand.Execute(object parameter)
        {
            Execute((T)parameter);
        }
    }
}

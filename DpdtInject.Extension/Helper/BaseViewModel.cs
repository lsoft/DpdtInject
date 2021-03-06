using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Threading;

namespace DpdtInject.Extension.Helper
{
    /// <summary>
    /// Класс, реализующий базовую функциональность viewmodel идеологии MVVM
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Диспатчер WPF, необходим для обновления привязок команд
        /// </summary>
        protected readonly Dispatcher _dispatcher;

        /// <summary>
        /// Событие изменения свойства
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Тестовое свойство бросания исключения в случае не нахождения биндинга
        /// </summary>
        protected bool _throwOnInvalidPropertyName;

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="dispatcher">Диспатчер WPF</param>
        protected BaseViewModel(Dispatcher dispatcher)
        {
            #region validate

            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }

            #endregion

            _dispatcher = dispatcher;
        }

        /// <summary>
        /// Активация евента изменения бинденого свойства
        /// </summary>
        protected void OnPropertyChanged()
        {
            OnPropertyChanged(string.Empty);
        }

        /// <summary>
        /// Активация евента изменения бинденого свойства
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);

            var handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        /// <summary>
        /// Вызывает обновление биндингов, например, на буттонах
        /// (автоматически в .net3.5 этого не происходит, похоже,
        /// хотя в .net4 происходит)
        /// Именно эта функция использует диспатчер
        /// </summary>
        protected virtual void OnCommandInvalidate()
        {
            this._dispatcher.BeginInvoke(
                new Action(CommandManager.InvalidateRequerySuggested));
        }


        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                // Verify that the property name matches a real,  
                // public, instance property on this object.
                var propertiesList = TypeDescriptor.GetProperties(this);
                if (propertiesList[propertyName] == null)
                {
                    var msg = "Invalid property name: " + propertyName;

                    if (this._throwOnInvalidPropertyName)
                    {
                        throw new Exception(msg);
                    }

                    Debug.Fail(msg);
                }
            }
        }

        protected virtual void DisposeViewModel()
        {
            
        }

        protected void BeginInvoke(Action a)
        {
            _dispatcher.BeginInvoke(a);
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            this.DisposeViewModel();
        }

        #endregion
    }
}

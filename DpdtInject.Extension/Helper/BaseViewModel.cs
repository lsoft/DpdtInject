using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace DpdtInject.Extension.Helper
{
    /// <summary>
    /// Класс, реализующий базовую функциональность viewmodel идеологии MVVM
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged, IDisposable
    {
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
        protected BaseViewModel()
        {
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

            CommandManager.InvalidateRequerySuggested();
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

        #region Implementation of IDisposable

        public void Dispose()
        {
            this.DisposeViewModel();
        }

        #endregion
    }
}

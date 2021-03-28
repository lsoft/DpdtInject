using DpdtInject.Extension.Helper;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Task = System.Threading.Tasks.Task;

namespace DpdtInject.Extension.UI.ViewModel.AddClusterMethod
{
    public class AddClusterMethodViewModel : BaseViewModel
    {
        private readonly AddClusterData _addClusterData;

        private ICommand? _closeCommand;
        private ICommand? _nextCommand;

        public string AdditionalFolders
        {
            get => _addClusterData.AdditionalFolders;
            set
            {
                _addClusterData.AdditionalFolders = value;
                OnPropertyChanged();
            }
        }

        public string? ClusterClassName
        {
            get => _addClusterData.ClusterClassName;
            set
            {
                _addClusterData.ClusterClassName = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> ClusterClassNameList
        {
            get;
        }

        public string BindingMethodName
        {
            get => _addClusterData.BindingMethodName;
            set
            {
                _addClusterData.BindingMethodName = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get
            {
                _ = _addClusterData.TryValidate(out var errorMessage);
                return errorMessage ?? string.Empty;
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand(
                        a =>
                        {
                            if (a is System.Windows.Window w)
                            {
                                w.Close();
                            }
                        });
                }

                return _closeCommand;
            }
        }

        public ICommand NextCommand
        {
            get
            {
                if (_nextCommand == null)
                {
                    _nextCommand = new AsyncRelayCommand(
                        async a =>
                        {
                            var filePath = await _addClusterData.ProcessAsync();

                            if (filePath == null)
                            {
                                return;
                            }

                            if (a is System.Windows.Window w)
                            {
                                w.Close();
                            }
                        },
                        r => _addClusterData.TryValidate(out _)
                        );
                }

                return _nextCommand;
            }
        }

        public AddClusterMethodViewModel(
            AddClusterData addClusterData
            )
        {
            if (addClusterData is null)
            {
                throw new System.ArgumentNullException(nameof(addClusterData));
            }

            _addClusterData = addClusterData;

            ClusterClassNameList = new ObservableCollection<string>(_addClusterData.ClusterClassNameList);
        }
    }
}

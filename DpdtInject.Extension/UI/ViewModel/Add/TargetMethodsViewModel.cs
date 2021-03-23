using DpdtInject.Extension.Container;
using DpdtInject.Extension.UI.ChainStep;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Task = System.Threading.Tasks.Task;
using DpdtInject.Extension.UI.ViewModel.Add.Inner;

namespace DpdtInject.Extension.UI.ViewModel.Add
{
    public class TargetMethodsViewModel : ChainViewModel
    {
        private readonly ChoosedParameters _choosedParameters;
        private readonly Func<Task> _previousStepAction;
        private readonly Func<Task> _nextStepAction;

        private ICommand? _nextCommand;
        private ICommand? _previousCommand;
        private ICommand? _closeCommand;

        private ContainerAndScanner? _containerAndScanner;


        public ObservableCollection<TargetMethodViewModel> TargetMethodList
        {
            get;
        } = new ObservableCollection<TargetMethodViewModel>();



        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand(
                        a =>
                        {
                            if (a is Window w)
                            {
                                w.Close();
                            }
                        });
                }

                return _closeCommand;
            }
        }


        public ICommand PreviousCommand
        {
            get
            {
                if (_previousCommand == null)
                {
                    _previousCommand = new AsyncRelayCommand(
                        async a =>
                        {
                            _choosedParameters.ChoosedTargetMethod = null;
                            await _previousStepAction();
                        },
                        r => true
                        );
                }

                return _previousCommand;
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
                            _choosedParameters.ChoosedTargetMethod = GetChoosedTargetMethod();
                            await _nextStepAction();
                        },
                        r => true
                        );
                }

                return _nextCommand;
            }
        }

        public TargetMethodsViewModel(
            ChoosedParameters choosedParameters,
            Func<Task> previousStepAction,
            Func<Task> nextStepAction
            )
        {
            if (choosedParameters is null)
            {
                throw new ArgumentNullException(nameof(choosedParameters));
            }

            if (previousStepAction is null)
            {
                throw new ArgumentNullException(nameof(previousStepAction));
            }

            if (nextStepAction is null)
            {
                throw new ArgumentNullException(nameof(nextStepAction));
            }
            _choosedParameters = choosedParameters;
            _previousStepAction = previousStepAction;
            _nextStepAction = nextStepAction;
        }

        public override async Task StartAsync()
        {
            var targetMethodList = new List<TargetMethodViewModel>();

            var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));

            _containerAndScanner = componentModel.GetService<ContainerAndScanner>();

            var solutionBind = _containerAndScanner.Binds;
            if (solutionBind != null)
            {
                foreach (var ppair in solutionBind.Dict)
                {
                    var projectBind = ppair.Value;

                    foreach (var cpair in projectBind.DictByDisplayString)
                    {
                        var clusterBind = cpair.Value;

                        foreach (var mpair in clusterBind.Dict)
                        {
                            var methodBind = mpair.Value;

                            targetMethodList.Add(
                                new TargetMethodViewModel(
                                    methodBind
                                    )

                                );
                        }
                    }
                }
            }

            if (targetMethodList.Count <= 0)
            {
                return;
            }


            foreach (var bm in targetMethodList.OrderBy(bm => bm.MethodBindContainer.ClusterType.ToDisplayString()))
            {
                TargetMethodList.Add(bm);
            }

            if (_choosedParameters.ChoosedTargetMethod != null)
            {
                var foundTargetMethod = TargetMethodList.FirstOrDefault(tm => ReferenceEquals(tm.MethodBindContainer.MethodSyntax, _choosedParameters.ChoosedTargetMethod.MethodSyntax));
                if (foundTargetMethod != null)
                {
                    foundTargetMethod.IsChecked = true;
                    return;
                }
            }

            TargetMethodList[0].IsChecked = true;
        }


        private MethodBindContainer GetChoosedTargetMethod()
        {
            return TargetMethodList.First(t => t.IsChecked).MethodBindContainer;
        }

    }
}

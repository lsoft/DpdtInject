using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DpdtInject.Extension.Shared;
using DpdtInject.Extension.Shared.Dto;
using Microsoft.VisualStudio.Core.Imaging;
using Microsoft.VisualStudio.Language.CodeLens;
using Microsoft.VisualStudio.Language.CodeLens.Remoting;
using Microsoft.VisualStudio.Threading;

using static DpdtInject.Extension.Shared.Logging;

namespace DpdtInject.Extension.CodeLens
{
    public class DpdtCodeLensDataPoint : IAsyncCodeLensDataPoint, IDisposable
    {
        private readonly ICodeLensCallbackService _callbackService;
        private readonly CodeLensDescriptor _descriptor;
        
        private VisualStudioConnectionHandler? _visualStudioConnection;
        private readonly ManualResetEventSlim _dataHasLoaded = new ManualResetEventSlim(initialState: false);
        
        private DpdtBindingReferenceSet? _bindings;

        public DpdtCodeLensDataPoint(
            ICodeLensCallbackService callbackService,
            CodeLensDescriptor descriptor
            )
        {
            if (callbackService is null)
            {
                throw new ArgumentNullException(nameof(callbackService));
            }

            if (descriptor is null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            _callbackService = callbackService;
            _descriptor = descriptor;
        }

        public event AsyncEventHandler? InvalidatedAsync;

        public CodeLensDescriptor Descriptor => this._descriptor;

        public Guid UniqueIdentifier
        {
            get;
        } = Guid.NewGuid();

        #region network related code

        internal async Task ConnectToVisualStudioAsync(
            int vspid
            )
        {
            _visualStudioConnection = await VisualStudioConnectionHandler
                .Create(owner: this, vspid)
                .ConfigureAwait(false)
                ;
        }

        // Called from VS via JSON RPC.
        public void Refresh()
        {
            Invalidate();
        }

        public void Dispose()
        {
            _visualStudioConnection?.Dispose();
            _dataHasLoaded.Dispose();
        }

        #endregion

        public async Task<CodeLensDataPointDescriptor> GetDataAsync(CodeLensDescriptorContext context, CancellationToken token)
        {
            try
            {
                _bindings = await GetReferenceSetAsync(context, token);
                _dataHasLoaded.Set();

                if (_bindings == null)
                {
                    var response = new CodeLensDataPointDescriptor()
                    {
                        Description = $"Dpdt: no data available",
                        TooltipText = $"Dpdt binding actual status",
                        IntValue = null, // no int value
                        ImageId = GetTypeIcon(),
                    };

                    return response;
                }

                if (_bindings.Status == DpdtBindingReferenceSetStatusEnum.Disabled)
                {
                    var response = new CodeLensDataPointDescriptor()
                    {
                        Description = $"Dpdt: DISABLED",
                        TooltipText = $"Dpdt binding actual status",
                        IntValue = null, // no int value
                        ImageId = GetTypeIcon(),
                    };

                    return response;
                }

                if (_bindings.Status == DpdtBindingReferenceSetStatusEnum.InProgress)
                {
                    var response = new CodeLensDataPointDescriptor()
                    {
                        Description = $"Dpdt: scanning...",
                        TooltipText = $"Dpdt binding actual status",
                        IntValue = null, // no int value
                        ImageId = GetTypeIcon(),
                    };

                    return response;
                }

                var bindCount = _bindings.GetBindingCount();
                if (bindCount > 1)
                {
                    var response = new CodeLensDataPointDescriptor()
                    {
                        Description = $"Dpdt: bind at {bindCount} points",
                        TooltipText = $"Dpdt binding actual status",
                        IntValue = null, // no int value
                        ImageId = GetTypeIcon(),
                    };

                    return response;
                }
                else if (bindCount == 0)
                {
                    var response = new CodeLensDataPointDescriptor()
                    {
                        Description = $"Dpdt: no bind found",
                        TooltipText = $"Dpdt binding actual status",
                        IntValue = null, // no int value
                        ImageId = GetTypeIcon()
                    };

                    return response;
                }
                else
                {
                    var response = new CodeLensDataPointDescriptor()
                    {
                        Description = $"Dpdt: bind at {_bindings.BindingTargets[0].ClusterDetail.FullName}",
                        TooltipText = $"Dpdt binding actual status",
                        IntValue = null, // no int value
                        ImageId = GetTypeIcon(),
                    };

                    return response;
                }
            }
            catch (Exception ex)
            {
                LogCL(ex);
                throw;
            }
        }


        public async Task<CodeLensDetailsDescriptor> GetDetailsAsync(CodeLensDescriptorContext context, CancellationToken token)
        {
            try
            {
                // When opening the details pane, the data point is re-created leaving `data` uninitialized. VS will
                // then call `GetDataAsync()` and `GetDetailsAsync()` concurrently.
                if (!_dataHasLoaded.Wait(timeout: TimeSpan.FromSeconds(.5), token))
                {
                    _bindings = await GetReferenceSetAsync(context, token);
                }

                var result = new CodeLensDetailsDescriptor()
                {
                    Headers = CreateHeaders(),
                    Entries = CreateEntries(),
                    CustomData =
                        _bindings != null 
                            ? new List<object>() { _bindings }
                            : new List<object>(),
                    PaneNavigationCommands = new List<CodeLensDetailPaneCommand>()
                    {
                        //new CodeLensDetailPaneCommand
                        //{
                        //    CommandDisplayName = "Add binding...",
                        //    CommandId = _addBindingCommandId,
                        //    CommandArgs = new object[] { /*(object)id*/ }
                        //}
                    },
                };

                return result;
            }
            catch (Exception ex)
            {
                LogCL(ex);
                throw;
            }
        }

        /// <summary>
        /// Raises <see cref="IAsyncCodeLensDataPoint.Invalidated"/> event.
        /// </summary>
        /// <remarks>
        ///  This is not part of the IAsyncCodeLensDataPoint interface.
        ///  The data point source can call this method to notify the client proxy that data for this data point has changed.
        /// </remarks>
        public void Invalidate()
        {
            _dataHasLoaded.Reset();
            this.InvalidatedAsync?.Invoke(this, EventArgs.Empty).ConfigureAwait(false);
        }


        private async Task<DpdtBindingReferenceSet?> GetReferenceSetAsync(
            CodeLensDescriptorContext context,
            CancellationToken token
            )
        {
            DpdtBindingReferenceSet? bindings = null;

            try
            {
                bindings = await _callbackService
                    .InvokeAsync<DpdtBindingReferenceSet>(
                        this,
                        nameof(IDpdtCodeLensListener.GetReferenceSet),
                        new object[]
                        {
                            new CodeLensTarget(
                                _descriptor.ProjectGuid,
                                _descriptor.FilePath,
                                (string)context.Properties["FullyQualifiedName"],
                                context.ApplicableSpan
                                )
                        },
                        token
                        )
                    .ConfigureAwait(false)
                    ;
            }
            catch (Exception ex)
            {
                LogCL(ex);
            }

            return bindings;
        }

        private static IEnumerable<CodeLensDetailEntryDescriptor> CreateEntries()
        {
            yield break;
        }

        private static List<CodeLensDetailHeaderDescriptor> CreateHeaders()
        {
            return new List<CodeLensDetailHeaderDescriptor>()
            {
            };
        }

        private static ImageId GetTypeIcon()
        {
            return new ImageId(
                new Guid("{bbd8a64b-7fd0-47fb-a600-503d90f22239}"),
                0
                );
        }
    }
}

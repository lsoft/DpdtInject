using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DpdtInject.Extension.Shared;
using Microsoft.VisualStudio.Language.CodeLens;
using Microsoft.VisualStudio.Language.CodeLens.Remoting;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Utilities;

using static DpdtInject.Extension.Shared.Logging;

namespace DpdtInject.Extension.CodeLens
{
    [Export(typeof(IAsyncCodeLensDataPointProvider))]
    [Name(Id)]
    [ContentType("code")]
    [LocalizedName(typeof(Resources), "DpdtCodeLensProvider")]
    [Priority(200)]
    public class DpdtCodeLensPointProvider : IAsyncCodeLensDataPointProvider
    {
        internal const string Id = "DpdtCodeLensProviderName";

        private readonly Lazy<ICodeLensCallbackService> _callbackService;


        [ImportingConstructor]
        public DpdtCodeLensPointProvider(
            Lazy<ICodeLensCallbackService> callbackService
            )
        {
            if (callbackService is null)
            {
                throw new ArgumentNullException(nameof(callbackService));
            }

            _callbackService = callbackService;
        }

        public async Task<bool> CanCreateDataPointAsync(CodeLensDescriptor descriptor, CodeLensDescriptorContext context, CancellationToken token)
        {
            var result = false;

            if (await IsEnabled())
            {
                if (descriptor.Kind == CodeElementKinds.Type)
                {
                    //it's a type
                    result = true;
                }
                //else if (descriptor.Kind == CodeElementKinds.Method)
                //{
                //    var parts = descriptor.ElementDescription.Split('.');
                //    if (parts.Length == 2)
                //    {
                //        if (parts[0] == parts[1])
                //        {
                //            //it's a constructor
                //            result = true;
                //        }
                //    }
                //}
            }

            return 
                //Task.FromResult(
                    result
                //)
                ;
        }

        public async Task<bool> IsEnabled()
        {
            try
            {
                return await _callbackService
                    .Value
                    .InvokeAsync<bool>(this, nameof(IDpdtCodeLensListener.IsEnabled))
                    .ConfigureAwait(false)
                    ;
            }
            catch (Exception ex)
            {
                LogCL(ex);
                throw;
            }

        }

        public async Task<IAsyncCodeLensDataPoint> CreateDataPointAsync(CodeLensDescriptor descriptor, CodeLensDescriptorContext context, CancellationToken token)
        {
            try
            {
                var dp = new DpdtCodeLensDataPoint(
                    _callbackService.Value, 
                    descriptor
                    );

                var vspid = await _callbackService.Value
                    .InvokeAsync<int>(this, nameof(IDpdtCodeLensListener.GetVisualStudioPid))
                    .ConfigureAwait(false)
                    ;

                await dp.ConnectToVisualStudioAsync(vspid).ConfigureAwait(false);

                return dp;
            }
            catch (Exception ex)
            {
                LogCL(ex);
                throw;
            }

        }
    }
}

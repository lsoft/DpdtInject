using System;
using System.IO.Pipes;
using System.Threading.Tasks;
using DpdtInject.Extension.CodeLens;
using StreamJsonRpc;

namespace DpdtInject.Extension.Shared
{
    /// <summary>
    /// Taken from  https://github.com/bert2/microscope completely.
    /// Take a look to that repo, it's amazing!
    /// </summary>
    public class VisualStudioConnectionHandler : IRemoteCodeLens, IDisposable
    {
        private readonly DpdtCodeLensDataPoint _owner;
        private readonly NamedPipeClientStream _stream;
        private JsonRpc? _rpc;

        public async static Task<VisualStudioConnectionHandler> Create(DpdtCodeLensDataPoint owner, int vspid)
        {
            var handler = new VisualStudioConnectionHandler(owner, vspid);
            await handler.Connect().ConfigureAwait(false);
            return handler;
        }

        public VisualStudioConnectionHandler(DpdtCodeLensDataPoint owner, int vspid)
        {
            _owner = owner;
            _stream = new NamedPipeClientStream(
                serverName: ".",
                PipeName.Get(vspid),
                PipeDirection.InOut,
                PipeOptions.Asynchronous
                );
        }

        public void Dispose() => _stream.Dispose();

        public async Task Connect()
        {
            await _stream.ConnectAsync().ConfigureAwait(false);
            _rpc = JsonRpc.Attach(_stream, this);
            await _rpc.InvokeAsync(nameof(IRemoteVisualStudio.RegisterCodeLensDataPoint), _owner.UniqueIdentifier).ConfigureAwait(false);
        }

        public void Refresh() => _owner.Refresh();
    }
}

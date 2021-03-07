using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DpdtInject.Extension.Shared;
using Microsoft;
using StreamJsonRpc;
using CodeLensConnections = System.Collections.Concurrent.ConcurrentDictionary<System.Guid, DpdtInject.Extension.CodeLens.CodeLensConnectionHandler>;

using static DpdtInject.Extension.Shared.Logging;


namespace DpdtInject.Extension.CodeLens
{
    public class CodeLensConnectionHandler : IRemoteVisualStudio, IDisposable
    {
        private static readonly CodeLensConnections _connections = new CodeLensConnections();

        private JsonRpc? _rpc;
        private Guid? _dataPointId;

        public static async Task AcceptCodeLensConnectionsAsync()
        {
            try
            {
                while (true)
                {
                    var stream = new NamedPipeServerStream(
                        PipeName.Get(Process.GetCurrentProcess().Id),
                        PipeDirection.InOut,
                        NamedPipeServerStream.MaxAllowedServerInstances,
                        PipeTransmissionMode.Byte,
                        PipeOptions.Asynchronous);
                    await stream.WaitForConnectionAsync().ConfigureAwait(false);
                    _ = HandleConnectionAsync(stream);
                }
            }
            catch (Exception ex)
            {
                LogVS(ex);
                throw;
            }

            static async Task HandleConnectionAsync(NamedPipeServerStream stream)
            {
                try
                {
                    using (var handler = new CodeLensConnectionHandler())
                    {
                        var rpc = JsonRpc.Attach(stream, handler);
                        handler._rpc = rpc;
                        await rpc.Completion;
                    }
                }
                catch (Exception ex)
                {
                    LogVS(ex);
                }
                finally
                {
                    stream.Dispose();
                }
            }
        }

        public void Dispose()
        {
            if (_dataPointId.HasValue)
            {
                _ = _connections.TryRemove(_dataPointId.Value, out var _);
            }
        }

        // Called from each CodeLensDataPoint via JSON RPC.
        public void RegisterCodeLensDataPoint(Guid id)
        {
            _dataPointId = id;
            _connections[id] = this;
        }


        public static Task RefreshCodeLensDataPointAsync(Guid id)
        {
            if (!_connections.TryGetValue(id, out var conn))
            {
                throw new InvalidOperationException($"CodeLens data point {id} was not registered.");
            }

            if (conn == null)
            {
                return new Task(() => { });
            }

            return conn._rpc!.InvokeAsync(nameof(IRemoteCodeLens.Refresh));
        }

        public static Task RefreshAllCodeLensDataPointsAsync()
        {
            try
            {
                return Task.WhenAll(_connections.Keys.Select(RefreshCodeLensDataPointAsync));
            }
            catch (Exception excp)
            {
                LogVS(excp);
            }

            return new Task(() => { });
        }
    }
}

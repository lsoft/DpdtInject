#pragma warning disable CS0105
using BindFromType = System.Object;
using SessionSaver = DpdtInject.Injector.Bind.BaseSessionSaver;
//PROXYPRODUCER: additonal usings
// ReSharper disable SuspiciousTypeConversion.Global


namespace DpdtInject.Generator.Core
{
    public partial class CarcassProxy
        : BindFromType
    {
        private static readonly double _stopwatchFrequency = (double)global::System.Diagnostics.Stopwatch.Frequency;

        private readonly BindFromType _payload;
        private readonly SessionSaver _sessionSaver;

        private readonly string _payloadFullName;

        public CarcassProxy(
            BindFromType payload,
            SessionSaver sessionSaver
            )
        {
            if (payload is null)
            {
                throw new global::System.ArgumentNullException(nameof(payload));
            }

            if (sessionSaver is null)
            {
                throw new global::System.ArgumentNullException(nameof(sessionSaver));
            }

            _payload = payload;
            _sessionSaver = sessionSaver;

            _payloadFullName = payload.GetType().FullName ?? nameof(CarcassProxy);
        }

        //PROXYPRODUCER: put methods here

    }
}

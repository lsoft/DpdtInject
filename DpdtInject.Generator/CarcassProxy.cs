﻿#pragma warning disable CS0105
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using BindFromType = System.Object;
using SessionSaver = DpdtInject.Injector.Bind.BaseSessionSaver;
// ReSharper disable SuspiciousTypeConversion.Global

namespace DpdtInject.Generator
{
    public partial class CarcassProxy
        : BindFromType
    {
        private static readonly double _stopwatchFrequency = (double)Stopwatch.Frequency;

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
                throw new ArgumentNullException(nameof(payload));
            }

            if (sessionSaver is null)
            {
                throw new ArgumentNullException(nameof(sessionSaver));
            }

            _payload = payload;
            _sessionSaver = sessionSaver;

            _payloadFullName = payload.GetType().FullName ?? nameof(CarcassProxy);
        }

        //PROXYPRODUCER: put methods here


        //PROXYPRODUCER: aggressive inline and optimize
        private void Invoke(
            in Action invokeAction,
            in string memberName,
            in object[]? arguments
            )
        {
            if (invokeAction is null)
            {
                throw new ArgumentNullException(nameof(invokeAction));
            }

            var startDate = Stopwatch.GetTimestamp();
            try
            {

                invokeAction();

                _sessionSaver.FixSessionSafely(
                    _payloadFullName,
                    memberName,
                    (Stopwatch.GetTimestamp() - startDate) / _stopwatchFrequency,
                    null,
                    arguments
                    );
            }
            catch (Exception excp)
            {
                _sessionSaver.FixSessionSafely(
                    _payloadFullName,
                    memberName,
                    (Stopwatch.GetTimestamp() - startDate) / _stopwatchFrequency,
                    excp,
                    arguments
                    );

                throw;
            }
        }
    }
}

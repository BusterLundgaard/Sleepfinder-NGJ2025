// Copyright (c) coherence ApS.
// See the license file in the package root for more information.

namespace Coherence.Runtime
{
    using System;
    using System.Threading;
    using Log;

    public interface IWebSocket
    {
        bool PingWebSocket { get; }
        
        bool IsConnected();
        void OpenSocket(string endpoint, CancellationTokenSource abortToken, Action onConnect, Action onDisconnect,
            Action<string> onReceive, Action<Error, string> onError, Action<int, string, Error, string> onSendFail);
        void CloseSocket();
        void Send(int requestCounter, string requestId, string message);
        void Update();
        void Dispose();
    }
}

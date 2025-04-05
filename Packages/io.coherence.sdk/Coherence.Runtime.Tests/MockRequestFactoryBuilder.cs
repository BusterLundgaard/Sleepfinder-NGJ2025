// Copyright (c) coherence ApS.
// See the license file in the package root for more information.
namespace Coherence.Runtime.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Cloud;
    using Coherence.Utils;
    using Common;
    using Moq;

    /// <summary>
    /// Can be used to <see cref="Build"/> a mock <see cref="IRequestFactoryInternal"/>
    /// object for use in a test.
    /// </summary>
    internal sealed class MockRequestFactoryBuilder
    {
        /// <summary>
        /// <see cref="LoginResponse.SessionToken"/> value that the mock returns be default when
        /// <see cref="IRequestFactoryInternal.SendRequestAsync"/> is called, if <see cref="SetSessionToken"/>
        /// is not used to override it.
        /// </summary>
        public const string DefaultSessionToken = "DefaultSessionToken";

        private Mock<IRequestFactoryInternal> mock;

        private bool isReady = true;
        private Func<bool> isReadyGetter;
        private Func<Task<string>> sendRequestAsyncResult;

        private RequestException sendRequestAsyncThrows;
        private string sessionToken = DefaultSessionToken;
        private (string basePath, string pathParams, string method, string body, Dictionary<string, string> headers, string requestName, string sessionToken)? sendRequestAsyncWasCalledWith;
        private Func<string, string, string, string, Dictionary<string, string>, string, string, Task<string>> sendRequestAsyncFunc;
        private Func<Task<string>>[] sendRequestAsyncSequence;

        public bool SendRequestAsyncWasCalled => sendRequestAsyncWasCalledWith.HasValue;
        public (string basePath, string pathParams, string method, string body, Dictionary<string, string> headers, string requestName, string sessionToken) SendRequestAsyncWasCalledWith => sendRequestAsyncWasCalledWith.Value;

        public MockRequestFactoryBuilder() => isReadyGetter = ()=> isReady;

        public MockRequestFactoryBuilder SetSessionToken(string sessionToken)
        {
            this.sessionToken = sessionToken;
            return this;
        }

        public MockRequestFactoryBuilder SetIsReady(bool isReady = true)
        {
            this.isReady = isReady;
            return this;
        }

        public MockRequestFactoryBuilder SetSendRequestAsyncReturns(string result) => SetSendRequestAsyncReturns(Task.FromResult(result));
        public MockRequestFactoryBuilder SetSendRequestAsyncReturns(Task<string> result) => SetSendRequestAsyncReturns(()=> result);
        public MockRequestFactoryBuilder SetSendRequestAsyncReturns(Func<Task<string>> getResult)
        {
            sendRequestAsyncResult = getResult;
            return this;
        }

        public MockRequestFactoryBuilder OnSendRequestAsyncCalled(RequestException throws)
        {
            sendRequestAsyncThrows = throws;
            return this;
        }

        /// <summary>
        /// Delegate that gets executed when the method
        /// <see cref="IRequestFactory.SendRequestAsync(string, string, string, string, Dictionary{string, string}, string, string)"/>
        /// is called, which returns a value of type <see cref="Task{String}"/>.
        /// </summary>
        public MockRequestFactoryBuilder OnSendRequestAsyncCalled(Func<string, string, string, string, Dictionary<string, string>, string, string, Task<string>> func)
        {
            sendRequestAsyncFunc = func;
            return this;
        }

        /// <summary>
        /// Delegate that gets executed when the method
        /// <see cref="IRequestFactory.SendRequestAsync(string, string, string, string, Dictionary{string, string}, string, string)"/>
        /// is called, which returns a value of type <see cref="Task{String}"/>.
        /// </summary>
        public MockRequestFactoryBuilder SendRequestAsyncReturns(Func<Task<string>> func)
        {
            sendRequestAsyncFunc = (_, _, _, _, _, _, _) => func();
            return this;
        }

        /// <summary>
        /// Delegate that gets executed when the method
        /// <see cref="IRequestFactory.SendRequestAsync(string, string, string, string, Dictionary{string, string}, string, string)"/>
        /// is called, which returns a value of type <see cref="Task{String}"/>.
        /// </summary>
        public MockRequestFactoryBuilder SendRequestAsyncReturns(params Func<Task<string>>[] sequence)
        {
            sendRequestAsyncSequence = sequence;
            return this;
        }

        public Mock<IRequestFactoryInternal> Build()
        {
            if (mock is not null)
            {
                return mock;
            }

            mock = new Mock<IRequestFactoryInternal>();
            SetupSendRequestAsync(mock);
            SetupIsReady(mock);
            SetupThrottle(mock);
            return mock;
        }

        public void Connect()
        {
            SetIsReady();
            RaiseOnWebSocketConnect();
        }

        public void Disconnect()
        {
            SetIsReady(false);
            RaiseOnWebSocketDisconnect();
        }

        public void RaiseOnWebSocketConnect() => Build().Raise(mock => mock.OnWebSocketConnect += null);
        public void RaiseOnWebSocketDisconnect() => Build().Raise(mock => mock.OnWebSocketDisconnect += null);

        private void SetupSendRequestAsync(Mock<IRequestFactoryInternal> mock)
        {
            mock.Setup(GetSendRequestAsyncWithoutPathParamsSetupExpression())
                .Returns((string basePath, string method, string body, Dictionary<string, string> headers, string requestName, string sessionToken)
                    => mock.Object.SendRequestAsync(basePath, "", method, body, headers, requestName, sessionToken));

            if (sendRequestAsyncSequence is not null)
            {
                var sequence = new MockSequence();
                var setup = GetSendRequestAsyncWithPathParamsSetupExpression();
                foreach (var func in sendRequestAsyncSequence)
                {
                    mock.InSequence(sequence).Setup(setup).Returns(func);
                }
            }
            else
            {
                var setup = mock.Setup(GetSendRequestAsyncWithPathParamsSetupExpression());

                setup.Callback((string basePath, string pathParams, string method, string body, Dictionary<string, string> headers, string requestName, string sessionToken) => sendRequestAsyncWasCalledWith = (basePath, pathParams, method, body, headers, requestName, sessionToken));

                if (sendRequestAsyncThrows is not null)
                {
                    setup.Throws(sendRequestAsyncThrows);
                }

                if (sendRequestAsyncFunc is not null)
                {
                    setup.Callback(sendRequestAsyncFunc);
                }
                else if (sendRequestAsyncResult is not null)
                {
                    setup.Returns(sendRequestAsyncResult);
                }
                else if (sendRequestAsyncThrows is null)
                {
                    var loginResponse = new LoginResponse { SessionToken = sessionToken };
                    setup.Returns(Task.FromResult(CoherenceJson.SerializeObject(loginResponse)));
                }
            }

            Expression<Func<IRequestFactoryInternal, Task<string>>> GetSendRequestAsyncWithoutPathParamsSetupExpression() => requestFactory => requestFactory.SendRequestAsync
            (
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            );

            Expression<Func<IRequestFactoryInternal, Task<string>>> GetSendRequestAsyncWithPathParamsSetupExpression() => requestFactory => requestFactory.SendRequestAsync
            (
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            );
        }

        private void SetupIsReady(Mock<IRequestFactoryInternal> requestFactoryMock) => requestFactoryMock.SetupGet(factory => factory.IsReady).Returns(isReadyGetter);
        private void SetupThrottle(Mock<IRequestFactoryInternal> requestFactoryMock)
        {
            var throttle = new Mock<RequestThrottle>(TimeSpan.Zero, new Mock<IDateTimeProvider>().Object);
            throttle.Setup(x => x.WaitForCooldown(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            requestFactoryMock.SetupGet(factory => factory.Throttle).Returns(throttle.Object);
        }
    }
}

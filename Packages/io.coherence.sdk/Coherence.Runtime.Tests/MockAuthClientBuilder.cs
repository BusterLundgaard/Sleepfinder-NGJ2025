// Copyright (c) coherence ApS.
// See the license file in the package root for more information.
namespace Coherence.Runtime.Tests
{
    using Cloud;
    using Moq;

    /// <summary>
    /// Can be used to <see cref="Build"/> a mock <see cref="IAuthClientInternal"/>
    /// object for use in a test.
    /// </summary>
    internal sealed class MockAuthClientBuilder
    {
        private Mock<IAuthClientInternal> mock;

        private bool isLoggedIn = true;
        private SessionToken sessionToken = new("Expected SessionToken");

        public MockAuthClientBuilder SetIsLoggedIn(bool isLoggedIn = true)
        {
            this.isLoggedIn = isLoggedIn;
            return this;
        }

        public MockAuthClientBuilder SetSessionToken(SessionToken sessionToken)
        {
            this.sessionToken = sessionToken;
            return this;
        }

        public Mock<IAuthClientInternal> Build()
        {
            if (mock is not null)
            {
                return mock;
            }

            mock = new Mock<IAuthClientInternal>(MockBehavior.Strict);
            mock.Setup(x => x.LoggedIn).Returns(isLoggedIn);
            mock.Setup(x => x.SessionToken).Returns(sessionToken);
            return mock;
        }

        public static implicit operator Mock<IAuthClientInternal>(MockAuthClientBuilder builder) => builder.Build();
    }
}

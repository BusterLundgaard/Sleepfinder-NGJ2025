// Copyright (c) coherence ApS.
// See the license file in the package root for more information.

namespace Coherence.Cloud
{
    using Runtime;
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;

    /// <summary>
    /// Specifies a set of methods that can be used to log in to coherence Cloud, properties to determine
    /// if we are currently logged in or not, and events for getting notified about relevant things happening.
    /// </summary>
    public interface IAuthClient
    {
        /// <summary>
        /// Event that is invoked when the client has successfully logged in to coherence Cloud.
        /// </summary>
        event Action<LoginResponse> OnLogin;

        /// <summary>
        /// Event that is invoked when the client has <see cref="Logout">logged out</see> from coherence Cloud.
        /// </summary>
        event Action OnLogout;

        /// <summary>
        /// Event that is invoked when a login request has failed, and when the client's connection to
        /// coherence Cloud has been forcefully closed by the server.
        /// <para>
        /// <see cref="LoginError.Type"/> describes the type of the failure.
        /// </para>
        /// </summary>
        event Action<LoginError> OnError;

        /// <summary>
        /// Is the client currently logged in to coherence Cloud?
        /// </summary>
        bool LoggedIn { get; }

        /// <summary>
        /// Login to coherence Cloud using a guest account, without providing any username or password.
        /// <remarks>
        /// The player is authenticated using a randomly generated username and password.
        /// </remarks>
        /// </summary>
        /// <returns>
        /// An asynchronous operation returning a value representing the result of the operation.
        /// <para>
        /// If the operation was successful, then <see cref="LoginResult.LoggedIn"/> will be <see langword="true"/>,
        /// and <see cref="LoginResult.SessionToken"/> will contain a token which uniquely identifies the logged-in guest.
        /// This token can be stored on the user's device locally, and later used to
        /// <see cref="AuthClient.LoginWithToken">log in</see> to coherence Cloud again as the same guest.
        /// </para>
        /// <para>
        /// If the operation was not successful, then <see cref="LoginResult.LoggedIn"/> will be <see langword="false"/>,
        /// and <see cref="LoginResult.SessionToken"/> will contain the value <see cref="SessionToken.None"/>.
        /// <see cref="LoginResult.Type"/> can be used to determine the reason for the failure.
        /// </para>
        /// </returns>
        Task<LoginResult> LoginAsGuest();

        /// <summary>
        /// Login to coherence Cloud using a specific account with a username and password.
        /// <remarks>
        /// This requires 'Persisted Player Accounts' to be enabled in Project Settings on your
        /// <see href="https://coherence.io/dashboard">coherence Dashboard</see>.
        /// </remarks>
        /// </summary>
        /// <param name="username"> Username for the account which is used to log in. </param>
        /// <param name="password"> Password for the account which is used to log in. </param>
        /// <param name="autoSignup">
        /// Should an account with the provided <paramref name="username"/> be created, if one doesn't already exist?
        /// </param>
        /// <returns>
        /// An asynchronous operation returning a value representing the result of the operation.
        /// <para>
        /// If the operation was successful, then <see cref="LoginResult.LoggedIn"/> will be <see langword="true"/>,
        /// and <see cref="LoginResult.SessionToken"/> will contain a token which uniquely identifies the logged-in user.
        /// This token can be stored on the user's device locally, and later used to
        /// <see cref="AuthClient.LoginWithToken">log in</see> to coherence Cloud again using the same credentials,
        /// without the user needing to provide them again.
        /// </para>
        /// <para>
        /// If the operation was not successful, then <see cref="LoginResult.LoggedIn"/> will be <see langword="false"/>,
        /// and <see cref="LoginResult.SessionToken"/> will contain the value <see cref="SessionToken.None"/>.
        /// <see cref="LoginResult.Type"/> can be used to determine the reason for the failure.
        /// </para>
        /// </returns>
        Task<LoginResult> LoginWithPassword(string username, string password, bool autoSignup);

        /// <summary>
        /// Login to coherence Cloud using a <see cref="SessionToken"/> acquired from the result of a previous login operation.
        /// </summary>
        /// <returns>
        /// An asynchronous operation returning a value representing the result of the operation.
        /// <para>
        /// If the operation was successful, then <see cref="LoginResult.LoggedIn"/> will be <see langword="true"/>,
        /// and <see cref="LoginResult.SessionToken"/> will contain a refreshed token which should replace the old one
        /// that was passed to this method.
        /// </para>
        /// <para>
        /// If the operation was not successful, then <see cref="LoginResult.LoggedIn"/> will be <see langword="false"/>,
        /// and <see cref="LoginResult.SessionToken"/> will contain the value <see cref="SessionToken.None"/>.
        /// </para>
        /// </returns>
        Task<LoginResult> LoginWithToken(SessionToken sessionToken);
        void Logout();
    }
}

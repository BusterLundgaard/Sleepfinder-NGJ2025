// Copyright (c) coherence ApS.
// See the license file in the package root for more information.
namespace Coherence.Cloud
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents a user that has logged in to coherence Cloud.
    /// </summary>
    public record User
    {
        /// <summary>
        /// Represents no user.
        /// </summary>
        public static readonly User None = new(UserGuid.None, "", "", SessionToken.None);

        /// <summary>
        /// Identifier for the user.
        /// <para>
        /// Any empty string for users that logged in as guests.
        /// </para>
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// The unique identifier for the user.
        /// </summary>
        public UserGuid Guid { get; }

        /// <summary>
        /// Username of the user.
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// Token uniquely identifying the logged-in user or guest.
        /// <para>
        /// The token can be stored on the user's device locally, and later used to
        /// <see cref="IAuthClient.LoginWithToken">log in</see> to coherence Cloud again using the same credentials,
        /// without the user needing to provide them again.
        /// </para>
        /// </summary>
        public SessionToken SessionToken { get; }

        internal User(UserGuid guid, string userId, string username, SessionToken sessionToken)
        {
            Guid = guid ?? UserGuid.None;
            UserId = userId ?? "";
            Username = username ?? "";
            SessionToken = sessionToken;
        }

        /// <summary>
        /// Gets <see cref="User"/> that has logged in to coherence Cloud using the <see cref="CloudService"/> on
        /// the master coherence bridge.
        /// </summary>
        /// <returns>
        /// The <see cref="User"/> from the master coherence bridge, if available; otherwise, <see cref="User.None"/>.
        /// <para>
        /// Acquiring the <see cref="User"/> object will not be possible if no <see paramref="cloudService"/> is provided
        /// and no master coherence bridge is present in the active scenes, or if the user is not logged in to coherence Cloud.
        /// </para>
        /// </returns>
        [NotNull]
        internal static User Main => CloudService.FromMasterBridge?.AuthClient.User ?? None;

        /// <summary>
        /// Gets the <see cref="User"/> that has logged in to coherence Cloud using the provided <see cref="CloudService"/>.
        /// </summary>
        /// <param name="cloudService"> The cloud service that the local player used to log in. </param>
        /// <returns>
        /// The <see cref="User"/> that has logged in to coherence Cloud using the provided <see cref="CloudService"/>,
        /// if any; otherwise, <see cref="User.None"/>.
        /// </returns>
        [return: NotNull]
        internal static User Get([DisallowNull] CloudService cloudService) => cloudService.AuthClient.User ?? None;

        public override string ToString()
        {
            if (Guid == UserGuid.None)
            {
                return "User: None";
            }

            if (string.IsNullOrEmpty(UserId))
            {
                return $"Guest: {Guid}";
            }

            return $"User: {Username} ({UserId} / {Guid})";
        }

        public static implicit operator UserGuid(User user) => user.Guid;
    }
}

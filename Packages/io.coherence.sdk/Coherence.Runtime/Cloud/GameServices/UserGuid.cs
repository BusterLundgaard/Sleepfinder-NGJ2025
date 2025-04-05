// Copyright (c) coherence ApS.
// See the license file in the package root for more information.

#if UNITY_5_3_OR_NEWER
#define UNITY
#endif

namespace Coherence.Cloud
{
    using System;

    /// <summary>
    /// Represents a unique identifier for a <see cref="User"/> that has logged in to coherence Cloud.
    /// </summary>
    [Serializable]
    public record UserGuid : IFormattable
    {
        /// <summary>
        /// Represents no user id.
        /// </summary>
        public static readonly UserGuid None = new("");

#if UNITY
        [UnityEngine.SerializeField]
#endif
        internal string value;

        private UserGuid() => value = "";
        internal UserGuid(string value) => this.value = value ?? "";

        public static implicit operator string(UserGuid userId) => userId.value;
        public override string ToString() => value;
        public string ToString(string format, IFormatProvider formatProvider) => value?.ToString(formatProvider) ?? "";

        /// <summary>
        /// Converts the given <see cref="UserGuid"/> into a <see langword="string"/>.
        /// <remarks>
        /// The returned <see langword="string"/> value can be persisted on disk, and later converted back into
        /// a <see cref="UserGuid"/>.
        /// </remarks>>
        /// </summary>
        /// <param name="userId"> The user id to convert into a <see langword="string"/>. </param>
        /// <returns>
        /// The <see langword="string"/> representation of <see paramref="sessionToken"/>.
        /// </returns>
        public static string Serialize(UserGuid userId) => userId.value ?? "";

        /// <summary>
        /// Converts the previously <see cref="Serialize">serialized</see> <see langword="string"/> representation
        /// of a <see cref="UserGuid"/> back into a <see cref="UserGuid"/>.
        /// </summary>
        /// <param name="serializedUserId"> The <see langword="string"/> to convert back into a
        /// <see cref="UserGuid"/>. </param>
        /// <returns>
        /// An object of type <see cref="UserGuid"/>.
        ///<para>
        /// If <see cref="serializedUserId"/> is null or empty, then <see cref="UserGuid.None"/>.
        /// </para>
        /// </returns>
        public static SessionToken Deserialize(string serializedUserId) => new(serializedUserId);
    }
}

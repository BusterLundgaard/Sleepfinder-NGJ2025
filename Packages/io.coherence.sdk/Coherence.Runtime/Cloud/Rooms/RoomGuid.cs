// Copyright (c) coherence ApS.
// See the license file in the package root for more information.

#if UNITY_5_3_OR_NEWER
#define UNITY
#endif

namespace Coherence.Cloud
{
    using System;

    /// <summary>
    /// Represents a globally unique identifier for a room that has been created in coherence Cloud.
    /// </summary>
    /// <seealso cref="RoomLocalId"/>
    [Serializable]
    public struct RoomGuid : IEquatable<RoomGuid>
    {
        /// <summary>
        /// Represents no id.
        /// </summary>
        public static readonly RoomGuid None = new(0);

#if UNITY
        [UnityEngine.SerializeField]
#endif
        internal ulong value;

        internal RoomGuid(ulong value) => this.value = value;

        public bool Equals(RoomGuid other) => value == other.value;
        public override bool Equals(object obj) => obj is RoomGuid other && Equals(other);
        public override int GetHashCode() => value.GetHashCode();
        public static bool operator ==(RoomGuid left, RoomGuid right) => left.Equals(right);
        public static bool operator !=(RoomGuid left, RoomGuid right) => !left.Equals(right);
        public static implicit operator ulong(RoomGuid id) => id.value;
    }
}

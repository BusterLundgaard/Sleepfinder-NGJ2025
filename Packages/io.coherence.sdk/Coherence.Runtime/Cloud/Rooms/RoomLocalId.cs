#if UNITY_5_3_OR_NEWER
#define UNITY
#endif

// Copyright (c) coherence ApS.
// See the license file in the package root for more information.
namespace Coherence.Cloud
{
    using System;

    #pragma warning disable 649
    /// <summary>
    /// Represents a locally unique identifier for a room that has been created in coherence Cloud.
    /// </summary>
    /// <seealso cref="RoomGuid"/>
    [Serializable]
    public struct RoomLocalId : IEquatable<RoomLocalId>
    {
        /// <summary>
        /// Represents no id.
        /// </summary>
        public static readonly RoomLocalId None = new(0);

#if UNITY
        [UnityEngine.SerializeField]
#endif
        internal int value;

        internal RoomLocalId(int value) => this.value = value;

        public bool Equals(RoomLocalId other) => value == other.value;
        public override bool Equals(object obj) => obj is RoomLocalId other && Equals(other);
        public override int GetHashCode() => value;
        public static bool operator ==(RoomLocalId left, RoomLocalId right) => left.Equals(right);
        public static bool operator !=(RoomLocalId left, RoomLocalId right) => !left.Equals(right);
        public static implicit operator int(RoomLocalId id) => id.value;
        public static implicit operator ushort(RoomLocalId id) => (ushort)id.value;
    }
}

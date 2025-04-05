// Copyright (c) coherence ApS.
// See the license file in the package root for more information.
namespace Coherence.Cloud
{
    using System;

    /// <summary>
    /// Represents a room in coherence Cloud.
    /// </summary>
    public sealed class Room : IEquatable<Room>
    {
        /// <summary>
        /// The local identifier of the room.
        /// </summary>
        public RoomLocalId LocalId { get; }

        /// <summary>
        /// The globally unique identifier of the room.
        /// </summary>
        public RoomGuid Guid { get; }

        /// <summary>
        /// The name of the room.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The user that created the room.
        /// </summary>
        public User CreatedBy { get; }

        /// <summary>
        /// The maximum number of players that can be connected to the room.
        /// </summary>
        public int MaxPlayers { get; }

        /// <summary>
        /// The number of players currently connected to the room.
        /// </summary>
        public int ConnectedPlayers { get; internal set; }

        internal Room(RoomLocalId localId, RoomGuid guid, string name, User createdBy, int maxPlayers, int connectedPlayers)
        {
            LocalId = localId;
            Guid = guid;
            MaxPlayers = maxPlayers;
            ConnectedPlayers = connectedPlayers;
            Name = name;
            CreatedBy = createdBy;
        }

        public bool Equals(Room other) => other is not null && Guid.Equals(other.Guid);
        public override bool Equals(object obj) => Equals(obj as Room);
        public override int GetHashCode() => Guid.GetHashCode();
        public static bool operator ==(Room left, Room right) => Equals(left, right);
        public static bool operator !=(Room left, Room right) => !Equals(left, right);
    }
}

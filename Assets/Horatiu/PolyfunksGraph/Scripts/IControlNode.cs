namespace Polyfunks
{
    using System.Collections;
    using UnityEngine;

    public interface IControlNode
    {
        bool Exposed { get; }
        Vector2 Position { get; }
    }
}
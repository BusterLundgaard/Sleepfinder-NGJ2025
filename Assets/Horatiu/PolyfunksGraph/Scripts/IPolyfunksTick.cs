namespace Polyfunks
{
    using System.Collections;
    using UnityEngine;
    using XNode;

    public interface IPolyfunksTick
    {
        void Tick(double deltaTime);
    }

    public interface IPolyfunksValidate
    {
        void OnValidate();
    }

    public interface IPolyfunksGate
    {
        void OnGate(NodePort fromPort, NodePort toPort);

    }

    public interface IPolyfunksParameter
    {
        // sets value on the parameter with chosen name. this way actually the parameter nodes can have multiple parameters
        void SetParameterFromScript(string name, double value, bool instant);

    }
}
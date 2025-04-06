namespace Polyfunks
{
    using System.Collections;
    using UnityEngine;

    public class PolyfunksControl : MonoBehaviour
    {
        public IControlNode nodeRef;

        public enum ControlType
        {
            Float,
        }

        public ControlType type;

        // there needs to be a control script / different UI for each different polyfunks node. float nodes can have a knob, but math nodes might need some toggle and so on. 
        // need better architecture for implementing the UI of each node type as needed.

    }
}
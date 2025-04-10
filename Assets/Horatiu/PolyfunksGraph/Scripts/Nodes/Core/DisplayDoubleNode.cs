﻿namespace Polyfunks
{
    using XNode;

    public class DisplayDoubleNode : XNode.Node
    {

        /// <summary> 
        /// Create an input port that only allows a single connection.
        /// The backing value is not important, as we are only interested in the input value.
        /// We are also acceptable of all input types, so any type will do, as long as it is serializable. 
        /// </summary>
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public double input;

        public double GetEditorValue()
        {
            return GetInputValueDouble("input");
        }

        public override double GetValueDouble(NodePort port)
        {
            return GetInputValueDouble("input");
        }

    }
}

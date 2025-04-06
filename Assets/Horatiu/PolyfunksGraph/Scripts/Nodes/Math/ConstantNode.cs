namespace Polyfunks
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using XNode;

    public class ConstantNode : Node
    {
        [Input] public double value;

        [Output] public double valueOut;

        private void Reset()
        {
            name = "Constant";
        }

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

        }

        // Return the correct value of an output port when requested
        public override double GetValueDouble(NodePort port)
        {
            if (port.fieldName == nameof(valueOut))
                return GetInputValueDouble(nameof(value), value);

            return 0;
        }

        public override object GetValue(NodePort port)
        {
            return GetValueDouble(port);
        }
    }
}
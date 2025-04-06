namespace Polyfunks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using XNode;

    public class ClampNode : Node
    {
        [Input] public double value;
        [Input] public double min;
        [Input] public double max = 1;

        [Output] public double valueOut;

        private void Reset()
        {
            name = "Clamp";
        }

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

        }

        public override double GetValueDouble(NodePort port)
        {
            var _value = GetInputValueDouble(nameof(this.value), this.value);
            var _min = GetInputValueDouble(nameof(min), min);
            var _max = GetInputValueDouble(nameof(max), max);

            if (port.fieldName == nameof(valueOut))
                return Math.Min(_max, Math.Max(_min, _value));

            return 0;
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return GetValueDouble(port);
        }
    }
}
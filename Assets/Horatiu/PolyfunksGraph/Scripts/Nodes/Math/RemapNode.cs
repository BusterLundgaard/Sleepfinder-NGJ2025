namespace Polyfunks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using XNode;

    public class RemapNode : Node
    {
        [Input] public double valueIn;
        [Input] public double inMin;
        [Input] public double inMax = 1;
        [Input] public double outMin;
        [Input] public double outMax = 1;

        [Output] public double valueOut;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

        }

        public override double GetValueDouble(NodePort port)
        {
            if (port.fieldName == nameof(valueOut))
            {
                var valueIn = GetInputValueDouble(nameof(this.valueIn), this.valueIn);
                var inMin = GetInputValueDouble(nameof(this.inMin), this.inMin);
                var inMax = GetInputValueDouble(nameof(this.inMax), this.inMax);
                var outMin = GetInputValueDouble(nameof(this.outMin), this.outMin);
                var outMax = GetInputValueDouble(nameof(this.outMax), this.outMax);

                var param = MathUtils.InverseLerp(inMin, inMax, valueIn);
                var valueOut = MathUtils.Lerp(outMin, outMax, param);

                return valueOut;

            }
            return 0;
        }

        public override object GetValue(NodePort port)
        {
            return GetValueDouble(port);
        }
    }
}
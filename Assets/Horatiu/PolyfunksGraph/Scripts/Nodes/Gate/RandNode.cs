namespace Polyfunks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using XNode;

    public class RandNode : Node, IPolyfunksGate
    {
        [Input] public Gate gate;
        [Input] public double min;
        [Input] public double max = 1;

        private double r;
        private System.Random rand;

        [Output] public double tOut;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();
            r = 0;
            rand = new System.Random();

        }

        public override double GetValueDouble(NodePort port)
        {
            if (port.fieldName == nameof(tOut))
                return r;
            return 0;
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return GetValueDouble(port);
        }

        void IPolyfunksGate.OnGate(NodePort fromPort, NodePort toPort)
        {
            var _min = GetInputValueDouble(nameof(min), min);
            var _max = GetInputValueDouble(nameof(max), max);
            r = MathUtils.LerpUnclamped(_min, _max, rand.NextDouble());

        }

    }
}
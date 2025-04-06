namespace Polyfunks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using XNode;

    public class ProbNode : Node, IPolyfunksGate
    {
        [Input] public Gate realGateIn;

        [Range(0, 1f)]
        public double probability01 = 0.5;

        private System.Random rand;

        [Output] public Gate gateOut;


        // Use this for initialization
        protected override void Init()
        {
            base.Init();
            rand = new System.Random();

        }

        public override double GetValueDouble(NodePort port)
        {
            return 0;
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return GetValueDouble(port);
        }

        void IPolyfunksGate.OnGate(NodePort fromPort, NodePort toPort)
        {
            if (rand.NextDouble() <= probability01)
            {
                Gate.FireGate(this, nameof(gateOut));
            }

        }
    }
}
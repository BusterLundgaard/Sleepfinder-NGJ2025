namespace Polyfunks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using XNode;

    public class AttackNode : Node, IPolyfunksTick, IPolyfunksGate
    {
        [Input] public double gateIn;
        [Input] public Gate realGateIn;

        [Space]
        [Input] public double signalThreshold = 1;

        public double attackTime = 0.02;

        public double decayTime = 0.05;

        private double animTime;

        private double t;

        // make animation curve display attack and decay values readonly

        [Output] public double signalOut;

        public double tLerp = 0.1;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();
            t = 0.0;
            animTime = 0.0;

        }

        public override object GetValue(NodePort port)
        {
            return GetValueDouble(port);
        }

        public override double GetValueDouble(NodePort port)
        {
            if (port.fieldName == nameof(signalOut))
                return t;
            return 0;
        }

        void IPolyfunksTick.Tick(double deltaTime)
        {
            // check the in signal. if it's larger than a threshold, we attack (going to 1). else we decay (going to 0)
            var _in = GetInputValueDouble(nameof(gateIn), gateIn);

            if (_in >= signalThreshold)
            {
                // attack
                animTime = 0.0;
            }
            else
            {
                t = MathUtils.Lerp(t, AttackDecay(animTime), tLerp);
                t = Math.Max(0, t);
                animTime += deltaTime;

            }

            if (double.IsNaN(t))
            { t = 0.0; }
            if (double.IsNaN(gateIn))
            { gateIn = 0.0; }

        }

        private double AttackDecay(double x)
        {
            if (x < attackTime)
                return x / attackTime;
            else
                return 1.0 - (x - attackTime) / decayTime;
        }

        void IPolyfunksGate.OnGate(NodePort fromPort, NodePort toPort)
        {
            animTime = 0.0;
        }
    }
}
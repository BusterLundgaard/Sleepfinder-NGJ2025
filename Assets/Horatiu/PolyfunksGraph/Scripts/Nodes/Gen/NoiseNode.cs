namespace Polyfunks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using XNode;

    public class NoiseNode : Node, IPolyfunksTick
    {
        public const double TAU = Math.PI * 2;

        private double _t;
        private double _oscOut;

        private System.Random _rand;


        [Output] public double tOut;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();
            _t = 0;

            _rand = new System.Random();

        }

        public override double GetValueDouble(NodePort port)
        {
            if (port.fieldName == nameof(tOut))
                return _oscOut;

            return 0;
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return GetValueDouble(port);
        }

        void IPolyfunksTick.Tick(double deltaTime)
        {
            _t += deltaTime * TAU;
            if (double.IsNaN(_t))
                _t = 0;

            _oscOut = (_rand.NextDouble() * 2 - 1);
        }
    }
}
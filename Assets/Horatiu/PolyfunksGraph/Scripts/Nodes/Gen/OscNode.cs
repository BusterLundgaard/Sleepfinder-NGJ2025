namespace Polyfunks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using XNode;

    public class OscNode : Node, IPolyfunksTick
    {
        public const double TAU = Math.PI * 2;

        [Input] public double freq = 220;
        [Input] public double phasePi = 0;

        private double _t;
        private double _oscOut;

        public bool control;

        [Output] public double tOut;


        // Use this for initialization
        protected override void Init()
        {
            base.Init();
            _t = 0;

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
            var _freq = GetInputValueDouble(nameof(freq), this.freq);
            _t += deltaTime * _freq * TAU;
            if (double.IsNaN(_t))
                _t = 0;

            // do the calculation in the tick, so we save performance when a lot of nodes ask for the value.
            var _phase = GetInputValueDouble(nameof(phasePi), this.phasePi) * Math.PI;

            _oscOut = Math.Sin(_t + _phase);

        }
    }
}
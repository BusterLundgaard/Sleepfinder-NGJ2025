namespace Polyfunks
{
    using Polyfunks;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using XNode;

    [Serializable]
    public struct Polyfunks2State
    {
        public double frequency;
        public double spin;
        public double order;
        public double teeth;
        public double amplitude;

    }


    public class PolyfunksOutputNode : Node, IPolyfunksValidate
    {

        [Input] public double frequency = 1;
        [Input] public double spin;
        [Input] public double order = 3;
        [Input] public double teeth;
        [Input] public double amplitude = 0.5;


        public Polyfunks2State GetPolyfunksState()
        {
            var frequencyVal = GetInputValueDouble(nameof(frequency), frequency);
            var spinVal = GetInputValueDouble(nameof(spin), spin);
            var orderVal = GetInputValueDouble(nameof(order), order);
            var teethVal = GetInputValueDouble(nameof(teeth), teeth);
            var ampFinalVal = GetInputValueDouble(nameof(amplitude), amplitude);

            var s = new Polyfunks2State()
            {
                frequency = frequencyVal,
                order = orderVal,
                teeth = teethVal,
                spin = spinVal,
                amplitude = ampFinalVal,
            };
            return s;
        }

        public void OnValidate()
        {
            order = Math.Max(3, order);
        }

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

        }

        // Return the correct value of an output port when requested, but directly as double, avoiding conversion to object, which causes GC.
        public override double GetValueDouble(NodePort port)
        {
            if (port.fieldName == nameof(frequency))
                return GetInputValueDouble(nameof(frequency), frequency);

            if (port.fieldName == nameof(spin))
                return GetInputValueDouble(nameof(spin), spin);

            if (port.fieldName == nameof(order))
                return GetInputValueDouble(nameof(order), order);

            if (port.fieldName == nameof(teeth))
                return GetInputValueDouble(nameof(teeth), teeth);

            if (port.fieldName == nameof(amplitude))
                return GetInputValueDouble(nameof(amplitude), amplitude);

            return 0;
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return GetValueDouble(port);
        }

    }
}
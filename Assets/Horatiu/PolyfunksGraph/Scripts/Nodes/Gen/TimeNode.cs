namespace Polyfunks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using XNode;

    public class TimeNode : Node, IPolyfunksTick, IPolyfunksGate
    {
        [Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public double tIn;
        [Input] public Gate gateReset;

        private double _t;

        public double multiplier = 1;

        [Output] public double tOut;
        [Output] public double tMulOut;


        // Use this for initialization
        protected override void Init()
        {
            base.Init();
            _t = 0;

        }

        public override double GetValueDouble(NodePort port)
        {
            if (port.fieldName == nameof(tOut))
                return _t;

            if (port.fieldName == nameof(tMulOut))
                return _t * multiplier;

            return 0;
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return GetValueDouble(port);
        }

        void IPolyfunksTick.Tick(double deltaTime)
        {
            // inherit value if connected (allows chaining times)
            var inPort = GetInputPort(nameof(tIn));
            if (inPort.Connection != null)
            {
                _t = inPort.GetInputValueDouble();
            }
            else
            {
                // if not connected grow it ourselves
                _t += deltaTime;
            }

            if (double.IsNaN(_t))
                _t = 0;
        }

        void IPolyfunksGate.OnGate(NodePort fromPort, NodePort toPort)
        {
            _t = 0;
        }
    }
}
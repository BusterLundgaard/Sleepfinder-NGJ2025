namespace Polyfunks
{
    using System.Collections;
    using System.Linq;
    using UnityEngine;
    using XNode;

    public class MasterGateNode : Node, IPolyfunksTick, IPolyfunksGate
    {
        private const double inverse60 = 1.0 / 60;


        [Input]
        public double bpm = 120;
        [Input]
        public Gate gateReset;

        private double _time;

        public bool resetAllMasterGates = false;

        [Output]
        public double bpmOut;

        [Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.Strict)]
        public Gate gateOut;


        // Use this for initialization
        protected override void Init()
        {
            base.Init();
            _time = 0;

        }

        public override object GetValue(NodePort port)
        {
            return null;
        }

        void IPolyfunksTick.Tick(double deltaTime)
        {
            var bpm = GetInputValueDouble(nameof(this.bpm), this.bpm);
            _time += deltaTime * bpm * inverse60;

            if (_time >= 1.0)
            {
                _time = 0;

                Gate.FireGate(this, nameof(gateOut));
            }


            // a makeshift button. sets all _time to zero for every node in this graph of type MasterGateNode.
            if (resetAllMasterGates)
            {
                resetAllMasterGates = false;

                var allMasters = this.graph.nodes.Where(n => n is MasterGateNode);
                foreach (var m in allMasters)
                {
                    (m as MasterGateNode)._time = 0;
                }
            }
        }

        void IPolyfunksGate.OnGate(NodePort fromPort, NodePort toPort)
        {
            _time = 0;
        }
    }
}
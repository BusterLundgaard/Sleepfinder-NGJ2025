namespace Polyfunks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using XNode;

    // this is a scriptableobject.
    [CreateAssetMenu]
    public class PolyfunksGraph : NodeGraph
    {
        private PolyfunksOutputNode _outputNode;
        public PolyfunksOutputNode outputNode
        {
            get
            {
                if (_outputNode == null)
                {
                    var pon = this.nodes.FirstOrDefault(n => n is PolyfunksOutputNode);
                    if (pon != null)
                    {
                        _outputNode = pon as PolyfunksOutputNode;
                    }
                }
                return _outputNode;
            }
        }

        public Polyfunks2State defaultState = new Polyfunks2State();

        public IEnumerable<IControlNode> GetControlNodes()
        {
            return nodes.Where(n => (n is IControlNode) && (n as IControlNode).Exposed).Select(n => n as IControlNode);
        }

        public IEnumerable<IPolyfunksParameter> GetParameterNodes()
        {
            return nodes.Where(n => (n is IPolyfunksParameter)).Select(n => n as IPolyfunksParameter);
        }

        public void OnValidate()
        {
            foreach (var n in nodes)
            {
                if (n == null)
                    continue;
                if (n is IPolyfunksValidate)
                {
                    (n as IPolyfunksValidate).OnValidate();
                }
            }
        }

        public Polyfunks2State GetPolyfunksState()
        {
            if (outputNode != null)
            {
                var s = outputNode.GetPolyfunksState();

                // crappy validate.
                s.order = Math.Max(3, s.order);


                return s;
            }

            return defaultState;
        }

        /// <summary>
        /// Called from audio thread when we want to advance the simulation, before sampling the new values
        /// </summary>
        public void AudioThread_Tick(double deltaTime)
        {
            foreach (var n in nodes)
            {
                if (n is IPolyfunksTick)
                {
                    (n as IPolyfunksTick).Tick(deltaTime);
                }
            }
        }
    }
}
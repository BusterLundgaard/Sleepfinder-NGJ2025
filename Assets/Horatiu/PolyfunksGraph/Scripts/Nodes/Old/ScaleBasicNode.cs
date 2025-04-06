namespace Polyfunks.Old
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using ToyBoxHHH;
    using UnityEngine;
    using XNode;

    public class ScaleBasicNode : Node, IPolyfunksTick
    {
        [Input(ShowBackingValue.Never)] 
        public double gate;
        public double Gate => GetInputValueDouble(nameof(gate), gate);

        [Input] public List<double> scale;
        public List<double> Scale => GetInputValue(nameof(scale), scale);

        private int index;
        private bool hasBeenSet;
        private System.Random rand;

        public enum Modes
        {
            Seq,
            Bak,
            Rnd,
            Pin,
        }
        [EnumButtons]
        public Modes mode;

        private bool pingPongDirectionFwd;

        [Output] public double gateOut;

        [Output] public double noteOut;

        [Output] public List<double> scaleOut;


        // Use this for initialization
        protected override void Init()
        {
            base.Init();
            index = 0;
            rand = new System.Random();

        }

        public override double GetValueDouble(NodePort port)
        {
            if (port.fieldName == nameof(gateOut))
                return Gate;

            if (port.fieldName == nameof(noteOut))
            {
                var _scale = Scale;
                return _scale[index % _scale.Count];
            }

            return 0;
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            if (port.fieldName == nameof(noteOut))
            {
                var _scale = Scale;
                return _scale[index % _scale.Count];
            }

            if (port.fieldName == nameof(scaleOut))
            {
                var _scale = Scale;
                return _scale;
            }

            return null;
        }

        void IPolyfunksTick.Tick(double deltaTime)
        {
            var _gate = Gate;
            var _scale = Scale;

            if (_gate >= 1)
            {
                if (!hasBeenSet)
                {
                    hasBeenSet = true;

                    // unity random doesn't work in audio thread
                    //r = (double)UnityEngine.Random.Range((float)min, (float)max);

                    switch (mode)
                    {
                        case Modes.Seq:
                            index = (index + 1) % _scale.Count;
                            break;
                        case Modes.Bak:
                            index = (index - 1 + _scale.Count) % _scale.Count;
                            break;
                        case Modes.Rnd:
                            index = rand.Next(_scale.Count);
                            break;
                        case Modes.Pin:
                            // if going up but we reached the top
                            if (pingPongDirectionFwd && index >= _scale.Count - 1)
                            {
                                // go baack!
                                pingPongDirectionFwd = false;
                            }
                            // if going down and we reached the bottom
                            if (!pingPongDirectionFwd && index <= 0)
                            {
                                pingPongDirectionFwd = true;
                            }
                            index += pingPongDirectionFwd ? 1 : -1;

                            break;
                        default:
                            break;
                    }

                }
            }
            else
            {
                hasBeenSet = false;
            }
        }
    }
}
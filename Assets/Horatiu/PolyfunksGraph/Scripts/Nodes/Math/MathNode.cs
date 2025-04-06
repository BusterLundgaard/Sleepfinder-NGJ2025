namespace Polyfunks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using ToyBoxHHH;
    using UnityEngine;
    using XNode;

    [System.Serializable]
    public class MathNode : Node
    {
        [Input] public double a;
        [Input] public double b;

        [Output] public double result;

        // Will be displayed as an editable field - just like the normal inspector
        [EnumButtons]
        public MathType mathType = MathType.Add;
        public enum MathType
        {
            Add,
            Sub, 
            Mul, 
            Div,
        }

        private void Reset()
        {
            name = "Math";
        }

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

        }

        public override double GetValueDouble(NodePort port)
        {
            // Get new a and b values from input connections. Fallback to field values if input is not connected
            var a = GetInputValueDouble(nameof(MathNode.a), this.a);
            var b = GetInputValueDouble(nameof(MathNode.b), this.b);

            // After you've gotten your input values, you can perform your calculations and return a value
            result = 0;
            if (port.fieldName == nameof(result))
                switch (mathType)
                {
                    case MathType.Add:
                    default:
                    {
                        result = a + b;
                        break;
                    }
                    case MathType.Sub:
                    {
                        result = a - b;
                        break;
                    }
                    case MathType.Mul:
                    {
                        result = a * b;
                        break;
                    }
                    case MathType.Div:
                    {
                        if (b == 0.0)
                            return 0;

                        result = a / b;
                        break;
                    }

                }
            return result;

            //return null;
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return GetValueDouble(port);
        }
    }
}
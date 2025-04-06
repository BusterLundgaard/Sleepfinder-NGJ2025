namespace Polyfunks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using XNode;

    [System.Serializable]
    public class MathSingleNode : Node
    {
        [Input] public double x;

        [Output] public double result;

        public float timesPi = 0;

        // Will be displayed as an editable field - just like the normal inspector
        public MathType mathType;
        public enum MathType
        {
            Sin,
            Cos,
            Tan,
            Sqrt,
            Abs,
            Sign,
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
            var _x = GetInputValueDouble(nameof(x), this.x);

            _x *= Math.PI * timesPi;

            // After you've gotten your input values, you can perform your calculations and return a value
            result = 0;
            if (port.fieldName == nameof(result))
                switch (mathType)
                {
                    case MathType.Sin:
                    default:
                    {
                        result = Math.Sin(_x);
                        break;
                    }
                    case MathType.Cos:
                    {
                        result = Math.Cos(_x);
                        break;
                    }
                    case MathType.Tan:
                    {
                        result = Math.Tan(_x);
                        break;
                    }
                    case MathType.Sqrt:
                    {
                        result = Math.Sqrt(_x);
                        break;
                    }
                    case MathType.Abs:
                    {
                        result = Math.Abs(_x);
                        break;
                    }
                    case MathType.Sign:
                    {
                        result = Math.Sign(_x);
                        break;
                    }
                }
            return result;

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return GetValueDouble(port);
            //return null;
        }
    }
}
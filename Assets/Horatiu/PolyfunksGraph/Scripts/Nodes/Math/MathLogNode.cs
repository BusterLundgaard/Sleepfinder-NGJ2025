namespace Polyfunks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using XNode;

    [System.Serializable]
    public class MathLogNode : Node
    {
        [Input] public double x;

        [Output] public double result;

        public bool absX = false;
        public double multiplyX = 1;
        public double addX = 0;

        public bool inverse = true;

        // Will be displayed as an editable field - just like the normal inspector
        public MathType mathType;
        public enum MathType
        {
            Log2,
            Logn,
            Log10,
        }

        private void Reset()
        {
            name = "Log";
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

            // log is not friends with 0

            _x = _x * multiplyX + addX;

            if (_x < 0.00000001)
                return 0;

            if (absX)
                _x = Math.Abs(_x);

            // After you've gotten your input values, you can perform your calculations and return a value
            result = 0;
            if (port.fieldName == nameof(result))
            {
                switch (mathType)
                {
                case MathType.Log2:
                    {
                        // if 0 causes problems we do like this
                        result = Math.Log(_x, 2);
                        break;
                    }
                case MathType.Logn:
                    {
                        result = Math.Log(_x);
                        break;
                    }
                case MathType.Log10:
                    {
                        result = Math.Log10(_x);
                        break;
                    }
                }

                if (inverse)
                    result = 1 / result;
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
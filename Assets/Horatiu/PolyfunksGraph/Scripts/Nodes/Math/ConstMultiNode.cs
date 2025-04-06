namespace Polyfunks
{
    using System.Collections;
    using System.Collections.Generic;
    using ToyBoxHHH;
    using UnityEngine;
    using XNode;

    public class ConstMultiNode : Node
    {
        [Input] public double value;

        [Input] public double factor1 = 1;
        [EnumButtons]
        public MulDiv factor1Mul = MulDiv.Mul;
        [Input] public double factor2 = 2;
        [EnumButtons]
        public MulDiv factor2Mul = MulDiv.Mul;
        [Input] public double factor3 = 4;
        [EnumButtons]
        public MulDiv factor3Mul = MulDiv.Mul;
        [Input] public double factor4 = 8;
        [EnumButtons]
        public MulDiv factor4Mul = MulDiv.Mul;

        public enum MulDiv
        {
            Mul,
            Div
        }

        [Output] public double valueOut;
        [Output] public double valueOut2;
        [Output] public double valueOut3;
        [Output] public double valueOut4;


        private void Reset()
        {
            name = "Constant";
        }

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

        }

        public override double GetValueDouble(NodePort port)
        {
            var v = GetInputValueDouble(nameof(value), value);

            if (port.fieldName == nameof(valueOut))
            {
                var f1 = GetInputValueDouble(nameof(factor1), factor1);
                var finalV = factor1Mul == MulDiv.Mul ? v * f1 : v / f1;
                return finalV;
            }
            if (port.fieldName == nameof(valueOut2))
            {
                var f2 = GetInputValueDouble(nameof(factor2), factor2);
                var finalV = factor2Mul == MulDiv.Mul ? v * f2 : v / f2;
                return finalV;
            }
            if (port.fieldName == nameof(valueOut3))
            {
                var f3 = GetInputValueDouble(nameof(factor3), factor3);
                var finalV = factor3Mul == MulDiv.Mul ? v * f3 : v / f3;
                return finalV;
            }
            if (port.fieldName == nameof(valueOut4))
            {
                var f4 = GetInputValueDouble(nameof(factor4), factor4);
                var finalV = factor4Mul == MulDiv.Mul ? v * f4 : v / f4;
                return finalV;
            }
            return 0;
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return GetValueDouble(port);
        }
    }

}
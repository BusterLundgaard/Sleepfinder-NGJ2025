using System;
using System.Collections;
using System.Collections.Generic;
using Polyfunks;
using UnityEngine;
using XNode;

[System.Serializable]
public class RefNode : Node, IPolyfunksParameter
{
    //[Input] public double x;


    [Output] public double outputz;

    // reference to external script to pull values from it. let's try!
    public string polyfunksParameter = "polyfunk";
    private double polyfunksValue;
    public void SetParameterFromScript(string name, double value, bool instant = false)
    {
        if (polyfunksParameter == name)
        {
            polyfunksValue = value;
            if (instant)
                outputz = value;
        }
    }

    public double maxDelta = 0.001;

    private void Reset()
    {
        name = "Ref";
    }

    // Use this for initialization
    protected override void Init()
    {
        base.Init();

    }

    public override double GetValueDouble(NodePort port)
    {
        // Get new a and b values from input connections. Fallback to field values if input is not connected
        //var _x = GetInputValueDouble(nameof(x), this.x);
        //outputz = _x;
        
        // perhaps smoothly lerp to it
        outputz = MathUtils.StepTowards(outputz, polyfunksValue, maxDelta);
        //outputz = polyfunksValue;

        return outputz;

    }

    // Return the correct value of an output port when requested
    public override object GetValue(NodePort port)
    {
        return GetValueDouble(port);
        //return null;
    }

}

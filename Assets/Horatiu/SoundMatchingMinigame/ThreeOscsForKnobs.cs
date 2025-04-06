using System.Collections;
using System.Collections.Generic;
using Polyfunks;
using UnityEngine;

public class ThreeOscsForKnobs : MonoBehaviour
{
    public bool fakeValues = false;

    [Range(0, 1f)]
    public float fake1, fake2, fake3;

    public float valuesSmoothing = 0.1f;
    private float smooth1, smooth2, smooth3;

    

    public float GetValue(int index)
    {
        if (!fakeValues)
        {
            if (index == 0)
                return DoctorsNumbers.instance.knob1;
            else if (index == 1)
                return DoctorsNumbers.instance.knob2;
            else
                return DoctorsNumbers.instance.knob3;
        }
        else
        {
            if (index == 0)
                return fake1;
            else if (index == 1)
                return fake2;
            else
                return fake3;
        }

    }

    public PolyfunksReference ref1, ref2, ref3;

    void Update()
    {
        // feed values from doctor (or fake ones) to the refs
        var v1 = GetValue(0);
        var v2 = GetValue(1);
        var v3 = GetValue(2);

        // smooth values!?
        smooth1 = Mathf.Lerp(smooth1, v1, valuesSmoothing);
        smooth2 = Mathf.Lerp(smooth2, v2, valuesSmoothing);
        smooth3 = Mathf.Lerp(smooth3, v3, valuesSmoothing);

        if (ref1 != null)
            ref1.value = smooth1;
        if (ref2 != null)
            ref2.value = smooth2;
        if (ref3 != null)
            ref3.value = smooth3;
    }


}

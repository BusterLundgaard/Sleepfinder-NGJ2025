using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutDoctorNumbersOnPolyfunks : MonoBehaviour
{
    public PolyfunksReference knob1;
    public PolyfunksReference knob2;
    public PolyfunksReference knob3;


    void Update()
    {
        if (knob1 != null)
            knob1.value = DoctorsNumbers.instance.knob1;
        if (knob2 != null)
            knob2.value = DoctorsNumbers.instance.knob2;
        if (knob3 != null)
            knob3.value = DoctorsNumbers.instance.knob3;
    }
}

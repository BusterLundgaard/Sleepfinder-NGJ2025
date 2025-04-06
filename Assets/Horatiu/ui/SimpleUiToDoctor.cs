using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleUiToDoctor : MonoBehaviour
{
    public Slider k1, k2, k3;

    void Update()
    {
        DoctorsNumbers.instance.knob1 = k1.value;
        DoctorsNumbers.instance.knob2 = k2.value;
        DoctorsNumbers.instance.knob3 = k3.value;

    }
}

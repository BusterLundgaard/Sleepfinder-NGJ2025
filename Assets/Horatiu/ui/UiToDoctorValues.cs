using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiToDoctorValues : MonoBehaviour
{
    public Slider slider1, slider2, slider3;

    void Update()
    {
        DoctorsNumbers.instance.knob1 = slider1.value;
        DoctorsNumbers.instance.knob2 = slider2.value;
        DoctorsNumbers.instance.knob3 = slider3.value;
    }

}

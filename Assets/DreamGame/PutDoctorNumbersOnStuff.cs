using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutDoctorNumbersOnStuff : MonoBehaviour
{
    public Material mat;

    void Update()
    {
        mat.SetFloat("_Knob1", DoctorsNumbers.instance.knob1);
        mat.SetFloat("_Knob2", DoctorsNumbers.instance.knob2);
        mat.SetFloat("_Knob3", DoctorsNumbers.instance.knob3);
        
    }
}

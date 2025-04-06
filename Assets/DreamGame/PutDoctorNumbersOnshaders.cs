using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutDoctorNumbersOnshaders : MonoBehaviour
{
    public Material mat1, mat2, mat3;

    void Update()
    {
        mat1.SetFloat("_Frequency", DoctorsNumbers.instance.knob1);
        mat2.SetFloat("_Frequency", DoctorsNumbers.instance.knob2);
        mat3.SetFloat("_Frequency", DoctorsNumbers.instance.knob3);
        
    }
}

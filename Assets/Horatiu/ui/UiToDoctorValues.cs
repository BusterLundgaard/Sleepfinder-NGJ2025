using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiToDoctorValues : MonoBehaviour
{
    struct Coefficients
    {
        public float A;
        public float B;
        public float C;
        public Coefficients(float a, float b, float c)
        {
            A = a; B = b; C = c;
        }
    }

    Coefficients[] arrangements = new Coefficients[]
   {
        new Coefficients(1, 1, -1),
        new Coefficients(1, -1, 1),
        new Coefficients(-1, 1, 1),
        new Coefficients(-1, -1, 1),
        new Coefficients(-1, 1, -1),
        new Coefficients(1, -1, -1)
   };

    Coefficients GenerateCoeffs()
    {
        Coefficients baseCoeffs = arrangements[Random.Range(0, arrangements.Length)];
        float A = baseCoeffs.A * Random.value;
        float B = baseCoeffs.B * Random.value;
        float C = baseCoeffs.C * Mathf.Abs((A * al_o + B * be_o) / om_o);
        return new Coefficients(A, B, C);
    }


    enum Param { None, Alpha, Beta, Omega };

    public Image knob1, knob2, knob3;  //alpha (al), beta (be), omega (om)
    public Image light1, light2, light3;
    public float paramStep = 0.1f;
    Param selectedParam = Param.None;

    float al, be, om;
    float al_o, be_o, om_o;

    Coefficients A1, A2, A3;

    //These are the final computed intensities of audio distortions (a1, a2, a3) and visual distortions respectively (v1, v2, v3)
    //These are computed as dot products between their respective coefficient vectors and (al, be, om)
    // a_i = dot_product(A_i, Vec(al, be, om))
    // v_i = dot_product(V_i, Vec(al, be, om))
    float a1, a2, a3;
    float v1, v2, v3;

    // Generate random required values of alpha, beta, and omega, and generate associated coefficients
    void new_round()
    {
        al = 0.1f + 0.9f * Random.value;
        be = 0.1f + 0.9f * Random.value;
        om = 0.1f + 0.9f * Random.value;

        al_o = 0.1f + 0.9f * Random.value;
        be_o = 0.1f + 0.9f * Random.value;
        om_o = 0.1f + 0.9f * Random.value;

        A1 = GenerateCoeffs();
        A2 = GenerateCoeffs();
        A3 = GenerateCoeffs();
    }

    void compute_amplitudes()
    {
        a1 = A1.A * al + A1.B * be + A1.C * om;
        a2 = A2.A * al + A2.B * be + A2.C * om;
        a3 = A3.A * al + A3.B * be + A3.C * om;
    }


    private void Start()
    {
        new_round();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A)) {
            light1.enabled = true;
            selectedParam = Param.Alpha; 
        } else
        {
            light1.enabled = false;
        }
        if (Input.GetKey(KeyCode.B)) {
            light2.enabled = true;
            selectedParam = Param.Beta;
        } else
        {
            light2.enabled = false;
        }
        if (Input.GetKey(KeyCode.O))
        {
            light3.enabled = true;
            selectedParam = Param.Omega;
        } else
        {
            light3.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) AdjustSelectedParam(-paramStep);
        if (Input.GetKeyDown(KeyCode.RightArrow)) AdjustSelectedParam(paramStep);

        if (Input.GetKeyDown(KeyCode.N)) {
            new_round();
        }

        knob1.transform.eulerAngles = new Vector3(0f, 0f, 90f - 180f * al);
        knob2.transform.eulerAngles = new Vector3(0f, 0f, 90f - 180f * be);
        knob3.transform.eulerAngles = new Vector3(0f, 0f, 90f - 180f * om);

        compute_amplitudes();

        DoctorsNumbers.instance.knob1 = al;
        DoctorsNumbers.instance.knob2 = be;
        DoctorsNumbers.instance.knob3 = om;
    }

    void AdjustSelectedParam(float delta)
    {
        switch (selectedParam)
        {
            case Param.Alpha: al = Mathf.Clamp01(al + delta); break;
            case Param.Beta: be = Mathf.Clamp01(be + delta); break;
            case Param.Omega: om = Mathf.Clamp01(om + delta); break;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoctorOrPatientActivator : MonoBehaviour
{
    [Header("GO.SetActive() onEnable based on doctor/patient/vr status.")]
    public bool showWhenDoctor = false;
    public bool showWhenPatient = false;

    public enum VRStatus
    {
        ShowOnlyInVR,
        ShowOnlyInNonVR,
        ShowAlways
    }
    public VRStatus vrStatus = VRStatus.ShowAlways;


    void OnEnable()
    {
        if (PlayerManager.instance.isDoctor)
        {
            gameObject.SetActive(showWhenDoctor);
        }
        else
        {
            if (!showWhenPatient)
            {
                gameObject.SetActive(false);
            }
            // if showWhenPatient, we also have to filter based on VR status.
            else
            {
                // if patient
                if (vrStatus == VRStatus.ShowAlways)
                {
                    gameObject.SetActive(true);
                }
                else if (vrStatus == VRStatus.ShowOnlyInVR)
                {
                    // show only in VR.
                    gameObject.SetActive(PlayerManager.instance.isVR);
                }
                else if (vrStatus == VRStatus.ShowOnlyInNonVR)
                {
                    // show only in non vr
                    gameObject.SetActive(!PlayerManager.instance.isVR);
                }
            }
        }

    }


}

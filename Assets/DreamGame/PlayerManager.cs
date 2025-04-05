using System;
using System.Collections;
using System.Collections.Generic;
using Coherence.Toolkit;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    // drag n drop
    public CoherenceBridge coherenceBridge;

    // lazy
    private static PlayerManager _instance;
    public static PlayerManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerManager>();
            }
            return _instance;
        }
    }

    public bool forceVrTrue = false;
    // if you're a VR player
    public bool isVR
    {
        get
        {
            if (forceVrTrue)
                return true;

            return UnityEngine.XR.XRSettings.enabled && UnityEngine.XR.XRSettings.isDeviceActive;

        }
    }


    public bool isDoctor { get; private set; }

    public GameObject coherenceWorldUI;
    public GameObject coherenceAutoConnectFirstWorldObj;

    public GameObject selectPlayerTypeUI;
    public Button selectDoctorButton;
    public Button selectPatientButton;

    public GameObject enableOnConnected;


    public Text debugText;

    void OnEnable()
    {
        coherenceBridge.onLiveQuerySynced.AddListener(OnLiveQuerySynced);

        selectDoctorButton.onClick.AddListener(ChooseDoctor);
        selectPatientButton.onClick.AddListener(ChoosePatient);

        if (isVR)
        {
            // auto connect to coherence and turn off their UI
            CoherenceConnect();

        }


        var xrSettingsEnabled = UnityEngine.XR.XRSettings.enabled;
        var isDeviceActive = UnityEngine.XR.XRSettings.isDeviceActive;
        // debug
        debugText.text = (xrSettingsEnabled ? "xr enabled" : "xr disabled")
         + "\n " + (isDeviceActive ? "device active" : "device not active")
         + "\n " + (isVR ? "vr" : "not vr");

    }

    void CoherenceConnect()
    {
        //turn off ui
        coherenceWorldUI.SetActive(false);
        // connect
        coherenceAutoConnectFirstWorldObj.SetActive(true);
    }

    private void OnLiveQuerySynced(CoherenceBridge arg0)
    {
        if (isVR)
        {
            ChoosePatient();
        }
        else
        {
            // show UI to figure out who you wanna be... ?!?!?!
            selectPlayerTypeUI.SetActive(true);
        }
    }

    void ChooseDoctor()
    {

        isDoctor = true;

        // connect!
        Connect();

    }

    void ChoosePatient()
    {
        // turn off the UI
        // try to auto connect etc

        isDoctor = false;

        // connect!
        Connect();
    }

    private void Connect()
    {
        // turn off coherence connection ui, in any case.
        coherenceWorldUI.SetActive(false);
        // turn off selection ui
        selectPlayerTypeUI.SetActive(false);
        // turn on the gameplay stuff.
        enableOnConnected.SetActive(true);
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using Coherence;
using Coherence.Toolkit;
using UnityEngine;

public class DoctorsNumbers : MonoBehaviour
{
    // lazy
    private static DoctorsNumbers _instance;
    public static DoctorsNumbers instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<DoctorsNumbers>();
            return _instance;
        }
    }

    // must be same scene for doctor and patient... otherwise we have to find another way to identify the prefab of the doctor.

    private CoherenceSync _sync;

    public float knob1;
    public float knob2;
    public float knob3;

    private void Awake()
    {
        _sync = GetComponent<CoherenceSync>();
    }


    void OnEnable()
    {
        // live query synced happens after connect and after syncing with the replication server
        _sync.CoherenceBridge.onLiveQuerySynced.AddListener(OnLiveQuerySynced);
    }

    private void OnLiveQuerySynced(CoherenceBridge _)
    {
        // if I'm the doctor player (PC), request authority.

    }

    public void Update()
    {
        // doctor always has authority.
        if (PlayerManager.instance.isDoctor && !_sync.HasStateAuthority)
        {
            _sync.RequestAuthority(AuthorityType.Full);
        }

    }

}


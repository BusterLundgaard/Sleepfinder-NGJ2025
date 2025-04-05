using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Heartbeat : MonoBehaviour
{
    public AudioClip[] audioClips;
    public AudioMixerGroup heartbeatMixerGroup;

    [Header("Toggle heartbeat")]
    public bool beating = true;

    public float bpm = 60f;
    public float bpmMultiplier = 1f;
    private float lastBeatTime = 0f;

    [Header("Index of sound clips to use. -1 means don't play that beat.")]
    public int[] heartBeatsOrdered = new int[] { 0, 1, -1, -1 };
    public float[] heartBeatsVolumes = new float[] { 1, 0.6f, 1, 1 };
    public AnimationCurve pitchBpmMultiplierCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
    private int curIndex = 0;

    void OnEnable()
    {
        if (SoundPooling.instance == null)
        {
            Debug.Log("<color=red>ERROR: Did you add SoundPooling to the scene?</color>");
        }
    }

    void Update()
    {
        if (!beating)
            return;

        bpmMultiplier = Mathf.Max(bpmMultiplier, 0.01f);
        if (lastBeatTime + (60f / (bpm * bpmMultiplier)) < Time.time)
        {
            lastBeatTime = Time.time;
            Beat();
        }

    }

    void Beat()
    {
        var source = SoundPooling.instance.GetFreeSource();
        var index = heartBeatsOrdered[curIndex % heartBeatsOrdered.Length];
        curIndex = (curIndex + 1) % heartBeatsOrdered.Length;
        if (index >= 0)
        {
            source.clip = audioClips[index];
            source.outputAudioMixerGroup = heartbeatMixerGroup;
            source.volume = heartBeatsVolumes[index];
            source.pitch = pitchBpmMultiplierCurve.Evaluate(bpmMultiplier);
            source.Play();
        }
    }

}

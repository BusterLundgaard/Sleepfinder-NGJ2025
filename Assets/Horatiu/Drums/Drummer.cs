using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drummer : MonoBehaviour
{
    // chaos
    public List<AudioClip> kicks = new List<AudioClip>();
    public List<AudioClip> snares = new List<AudioClip>();
    public List<AudioClip> cymbals = new List<AudioClip>();
    public List<AudioClip> other = new List<AudioClip>();

    public float[] weights = new float[] { 1, 0.7f, 0.4f, 0.25f };

    public AnimationCurve frequencyCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public float frequencyMultiplier = 1f;

    private float lastBeatTime = 0f;
    private float currentDelay = 0f;

    void Update()
    {
        if (lastBeatTime + currentDelay < Time.time)
        {
            lastBeatTime = Time.time;
            // curve might make freq v small. 
            currentDelay = frequencyCurve.Evaluate(Random.value) / frequencyMultiplier;
            currentDelay = Mathf.Max(0.01f, currentDelay);
            Beat();
        }

    }

    void Beat()
    {
        var source = SoundPooling.instance.GetFreeSource();
        source.clip = GetRandomClip();
        source.Play();
    }

    private int[] randomChecker = new int[4];

    private AudioClip GetRandomClip()
    {
        var r = Random.value;
        var sum = 0f;
        for (int i = 0; i < weights.Length; i++)
        {
            sum += weights[i];
        }
        r *= sum;

        if (r < weights[0])
        {
            return GetRandomClip(kicks);
        }
        else if (r < weights[0] + weights[1])
        {
            return GetRandomClip(snares);
        }
        else if (r < weights[0] + weights[1] + weights[2])
        {
            return GetRandomClip(cymbals);
        }
        else
        {
            return GetRandomClip(other);
        }

    }

    private AudioClip GetRandomClip(List<AudioClip> clips)
    {
        var r = Random.Range(0, clips.Count);
        return clips[r];
    }
}

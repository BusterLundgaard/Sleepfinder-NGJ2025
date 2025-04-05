using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioLooperKoalaStyle : MonoBehaviour
{
    // to play an audiosource and loop it with crossfade, you actually need a second audiosource that starts while the first is fading
    public AudioSource source;
    [SerializeField]
    private AudioClip myClip;
    // holds the sample data of the audioclip
    private float[] clipData;
    private int clipDataLength;

    private int sampleIndex;

    // shows you the current time in the inspector. also used for the start time.
    [Range(0, 1)]
    public float curTime = 0f;

    // normalized time when the clip should loop.
    [MinMaxSlider(0, 1)]
    public Vector2 loopRange = new Vector2(0, 1);

    // crossfade duration in seconds. should not exceed the loop range divided by 2.
    public float crossfadeDurationInSeconds = 0.1f;
    private int crossfadeDurationInSamples;

    public bool isPlaying;
    public bool isFading;


    [Header("Sample-perfect settings")]
    public bool playOnEnable = true;
    private float saVolume = 0f;
    private float saVolumeAudioThread = 0f;

    [Tooltip("use this to play it using the timeline where you can animate the bool timelinePlay on/off.")]
    // use this to play it using the timeline where you can animate the bool timelinePlay on/off.
    public bool timelinePlay = false;
    private bool oldTimelinePlay;

    public float timelineFadeTime = 1;

    private void Reset()
    {
        source = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        if (source == null)
            source = GetComponent<AudioSource>();

        InitClip();

        if (playOnEnable)
        {
            Play();
        }
    }

    void InitClip()
    {
        clipData = new float[myClip.samples * myClip.channels];
        clipDataLength = clipData.Length;
        myClip.GetData(clipData, 0);

        sampleIndex = (int)(curTime * clipDataLength);
        crossfadeDurationInSeconds = Mathf.Clamp(crossfadeDurationInSeconds, 0, (loopRange.y - loopRange.x) * myClip.length * 0.5f);
        crossfadeDurationInSamples = (int)(crossfadeDurationInSeconds * myClip.frequency * myClip.channels);

    }

    public void Play()
    {
        isPlaying = true;
        saVolume = 1f;
    }

    public void Play(float fadeInDuration)
    {
        isPlaying = true;
        FadeToVolume(1, fadeInDuration);
    }

    public void Pause()
    {
        isPlaying = false;
        saVolume = 0;
    }

    public void Pause(float fadeOutDuration)
    {
        FadeToVolume(0, fadeOutDuration, false);
    }

    public void Stop()
    {
        isPlaying = false;
        saVolume = 0;
        sampleIndex = (int)(loopRange.x * clipDataLength);
    }

    public void Stop(float fadeOutDuration)
    {
        FadeToVolume(0, fadeOutDuration, true);
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        // find the start and end of the clip based on the loop range
        int loopStartSampleIndex = (int)(loopRange.x * clipDataLength);
        int loopEndSampleIndex = (int)(loopRange.y * clipDataLength);

        // lerp the effects volume
        var spInitVolume = saVolumeAudioThread;
        saVolumeAudioThread = Mathf.Lerp(saVolumeAudioThread, saVolume, 0.1f);
        var spFinalVolume = saVolumeAudioThread;

        if (!isPlaying && spInitVolume == 0 && spFinalVolume == 0)
        {
            // set zeros
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = 0;
            }
            return;
        }

        // write the next samples to data buffer
        for (int i = 0; i < data.Length; i++)
        {
            var t = i / (float)data.Length;

            int si = (sampleIndex + i);
            if (si >= loopEndSampleIndex)
            {
                // loop it from the start of the loop region plus the crossfade duration, because we already sampled that part before the loopEnd
                si = (si - (loopEndSampleIndex - loopStartSampleIndex) + crossfadeDurationInSamples + clipDataLength) % clipDataLength;
            }

            // get the sample from the clip
            float sample = clipData[si];
            int siStart = (si - (loopEndSampleIndex - loopStartSampleIndex) + crossfadeDurationInSamples + clipDataLength) % clipDataLength;

            // sample from the start of the loop
            float sample2 = clipData[siStart];
            var crossfadeAmount = (si - (loopEndSampleIndex - crossfadeDurationInSamples))
                                  / (float)crossfadeDurationInSamples;
            // crossfade
            float crossfade = Mathf.Lerp(sample, sample2, crossfadeAmount);

            data[i] = crossfade * Mathf.Lerp(spInitVolume, spFinalVolume, t);

        }

        // increment the sample
        sampleIndex += data.Length;

        // if we've reached the end of the clip, loop it
        // if (sample >= clipDataLength)
        // {
        //     sample -= clipDataLength;
        // }
        if (sampleIndex >= loopEndSampleIndex)
        {
            sampleIndex = sampleIndex - (loopEndSampleIndex - loopStartSampleIndex) + crossfadeDurationInSamples;
        }

    }

    private void Update()
    {
        if (timelinePlay != oldTimelinePlay)
        {
            oldTimelinePlay = timelinePlay;
            if (timelinePlay)
                FadeToVolume(1, timelineFadeTime);
            else
                FadeToVolume(0, timelineFadeTime);

        }


        curTime = sampleIndex / (float)clipDataLength;

        // this is in case we change the crossfadeDurationInSeconds at runtime
        crossfadeDurationInSeconds = Mathf.Clamp(crossfadeDurationInSeconds, 0, (loopRange.y - loopRange.x) * myClip.length);
        crossfadeDurationInSamples = (int)(crossfadeDurationInSeconds * myClip.frequency * myClip.channels);

    }

    public void FadeToVolume(float targetVolume01, float duration, bool stop = false)
    {
        if (targetVolume01 > 0 && !isPlaying)
        {
            Play();
        }

        isFading = true;
        var spInitVolume = saVolume;
        StopAllCoroutines();
        StartCoroutine(pTween.To(duration, t =>
        {
            saVolume = Mathf.Lerp(spInitVolume, targetVolume01, t);
            if (t == 1)
            {
                isFading = false;
                if (saVolume == 0)
                {
                    if (stop) Stop(); else Pause();
                }
            }
        }));
    }

}

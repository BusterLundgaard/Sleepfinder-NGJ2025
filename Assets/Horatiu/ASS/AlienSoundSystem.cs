using System;
using System.Collections;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;
using UnityEngine.Audio;

public class AlienSoundSystem : MonoBehaviour
{
    
    public float fadeTime = 2f;
    public List<AudioLooperKoalaStyle> loopers = new List<AudioLooperKoalaStyle>();


	[DebugButton]
    private void AutoAddLoopers()
    {
        loopers.Clear();
        loopers.AddRange(transform.GetComponentsInChildren<AudioLooperKoalaStyle>());
    }

	public AudioMixer audioMixer;

	private void Update()
    {
        // multiply music volume in the audio mixer by Alien.feedbackMultiplierForLOSE;
        // var musicVol = LinearToDecibel(Alien.feedbackMultiplierForLOSE);
        // audioMixer.SetFloat("MusicVol", musicVol);


		for (int i = 0; i < loopers.Count; i++)
        {
            var looper = loopers[i];
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                if (looper.isPlaying)
                {
                    looper.FadeToVolume(0, fadeTime);
                }
                else
                {
                    looper.FadeToVolume(1, fadeTime);
                }
            }

        }
    }

	private float LinearToDecibel(float linear)
	{
		float dB;

		if (linear != 0)
			dB = 20.0f * Mathf.Log10(linear);
		else
			dB = -144.0f;

		return dB;
	}

    private float DecibelToLinear(float dB)
	{
		float linear = Mathf.Pow(10.0f, dB / 20.0f);

		return linear;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundPooling : MonoBehaviour
{
    private static SoundPooling _instance;
    public static SoundPooling instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SoundPooling>();
            }
            return _instance;
        }
    }

    private Transform _audioSourceParent;
    private List<AudioSource> _sources = new List<AudioSource>();

    public AudioSource GetFreeSource()
    {
        // find existing source
        for (int i = 0; i < _sources.Count; i++)
        {
            var s = _sources[i];
            if (!s.isPlaying)
            {
                return s;
            }    
        }

        // add new source
        {
            var go = new GameObject("AudioSource");
            if (_audioSourceParent == null)
            {
                _audioSourceParent = new GameObject("AudioSourceParent").transform;
                _audioSourceParent.transform.SetParent(transform);
            }
            go.transform.SetParent(_audioSourceParent.transform);

            var s = go.AddComponent<AudioSource>();
            s.loop = false;
            s.playOnAwake = false;
            _sources.Add(s);

            return s;
        }
    }
}

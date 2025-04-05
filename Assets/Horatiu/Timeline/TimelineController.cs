using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using ToyBoxHHH;

public class TimelineController : MonoBehaviour
{
    public PlayableDirector playableDirector;
    public bool pauseOnEnable = true;

    private void OnValidate()
    {
        if (playableDirector == null)
            playableDirector = GetComponent<PlayableDirector>();
    }

    [DebugButton]
    public void Resume()
    {
        playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1f);
    }

    [DebugButton]
    public void Pause()
    {
        playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0f);
    }

    void OnEnable()
    {
        playableDirector.Play();
        if (pauseOnEnable)
        {
            Pause();
        }
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public TimelineDoctor timelineDoctor;

    [Serializable]
    public class LevelData
    {
        public string levelName = "1";

        public float levelTime = 0;

    }

    public List<LevelData> levels = new();
    public int curLevel
    {
        get
        {
            // the current level is the latest level time before the current time of the timeline.
            var curTimelineTime = DoctorsNumbers.instance.timelineTime;
            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i].levelTime < curTimelineTime)
                {
                    return i;
                }
            }
            return levels.Count - 1;
        }
    }

    public void SetLevel(int index)
    {
        SetLevel(levels[index]);
    }
    public void SetLevel(LevelData level)
    {
        // set timeline time to that level's time.
        DoctorsNumbers.instance.timelineTime = level.levelTime;
        timelineDoctor.timelineController.Pause();
    }

    public void PlayUntilLevel(int levelIndex)
    {
        if (levelIndex >= levels.Count)
        {
            SetLevel(0);
            return;
        }
        PlayUntilLevel(levels[levelIndex]);
    }

    public void PlayUntilLevel(LevelData level)
    {
        timelineDoctor.timelineController.Resume();
        StartCoroutine(pTween.WaitCondition(() =>
        {
            return DoctorsNumbers.instance.timelineTime >= level.levelTime;
        }, () =>
        {
            timelineDoctor.timelineController.Pause();
        }));

    }

    [DebugButton]
    public void PlayUntilNextLevel()
    {
        PlayUntilLevel(curLevel + 1);
    }

    void Start()
    {
        SetLevel(levels[0]);
    }



}

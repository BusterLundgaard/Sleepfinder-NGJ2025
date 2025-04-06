using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineDoctor : MonoBehaviour
{
    public TimelineController timelineController;

    public bool autoSetBasedOnDoctorPatient = true;

    // if this is true, it sets the timeline time from doctor. if false, it lets the timeline play and reads the timeline time, for sharing.
    public bool setTimelineTimeFromDoctor = false;
    public float timelineTime;

    void Update()
    {
        if (autoSetBasedOnDoctorPatient)
        {
            if (PlayerManager.instance.isDoctor)
            {
                setTimelineTimeFromDoctor = false;
            }
            else
            {
                setTimelineTimeFromDoctor = true;
            }
        }

        // when set from doctor, always paused.
        if (setTimelineTimeFromDoctor)
        {
            timelineTime = DoctorsNumbers.instance.timelineTime;
            timelineController.Pause();
            timelineController.playableDirector.time = timelineTime;
            timelineController.playableDirector.Evaluate();

        }
        // when we are doctor, we can play or pause. it will just feed the value to the doctor.
        else
        {
            timelineTime = (float)timelineController.playableDirector.time;
            Debug.Log(DateTime.Now + " - Time: " + timelineTime);
            DoctorsNumbers.instance.timelineTime = timelineTime;

        }


    }
}

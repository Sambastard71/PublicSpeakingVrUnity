using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AudioClips
{
    public int[] index;
    public AudioClip[] Lessons;

    public AudioClip GetByIndex(int index)
    {
        int i = 0;
        for (i = 0; i < this.index.Length; i++)
        {
            if (this.index[i] == index)
            {
                break;
            }
        }
        if (i == this.index.Length)
        {
            return Lessons[i-1];
        }
        
        return Lessons[i];
    }

}

public enum Language
{
    Italian,
    English
}

public class AudioClipSetter : MonoBehaviour
{
    public Language Language;

    [SerializeField]
    public AudioClips Lessons;
    public AudioSource LipSyncSource;
    public AudioSource PlayerSource;
    [SerializeField]
    int lastLessonPlayed;
    [SerializeField]
    int actualLesson;

    bool CanRecord;

    public MicRecorder recorder;
    public AudioSource MusicPlayer;
    public int MusicLessonIndex;

    int ActualLesson
    {
        get
        {
            return actualLesson;
        }
        set
        {
            if (!LipSyncSource.isPlaying && !PlayerSource.isPlaying)
            {
                actualLesson = value;
            }
        }
    }

    int command;
    public int Command
    {
        get
        {
            return command;
        }
        set
        {
            if (!LipSyncSource.isPlaying && !PlayerSource.isPlaying)
            {
                command = value;
            }
        }
    }

    bool repeatLesson;

    void Start()
    {
        Debug.Log("Inizio Start");
        CanRecord = true;
        LipSyncSource.Stop();
        PlayerSource.Stop();
        ActualLesson = 0;
        lastLessonPlayed = 0;
        Command = 0;

        Debug.Log("Fine Start");
    }

    void Update()
    {
        if (!LipSyncSource.isPlaying && !PlayerSource.isPlaying)
        {
            if (lastLessonPlayed == 80)
            {
                Debug.Log("Lesson 80");
                Application.Quit();
            }
            //if (actualLesson != ActualLessonEditor && actualLesson != 0)
            //{
            //    ActualLessonEditor = actualLesson;
            //    LoadLesson(ActualLesson);
            //}
            //else if (actualLesson == ActualLessonEditor && actualLesson != 0 && ActualLessonEditor != 0)
            //{
            //    RepeatLesson();
            //}

            if (Command == 0 && ActualLesson == lastLessonPlayed && !recorder.isRecording && CanRecord)
            {
                recorder.StartAndEndRecording();
                CanRecord = false;
            }

            if (Command != 0)
            {
                SetLesson();
            }

            if (ActualLesson != lastLessonPlayed || repeatLesson)
            {
                Debug.Log("[SetAudioClip] Command: " + Command);
                lastLessonPlayed = ActualLesson;
                repeatLesson = false;
                LoadLesson(ActualLesson);
            }

            if (ActualLesson == MusicLessonIndex)
            {
                MusicPlayer.Play();
            }
        }
        //Debug.Log("Fine Update");
    }

    private void SetLesson()
    {
        if (Command == 1 && ActualLesson == 0)
        {
            ActualLesson = 1;
        }
        else if (Command == 2 && ActualLesson > 1)
        {
            repeatLesson = true;
        }
        else if (Command == 3 && ActualLesson > 0 && ActualLesson < Lessons.Lessons.Length - 2)
        {
            ActualLesson++;
        }
        else if (Command == 70)
        {
            LoadLesson(70);
        }
        else if (Command == 80 || ActualLesson == Lessons.Lessons.Length - 2)
        {
            ActualLesson = Command;
        }
        else if (Command == 1 && ActualLesson >= 1)
        {
            LoadLesson(70);
        }
    }

    void LoadLesson(int codeLessonToLoad)
    {
        LipSyncSource.Stop();
        PlayerSource.Stop();        
        AudioClip lessonToLoad = Lessons.GetByIndex(codeLessonToLoad);
        LipSyncSource.clip = lessonToLoad;
        PlayerSource.clip = lessonToLoad;
        Command = 0;
        CanRecord = true;
        LipSyncSource.Play();
        PlayerSource.Play();
    }

}

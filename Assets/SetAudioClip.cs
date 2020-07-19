using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
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
        return Lessons[i];
    }

}

public class SetAudioClip : MonoBehaviour
{
    [SerializeField]
    public AudioClips Lessons;
    public AudioSource Source;
    [SerializeField]
    int lastLessonPlayed;
    [SerializeField]
    int actualLesson;
    int ActualLesson
    {
        get
        {
            return actualLesson;
        }
        set
        {
            if (!Source.isPlaying)
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
            if (!Source.isPlaying)
            {
                command = value;
            }
        }
    }

    bool repeatLesson;

    void Start()
    {
        Debug.Log("Inizio Start");
        Source.Stop();
        ActualLesson = 0;
        lastLessonPlayed = 0;
        Command = 0;

        Debug.Log("Fine Start");
    }

    void Update()
    {
        if (!Source.isPlaying)
        {
            if (ActualLesson == 80)
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
        }
        //Debug.Log("Fine Update");
    }

    
    private void SetLesson()
    {
        if (Command == 1 && ActualLesson == 0)
        {
            ActualLesson = 1;
        }
        else if(Command == 2 && ActualLesson > 1)
        {
            repeatLesson = true;
        }
        else if(Command == 3 && ActualLesson > 0 && ActualLesson < 7)
        {
            ActualLesson++;
        }
        else if (Command == 70)
        {
            LoadLesson(70);
        }
        else if (Command == 80 || ActualLesson == 7)
        {
            ActualLesson = Command;
        }
    }

    void LoadLesson(int codeLessonToLoad)
    {
        Source.Stop();
        AudioClip lessonToLoad = Lessons.GetByIndex(codeLessonToLoad);
        Source.clip = lessonToLoad;
        Command = 0;
        Source.Play();
    }
}

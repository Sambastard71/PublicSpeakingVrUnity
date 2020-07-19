using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingSetter : MonoBehaviour
{
    public SettingSave setting;
    public TcpScript tcp;
    public MicRecorder Recorder;

    public GameObject Female;
    public AudioClipSetter ItalianFemaleSetter;
    public AudioClipSetter EnglishFemaleSetter;

    public GameObject Male;
    public AudioClipSetter ItalianMaleSetter;
    public AudioClipSetter EnglishMaleSetter;

    public bool isSetted;

    //void Start()
    //{
    //    isSetted = false;
    //}

    void Awake()
    {
        //if (!isSetted)
        //{

        setting = GameObject.FindGameObjectWithTag("Setting").GetComponent<SettingSave>();
        if (setting != null)
        {
            if (setting.IsMan)
            {
                Destroy(Female);
                if (setting.IsEnglish)
                {
                    Destroy(ItalianMaleSetter);
                    tcp.setter = EnglishMaleSetter;
                    Recorder.setter = EnglishMaleSetter;
                }
                else
                {
                    Destroy(EnglishMaleSetter);
                    tcp.setter = ItalianMaleSetter;
                    Recorder.setter = ItalianMaleSetter;
                }
            }
            else
            {
                Destroy(Male);
                if (setting.IsEnglish)
                {
                    Destroy(ItalianFemaleSetter);
                    tcp.setter = EnglishFemaleSetter;
                    Recorder.setter = EnglishFemaleSetter;
                }
                else
                {
                    Destroy(EnglishFemaleSetter);
                    tcp.setter = ItalianFemaleSetter;
                    Recorder.setter = ItalianFemaleSetter;
                }
            }

            tcp.Ip = setting.Ip;
            tcp.Port = int.Parse(setting.Port);
        }
        //}
    }
}

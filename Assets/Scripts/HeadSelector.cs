using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using TMPro;

public class HeadSelector : MonoBehaviour
{
    RaycastHit hit;
    
    public SettingSave settings;

    public Transform RaycastTransform;

    float timer;
    public float EndingTimer;

    public SceneLoader loader;

    public GameObject ManPlane;
    public GameObject WomanPlane;
    public GameObject EnglishPlane;
    public GameObject ItalianPlane;
    public GameObject LanguageItaPlane;
    public GameObject LanguageEngPlane;

    public GameObject IpPlane;
    public GameObject PortPlane;
    public GameObject StartPlane;
    public GameObject LogoPlane;


    #region Keyboard
    public GameObject Keyboard;
    public GameObject OnePlane;
    public GameObject TwoPlane;
    public GameObject ThreePlane;
    public GameObject FourPlane;
    public GameObject FivePlane;
    public GameObject SixPlane;
    public GameObject SevenPlane;
    public GameObject EightPlane;
    public GameObject NinePlane;
    public GameObject DotPlane;
    public GameObject ZeroPlane;
    public GameObject CancPlane;
    #endregion


    bool isIp;
    bool isPort;

    public GameObject Pointer;

    void Start()
    {
        timer = 0;
        VideoPlayer vp =  LogoPlane.GetComponent<VideoPlayer>();
        vp.StepForward();

    }

    void Update()
    {
        
        Debug.DrawRay(RaycastTransform.position, RaycastTransform.forward * 100, Color.cyan);
        Pointer.transform.up = -RaycastTransform.forward;

        if (Physics.Raycast(RaycastTransform.position, RaycastTransform.forward, out hit))
        {
            Pointer.transform.position = hit.point + new Vector3(0,0,-0.5f);

            timer += Time.deltaTime;
            //Debug.Log("[Selection.cs] Timer: " + timer);

            if (hit.transform.tag == "Ip")
            {
                timer = 0;
                RaycastIpOrPort(true);
            }

            if (hit.transform.tag == "Port")
            {
                timer = 0;
                RaycastIpOrPort(false);
            }

            if (hit.transform.tag == "One")
            {
                timer = 0;
                DigitSomething("1");
            }

            if (hit.transform.tag == "Two")
            {
                timer = 0;
                DigitSomething("2");
            }

            if (hit.transform.tag == "Three")
            {
                timer = 0;
                DigitSomething("3");
            }

            if (hit.transform.tag == "Four")
            {
                timer = 0;
                DigitSomething("4");
            }

            if (hit.transform.tag == "Five")
            {
                timer = 0;
                DigitSomething("5");
            }

            if (hit.transform.tag == "Six")
            {
                timer = 0;
                DigitSomething("6");
            }

            if (hit.transform.tag == "Seven")
            {
                timer = 0;
                DigitSomething("7");
            }

            if (hit.transform.tag == "Eight")
            {
                timer = 0;
                DigitSomething("8");
            }

            if (hit.transform.tag == "Nine")
            {
                timer = 0;
                DigitSomething("9");
            }

            if (hit.transform.tag == "Dot")
            {
                timer = 0;
                DigitSomething(".");
            }

            if (hit.transform.tag == "Zero")
            {
                timer = 0;
                DigitSomething("0");
            }

            if (hit.transform.tag == "Delete")
            {
                Debug.Log("[Selection.cs] Raycasted Delete");
                timer = 0;
                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || Input.GetKeyDown(KeyCode.LeftShift))
                {
                    if (isIp)
                    {
                        settings.Ip = settings.Ip.Remove(settings.Ip.Length - 1);
                    }
                    else if (isPort)
                    {
                        settings.Port = settings.Port.Remove(settings.Port.Length - 1);
                    }
                }
            }

            if (hit.transform.tag == "Next")
            {
                Debug.Log("[Selection.cs] Raycasted Next");
                timer = 0;
                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || Input.GetKeyDown(KeyCode.LeftShift))
                {
                    Keyboard.SetActive(false);
                    IpPlane.SetActive(false);
                    PortPlane.SetActive(false);
                    hit.transform.gameObject.SetActive(false);

                    StartPlane.SetActive(true);
                }
            }

            if (hit.transform.tag == "Start")
            {
                Debug.Log("[Selection.cs] Raycasted Start");
                if (timer >= EndingTimer)
                {
                    StartPlane.SetActive(false);
                    LogoPlane.SetActive(true);
                    VideoPlayer player = LogoPlane.GetComponent<VideoPlayer>();
                    player.Play();

                    player.loopPointReached += LogoEndReached;
                }
            }

            if (hit.transform.tag == "English")
            {
                Debug.Log("[Selection.cs] Raycasted English");
                if (timer >= EndingTimer)
                {
                    VideoPlayer player = EnglishPlane.GetComponent<VideoPlayer>();
                    player.Play();

                    player.loopPointReached += EnglishEndReached;
                }
            }

            if (hit.transform.tag == "Italian")
            {
                Debug.Log("[Selection.cs] Raycasted Italian");
                if (timer >= EndingTimer)
                {
                    VideoPlayer player = ItalianPlane.GetComponent<VideoPlayer>();
                    player.Play();

                    player.loopPointReached += ItalianEndReached;
                }
            }

            if (hit.transform.tag == "Man")
            {
                Debug.Log("[Selection.cs] Raycasted Man");
                if (timer >= EndingTimer)
                {
                    VideoPlayer player = ManPlane.GetComponent<VideoPlayer>();
                    player.Play();

                    player.loopPointReached += ManEndReached;
                }
            }

            if (hit.transform.tag == "Woman")
            {
                Debug.Log("[Selection.cs] Raycasted Woman");
                if (timer >= EndingTimer)
                {
                    VideoPlayer player = WomanPlane.GetComponent<VideoPlayer>();
                    player.Play();

                    player.loopPointReached += WomanEndReached;
                }
            }

        }
        else
        {
            timer = 0;
            Pointer.transform.position = RaycastTransform.position + (RaycastTransform.forward * 10);
        }
    }

    void LogoEndReached(VideoPlayer vp)
    {
        LogoPlane.SetActive(false);

        EnglishPlane.SetActive(true);
        ItalianPlane.SetActive(true);

    }

    void EnglishEndReached(VideoPlayer vp)
    {
        timer = 0;

        EnglishPlane.SetActive(false);
        ItalianPlane.SetActive(false);

        settings.IsEnglish = true;
        settings.IsItalian = false;

        LanguageEngPlane.SetActive(true);
        ManPlane.SetActive(true);
        WomanPlane.SetActive(true);
    }

    void ItalianEndReached(VideoPlayer vp)
    {
        timer = 0;

        EnglishPlane.SetActive(false);
        ItalianPlane.SetActive(false);

        settings.IsItalian = true;
        settings.IsEnglish = false;

        LanguageItaPlane.SetActive(true);
        ManPlane.SetActive(true);
        WomanPlane.SetActive(true);
    }

    public void WomanEndReached(VideoPlayer vp)
    {
        timer = 0;

        WomanPlane.SetActive(false);
        ManPlane.SetActive(false);
        LanguageEngPlane.SetActive(false);
        LanguageItaPlane.SetActive(false);
        settings.IsWoman = true;
        settings.IsMan = false;

        loader.Load_Scene();
    }

    public void ManEndReached(VideoPlayer vp)
    {
        timer = 0;

        WomanPlane.SetActive(false);
        ManPlane.SetActive(false);
        LanguageEngPlane.SetActive(false);
        LanguageItaPlane.SetActive(false);

        settings.IsMan = true;
        settings.IsWoman = false;

        loader.Load_Scene();
    }

    public void RaycastIpOrPort(bool IpOrPo)
    {
        Debug.Log("[Selection.cs] Raycasted " + (IpOrPo? "Ip" : "Port"));

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            IpPlane.GetComponent<MeshRenderer>().material.color = IpOrPo ? Color.red : Color.white;
            PortPlane.GetComponent<MeshRenderer>().material.color = IpOrPo ? Color.white : Color.red;

            isIp = IpOrPo;
            isPort = !IpOrPo;
        }
    }

    public void DigitSomething(string s)
    {
        Debug.Log("[Selection.cs] Raycasted "+s);

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (isIp)
            {
                settings.Ip += s;
            }
            else if (isPort)
            {
                settings.Port += s;
            }
        }
    }
}

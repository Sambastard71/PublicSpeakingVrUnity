using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingSave : MonoBehaviour
{
    public bool IsMan;
    public bool IsWoman;

    public bool IsEnglish;
    public bool IsItalian;

    public string Ip;
    public string Port;

    public TextMeshPro IpText;
    public TextMeshPro PortText;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        //    IsMan = false;
        //    IsWoman = false;
        //    IsEnglish = false;
        //    IsItalian = true;
    }
    
    void Update()
    {
        if (IpText != null)
        {
            IpText.text = Ip;
        }
        if (PortText != null)
        {
            PortText.text = Port;
        }
    }
}

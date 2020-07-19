using System.Collections;
using System.IO;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tcpScript : MonoBehaviour
{
    public SetAudioClip setter;

    public string Ip;
    public int Port;
    public Text Log;

    TcpClient client;
    NetworkStream stream;
    ASCIIEncoding encoder;

    byte[] byteArraySend;
    byte[] byteArrayRecv;

    bool firstConnection;

    // Start is called before the first frame update
    void Start()
    {
        firstConnection = true;
        encoder = new ASCIIEncoding();

        byteArrayRecv = new byte[100];

        client = new TcpClient();

        Log.text = "";
        TryConnect();

    }

    // Update is called once per frame
    void Update()
    {
        if (client.Connected && firstConnection)
        {
            string s = "Client connected at Ip: " + Ip + " at port " + Port;
            Debug.Log(s);
            Log.text = s;

            firstConnection = false;
            stream = client.GetStream();
            stream.Flush();
        }

        if (client.Connected)
        {
            string s = "";

            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || Input.GetKeyDown(KeyCode.Alpha1))
            {
                s = "inizia la lezione";
                byteArraySend = encoder.GetBytes(s);
                stream.Write(byteArraySend, 0, byteArraySend.Length);
                Log.text = s;
            }
            else if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad) || Input.GetKeyDown(KeyCode.Alpha2))
            {
                s = "ripeti la lezione";
                byteArraySend = encoder.GetBytes(s);
                stream.Write(byteArraySend, 0, byteArraySend.Length);
                Log.text = s;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                s = "Vai Avanti";
                byteArraySend = encoder.GetBytes(s);
                stream.Write(byteArraySend, 0, byteArraySend.Length);
                Log.text = s;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                s = "termina la lezione";
                byteArraySend = encoder.GetBytes(s);
                stream.Write(byteArraySend, 0, byteArraySend.Length);
                Log.text = s;
            }

            if (stream.DataAvailable)
            {
                Read();
            }
        }
    }

    private void Read()
    {
        
        stream.Read(byteArrayRecv, 0, byteArrayRecv.Length);
        int nextLesson = BitConverter.ToInt32(byteArrayRecv, 0);
        string recvString = "Recived: " + nextLesson;
        
        //recvString += Encoding.Default.GetString(byteArrayRecv);
        //for (int i = 0; i < byteArrayRecv.Length; i++)
        //{
        //    recvString += byteArrayRecv[i];
        //}
        setter.Command = nextLesson;
        Debug.Log(recvString);
        Log.text = recvString;
        Array.Clear(byteArrayRecv, 0, byteArrayRecv.Length);
        
    }

    public void TryConnect()
    {
        string s = "[test.cs] trying to connect at " + Ip + " at port " + Port;
        Debug.Log(s);
        Log.text = s;
        client.Connect(Ip, Port);
    }
}

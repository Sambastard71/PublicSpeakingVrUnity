using System.Collections;
using System.IO;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TcpScript : MonoBehaviour
{
    public AudioClipSetter setter;

    public string Ip;
    public int Port;
    //public Text Log;

    TcpClient client;
    NetworkStream stream;

    public byte[] ByteArraySend;
    byte[] byteArrayRecv;

    bool firstConnection;
    bool isConnecting;

    public bool ThereIsDataToSend;

    public int fileindex;

    // Start is called before the first frame update
    void Start()
    {
        fileindex = 0;

        firstConnection = true;
        isConnecting = false;

        ThereIsDataToSend = false;

        byteArrayRecv = new byte[100];

        client = new TcpClient();

        //Log.text = "";
        TryConnect();
    }

    // Update is called once per frame
    void Update()
    {
        if (firstConnection)
        {
            if (!isConnecting)
            {
                TryConnect();
            }

            //Log.text = gamg;
            if (client.Connected)
            {
                string gamg = "Client connected at Ip: " + Ip + " at port " + Port;
                Debug.Log(gamg);
                
                firstConnection = false;
                stream = client.GetStream();
            }
        }

        if (client.Connected)
        {
            if (ThereIsDataToSend)
            {
                ByteArraySend = ReadAndSendFromFile(
                    Application.persistentDataPath + "/AudioTemp/audiotemp" + fileindex + ".wav");
                
                File.Delete(
                    Application.persistentDataPath + "/AudioTemp/audiotemp" + fileindex + ".wav");
                Debug.Log("ByteArraySend length: " + ByteArraySend.Length);
                stream.Write(ByteArraySend, 0, ByteArraySend.Length);
                ThereIsDataToSend = false;
            }


            if (stream.DataAvailable)
            {
                Read();
            }
        }
    }

    private void Read()
    {

        int rlen = stream.Read(byteArrayRecv, 0, byteArrayRecv.Length);

        int command = BitConverter.ToInt32(byteArrayRecv, 0);
        string recvString = "Recived: " + command;

        setter.Command = command;

        Debug.Log(recvString);
        //Log.text = recvString;
        
        Array.Clear(byteArrayRecv, 0, byteArrayRecv.Length);
    }

    public void TryConnect()
    {
        string s = "[test.cs] trying to connect at " + Ip + " at port " + Port;
        Debug.Log(s);
        //Log.text = s;
        if (Ip != "" && Port != 0)
        {
            client.Connect(Ip, Port);
            isConnecting = true;
        }
    }

    public byte[] ReadAndSendFromFile(string path)
    {
        byte[] wav = File.ReadAllBytes(path);
        return wav;
    }
}

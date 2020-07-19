using System.Collections;
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MicRecorder : MonoBehaviour
{
    public AudioClipSetter setter;

    public AudioSource source;
    public bool isRecording;

    List<float> tempRecording;
    float[] recordedClip;

    public TcpScript tcpSender;

    //To make Wav
    FileStream fileStream;
    public Text Log;

    int fileIndex;
    [SerializeField]
    bool isInvoked;

    void Start()
    {
        fileIndex = 0;
        isRecording = false;
        isInvoked = true;

        tempRecording = new List<float>();
        
        if (!Directory.Exists(Application.persistentDataPath + "/AudioTemp"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/AudioTemp");
        }
        
        //source.clip = Microphone.Start(null, true, 1, 44100);
        //Invoke("ResizeRecording", 1);

    }

    void ResizeRecording()
    {
        if (isRecording)
        {
            //add the next second of recorded audio to temp vector
            int length = 44100;
            float[] clipData = new float[length];
            source.clip.GetData(clipData, 0);
            tempRecording.AddRange(clipData);
            Invoke("ResizeRecording", 1);
        }
    }

    void Update()
    {
        //space key and oculus trigger button triggers recording to start...
        if (Input.GetKeyDown("space") || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            StartAndEndRecording();
        }
    }

    public void StartAndEndRecording()
    {
        isRecording = !isRecording;

        Debug.Log("[MicRecorder.cs] " + (isRecording ? "is Recording" : "Off"));

        if (isRecording == false)
        {
            //stop recording, get length, create a new array of samples
            int length = Microphone.GetPosition(null);

            Microphone.End(null);
            float[] clipData = new float[length];
            source.clip.GetData(clipData, 0);

            //create a larger vector that will have enough space to hold our temporary
            //recording, and the last section of the current recording
            float[] fullClip = new float[clipData.Length + tempRecording.Count];
            for (int i = 0; i < fullClip.Length; i++)
            {
                //write data all recorded data to fullCLip vector
                if (i < tempRecording.Count)
                    fullClip[i] = tempRecording[i];
                else
                    fullClip[i] = clipData[i - tempRecording.Count];
            }

            recordedClip = fullClip;
            source.clip = AudioClip.Create("recorded samples", fullClip.Length, 1, 44100, false);
            source.clip.SetData(fullClip, 0);
            source.loop = true;

            CreateWavFile(Application.persistentDataPath + "/AudioTemp/audiotemp" + fileIndex + ".wav");
            tcpSender.fileindex = fileIndex;
            fileIndex++;
            tcpSender.ThereIsDataToSend = true;
        }
        else
        {
            Debug.Log("[MicRecord.cs] Invokeing Record After 4");
            Invoke("StartAndEndRecording", 4);
            //stop audio playback and start new recording...
            source.Stop();
            tempRecording.Clear();
            Microphone.End(null);
            source.clip = Microphone.Start(null, true, 1, 44100);
            Invoke("ResizeRecording", 1);
        }
    }
    void CreateWavFile(string tempPath)
    {
        const int HEADER_SIZE = 44;
        fileStream = new FileStream(tempPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        fileStream.Seek(0, SeekOrigin.Begin);
        byte emptybyte = new byte();

        for (int i = 0; i < HEADER_SIZE; i++)
        {
            fileStream.WriteByte(emptybyte);
        }

        Int16[] intData = new Int16[recordedClip.Length];

        Byte[] bytesData = new Byte[recordedClip.Length * 2]; // bytesData array is twice the size of floatsArray array because a float converted in Int16 is 2 bytes.

        const float rescaleFactor = 32767; //to convert float to Int16

        for (var i = 0; i < recordedClip.Length; i++)
        {
            intData[i] = (short)(recordedClip[i] * rescaleFactor);
            var byteArr = new Byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }
        fileStream.Write(bytesData, 0, bytesData.Length);

        int hz = 44100; //frequency or sampling rate

        fileStream.Seek(0, SeekOrigin.Begin);

        Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF"); //RIFF marker. Marks the file as a riff file. Characters are each 1 byte long. 
        fileStream.Write(riff, 0, 4);

        Byte[] chunkSize = BitConverter.GetBytes(recordedClip.Length - 8); //file-size (equals file-size - 8). Size of the overall file - 8 bytes, in bytes (32-bit integer). Typically, you'd fill this in after creation.
        fileStream.Write(chunkSize, 0, 4);

        Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE"); //File Type Header. For our purposes, it always equals "WAVE".
        fileStream.Write(wave, 0, 4);

        Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt "); //Mark the format section. Format chunk marker. Includes trailing null. 
        fileStream.Write(fmt, 0, 4);

        Byte[] subChunk1 = BitConverter.GetBytes(16); //Length of format data.  Always 16. 
        fileStream.Write(subChunk1, 0, 4);

        UInt16 two = 2;
        UInt16 one = 1;

        Byte[] audioFormat = BitConverter.GetBytes(one); //Type of format (1 is PCM, other number means compression) . 2 byte integer. Wave type PCM
        fileStream.Write(audioFormat, 0, 2);

        Byte[] numChannels = BitConverter.GetBytes(one); //Number of Channels - 2 byte integer
        fileStream.Write(numChannels, 0, 2);

        Byte[] sampleRate = BitConverter.GetBytes(hz); //Sample Rate - 32 byte integer. Sample Rate = Number of Samples per second, or Hertz.
        fileStream.Write(sampleRate, 0, 4);

        Byte[] byteRate = BitConverter.GetBytes(hz * 2 * 1);// sampleRate * bytesPerSample * number of channels, here 16000*2*1.
        fileStream.Write(byteRate, 0, 4);

        UInt16 blockAlign = (ushort)(1 * 2); //channels * bytesPerSample, here 1 * 2  // Bytes Per Sample: 1=8 bit Mono,  2 = 8 bit Stereo or 16 bit Mono, 4 = 16 bit Stereo
        fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

        UInt16 sixteen = 16;
        Byte[] bitsPerSample = BitConverter.GetBytes(sixteen); //Bits per sample (BitsPerSample * Channels) ?? should be 8???
        fileStream.Write(bitsPerSample, 0, 2);

        Byte[] dataString = System.Text.Encoding.UTF8.GetBytes("data"); //"data" chunk header. Marks the beginning of the data section.
        fileStream.Write(dataString, 0, 4);

        Byte[] subChunk2 = BitConverter.GetBytes(fileStream.Length - HEADER_SIZE); //Size of the data section. data-size (equals file-size - 44). or NumSamples * NumChannels * bytesPerSample ??
        fileStream.Write(subChunk2, 0, 4);

        fileStream.Close();
    }
} 

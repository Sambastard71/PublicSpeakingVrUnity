using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;
using Python.Runtime;

public class pytonCaller : MonoBehaviour
{

    public GameObject cube;
    public SetAudioClip AudioClipSetter;
    dynamic demoModulo;

    dynamic output;

    string path_model;
    string path_csv;

    dynamic model;
    dynamic stopWords;
    dynamic recognizer;
    dynamic microphone;

    dynamic sentence;
    int statusSentence;

    int id_section;
    int command;

    void Start()
    {
        statusSentence = 0;
        command = 0;
        id_section = 0;
        Debug.Log("python");
        using (Py.GIL())
        {
            dynamic sys = Py.Import("sys");

            sys.path.append(Application.dataPath + "\\Plugins\\Packages\\python_net\\Lib\\site-packages");
            sys.path.append(Application.dataPath + "\\Public_Speaking");

            demoModulo = Py.Import("Demo_Unity_integration");
            
            path_model = Application.dataPath + "\\Public_Speaking\\models";
            path_csv = Application.dataPath + "\\Public_Speaking\\data\\risposte_utente.csv";

            output = demoModulo.setup(path_model);

            model = output[0];
            stopWords = output[1];
            recognizer = output[2];
            microphone = output[3];

            Debug.Log(output);
            output = null;
        }
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        using (Py.GIL())
        {
            if (!AudioClipSetter.Source.isPlaying)
            {
                output = demoModulo.runListener(microphone, recognizer);

                sentence = output[0];
                statusSentence = (int)(output[1]);

                Debug.Log("Listner Output: ");
                Debug.Log("Sentence: " + sentence);
                Debug.Log("status sentence: " + statusSentence);
            }


            //if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || Input.GetKeyDown(KeyCode.Space))
            //{
            //    cube.transform.localScale *= 0.5f;
            //    statusSentence = 1;
            //    sentence = "inizia la lezione";
            //}

            if (statusSentence == -1)
            {
                return;
            }
            else if (statusSentence == 1)
            {
                output = demoModulo.runSpeaker(model, sentence, id_section, stopWords, path_csv);

                command = (int)(output[0]);
                id_section = (int)(output[1]);

                Debug.Log("Speaker Output: ");
                Debug.Log("Command: " + command);
                Debug.Log("id_section: " + id_section);
            }

            if (command > 0)
            {
                //AudioClipSetter.ActualLesson = id_section;

                command = 0;
                statusSentence = -1;
                sentence = "";
                output = null;
            }

        }
    }
}

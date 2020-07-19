using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    AsyncOperation operation;

    bool isLoading;
    
    void Start()
    {
        isLoading = false;
    }

    public void Load_Scene()
    {
        if (!isLoading)
        {
            Debug.Log("[LoadScene.cs] startedLoading");
            operation = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
            isLoading = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (operation != null)
        {
            Debug.Log("[LoadScene.cs] Loading Progress: " + operation.progress);
            if (operation.isDone)
            {
                operation.allowSceneActivation = true;
            }
        }
    }
}

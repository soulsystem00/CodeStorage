using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerObject : MonoBehaviour
{
    SampleServer sampleServer = new SampleServer();
    void Awake()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            sampleServer.Init();
            DontDestroyOnLoad(this);
        }
    }

    void OnApplicationQuit()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            sampleServer.Shutdown();
        }
    }
}

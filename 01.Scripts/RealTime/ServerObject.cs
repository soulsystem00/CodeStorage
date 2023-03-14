using UnityEngine;

public class ServerObject : MonoBehaviour
{
#if UNITY_EDITOR
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
            sampleServer.Shutdown();
    }
#endif
}

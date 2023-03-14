using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour
{
    AsyncOperation scene;
    void Start()
    {
        scene = SceneManager.LoadSceneAsync(1);
        scene.allowSceneActivation = false;
        Invoke("ChangeScene", 3f);
    }

    void ChangeScene()
    {
        scene.allowSceneActivation = true;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DisconnectPanel : MonoBehaviour
{
    public void OnExitBtnClicked()
    {
        SceneManager.LoadScene(2);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpectatorManager : MonoBehaviour
{
    [SerializeField] Button leftBtn;
    [SerializeField] Button rightBtn;

    public CameraController mainCamera;
    public CameraController inGameMiniMapCamera;
    public MiniMapCamController miniMapCam;

    public void SetCameraObject()
    {
        GameObject go = PlayerListManager.Instance.GetRandomGameObject();
        if (go != null)
        {
            mainCamera.playerObj = go;
            inGameMiniMapCamera.playerObj = go;
            miniMapCam.playerObj = go;
        }
    }
}

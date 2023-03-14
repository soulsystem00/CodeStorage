using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NickNameUI : MonoBehaviour
{
    [SerializeField] Text nickName;
    [SerializeField] Outline outlineX;
    [SerializeField] Outline outlineY;

    void OnEnable()
    {
        StartCoroutine(MakeObjLookCam());
    }

    IEnumerator MakeObjLookCam()
    {
        while (true)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                var qua = Quaternion.LookRotation(cam.transform.position - this.transform.position);
                transform.rotation = Quaternion.Euler(-qua.eulerAngles.x, 0f, 0f);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void SetNickName(string _nickName, string _team, ulong _session_id)
    {
        nickName.text = _nickName;
        if (_team == "Red")
        {
            nickName.color = Color.red;
        }
        else
        {
            nickName.color = Color.blue;
        }

        outlineX.enabled = (_session_id == RealTime.RealTimeNetwork.SessionId);
        outlineY.enabled = (_session_id == RealTime.RealTimeNetwork.SessionId);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using UnityEngine;

public class MiniMapCamController : MonoBehaviour
{
    public GameObject playerObj;
    public float moveOffSet = 0f;
    Vector3 cameraOffset;
    Vector3? curPos = null;

    void Awake()
    {
        cameraOffset = transform.position;
    }

    void OnEnable()
    {
        if (playerObj != null)
        {
            transform.position = new Vector3(playerObj.transform.position.x, cameraOffset.y, 0f);
        }
        curPos = null;
    }

    public void HandleUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            if (curPos != null)
            {
                Vector3 prevPos = curPos.Value;
                curPos = Input.mousePosition;
                if (curPos.Value.x - prevPos.x < 0)
                {
                    transform.position += Vector3.right;
                    if (transform.position.x >= (50f + moveOffSet))
                    {
                        transform.position = new Vector3((50f + moveOffSet), transform.position.y, transform.position.z);
                    }
                }
                else if (curPos.Value.x - prevPos.x > 0)
                {
                    transform.position += Vector3.left;
                    if (transform.position.x <= (-50f + moveOffSet))
                    {
                        transform.position = new Vector3((-50f + moveOffSet), transform.position.y, transform.position.z);
                    }
                }
            }
            else
            {
                curPos = Input.mousePosition;
            }
        }
    }
}

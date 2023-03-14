using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShipControlUI : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] RectTransform controller;
    [SerializeField] GameObject endPoint;

    Vector3 startPos;
    Vector3 curPos;
    Vector3 dirVec;

    Sequence sequence;
    Camera camera;

    void Awake()
    {
        camera = Camera.main;
        InitController();
    }

    void Update()
    {
        // HandleUpdate();
    }

    public void HandleUpdate()
    {
#if UNITY_EDITOR
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                SetStartPos();
                controller.anchoredPosition = startPos;
                controller.gameObject.transform.DOScale(150f, 0.4f);
                KillSequence();
                InitController();
            }

            if (Input.GetMouseButton(0))
            {
                SetCurPos();
                SetDirVec();
                RotateObject(dirVec);
                SetEndPoint();
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (controller.localScale.x > 0f)
                Disappear();
        }
#elif UNITY_ANDROID
if (Input.touchCount > 0 && Input.touchCount < 2)
        {
            if (!IsPointerOverUIObject(Input.mousePosition))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    SetStartPos();
                    controller.anchoredPosition = startPos;
                    controller.gameObject.transform.DOScale(150f, 0.4f);
                    KillSequence();
                    InitController();
                }

                if (Input.GetMouseButton(0))
                {
                    SetCurPos();
                    SetDirVec();
                    RotateObject(dirVec);
                    SetEndPoint();
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (controller.localScale.x > 0f)
                    Disappear();
            }
        }
#endif
    }

    private void KillSequence()
    {
        if (sequence != null)
        {
            if (sequence.active)
            {
                sequence.Kill();
            }
        }
    }

    private void InitController()
    {
        controller.gameObject.transform.localScale = Vector3.zero;
    }

    private void InitCanvas()
    {
        canvas.planeDistance = 5f;
    }

    private void Disappear()
    {
        endPoint.transform.localPosition = new Vector3(-1f, 0f, 0f);
        KillSequence();
        sequence = DOTween.Sequence();
        sequence.Append(controller.gameObject.transform.DOScale(172f, 0.2f));
        sequence.Append(controller.gameObject.transform.DOScale(0f, 0.2f));
        sequence.Play();
    }

    private void SetDirVec()
    {
        dirVec = (curPos - startPos).normalized;
    }

    private void SetEndPoint()
    {
        // Debug.Log((curPos - startPos).magnitude);
        if ((curPos - startPos).magnitude < 68f)
        {
            endPoint.transform.localPosition = Vector3.left;
        }
        else if ((curPos - startPos).magnitude < 360f)
        {
            endPoint.transform.localPosition = new Vector3(-(curPos - startPos).magnitude / 68f, 0f, 0f);
        }

        // endPoint.transform.localPosition = new Vector3(-(curPos - startPos).magnitude * 25f, 0f, 0f);
    }

    private void SetStartPos()
    {
        startPos = Input.mousePosition;
        startPos.z = canvas.planeDistance;
        startPos = camera.ScreenToViewportPoint(startPos);
        startPos = new Vector3(720f * startPos.x, 1280f * startPos.y, 0f);
        // Debug.Log(startPos.x);
    }

    private void SetCurPos()
    {
        curPos = Input.mousePosition;
        curPos.z = canvas.planeDistance;
        curPos = camera.ScreenToViewportPoint(curPos);
        curPos = new Vector3(720f * curPos.x, 1280f * curPos.y, 0f);
        // Debug.Log($"{curPos} {startPos}");
        // startPos = controller.transform.position;
    }

    private void RotateObject(Vector3 _dirVec)
    {
        var deg = Mathf.Atan2(_dirVec.y, _dirVec.x) * Mathf.Rad2Deg;
        controller.gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, deg) * Quaternion.Euler(0f, 0f, 180f);
    }

    private bool IsPointerOverUIObject(Vector2 touchPos)
    {
        PointerEventData eventDataCurrentPosition
            = new PointerEventData(EventSystem.current);

        eventDataCurrentPosition.position = touchPos;

        List<RaycastResult> results = new List<RaycastResult>();


        EventSystem.current
        .RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }
}
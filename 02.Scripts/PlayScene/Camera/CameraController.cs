using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject playerObj;
    Vector3 cameraOffset;

    Camera camera;
    float perspectiveZoomSpeed = 0.05f;
    float orthoZoomSpeed = 0.05f;

    void Awake()
    {
        camera = Camera.main;
        cameraOffset = transform.position;
    }

    public void HandleUpdate()
    {
        if (playerObj != null)
        {
            transform.position = playerObj.transform.position + cameraOffset;
        }
    }

    public void ZoomInOut()
    {
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            if (camera.orthographic)
            {
                camera.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;
                camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, 5f, 20f);
            }
            else
            {
                camera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;
                camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 20f, 60f);
            }
        }
    }
}
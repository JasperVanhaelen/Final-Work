using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchCamera : MonoBehaviour
{
    public float zoomSpeed = 0.1f;
    public float minZoom = 2f;
    public float maxZoom = 10f;
    public float panSpeed = 0.01f;

    private Camera cam;
    private Vector2 lastPanPosition;
    private int panFingerId; // Touch finger ID for panning
    private bool isPanning;

    public static bool IsCameraLocked = false;

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (IsCameraLocked)
        return;

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastPanPosition = touch.position;
                panFingerId = touch.fingerId;
                isPanning = true;
            }
            else if (touch.fingerId == panFingerId && isPanning)
            {
                if (touch.phase == TouchPhase.Moved)
                {
                    Vector2 delta = touch.position - lastPanPosition;
                    PanCamera(delta);
                    lastPanPosition = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    isPanning = false;
                }
            }
        }
        else if (Input.touchCount == 2)
        {
            // Zooming
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            Vector2 touch1Prev = touch1.position - touch1.deltaPosition;
            Vector2 touch2Prev = touch2.position - touch2.deltaPosition;

            float prevMagnitude = (touch1Prev - touch2Prev).magnitude;
            float currentMagnitude = (touch1.position - touch2.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            ZoomCamera(-difference * zoomSpeed);
        }
    }

    void PanCamera(Vector2 delta)
    {
        Vector3 translation = new Vector3(-delta.x * panSpeed, -delta.y * panSpeed, 0);
        cam.transform.Translate(translation);
    }

    void ZoomCamera(float increment)
    {
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + increment, minZoom, maxZoom);
    }
}
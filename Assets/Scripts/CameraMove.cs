using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [HideInInspector] public static CameraMove Instance;

    Vector3 startMousePos, deltaMousePos, startPos;

    float widthSize, heigthSize, nextSize;

    [SerializeField] Camera mainCamera, topCamera;

    [SerializeField] float minZoom;
    [SerializeField] float maxZoom, zoomLerpSpeed;

    [SerializeField] float leftLimit, rightLimit, upperLimit, bottomLimit;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        nextSize = mainCamera.orthographicSize;
    }

    private void Update()
    {
        MouseMove();
        if(!blockedZoom)
            Zoom();
    }

    void MouseMove()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            startMousePos = Input.mousePosition;
            startPos = transform.localPosition;
            widthSize = mainCamera.orthographicSize / Screen.height * Screen.width * 2;
            heigthSize = mainCamera.orthographicSize * 2.3f;
        }
        else if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            deltaMousePos = startMousePos - Input.mousePosition;
            Vector3 pos = new Vector3(startPos.x + deltaMousePos.x / Screen.width * widthSize, 0, startPos.z + deltaMousePos.y / Screen.height * heigthSize);
            pos = new Vector3(Mathf.Clamp(pos.x, leftLimit, rightLimit), 0f, Mathf.Clamp(pos.z, bottomLimit, upperLimit));
            transform.localPosition = pos;
        }
    }

    void Zoom()
    {
        if (Input.mouseScrollDelta != Vector2.zero)
            nextSize = Mathf.Clamp(mainCamera.orthographicSize + Input.mouseScrollDelta.y * -3, minZoom, maxZoom);
        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, nextSize, zoomLerpSpeed);
        topCamera.orthographicSize = mainCamera.orthographicSize;
    }

    float previousZoom;
    bool blockedZoom = false;
    public void BlockedZoom(float zoom)
    {
        previousZoom = mainCamera.orthographicSize;
        mainCamera.orthographicSize = zoom;
        topCamera.orthographicSize = zoom;
        blockedZoom = true;
    }

    public void UnblockedZoom()
    {
        mainCamera.orthographicSize = previousZoom;
        topCamera.orthographicSize = previousZoom;
        blockedZoom = false;
    }

    //limited camera


}

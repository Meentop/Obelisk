using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCanvaz : MonoBehaviour
{
    Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        float scale = 0.01f * mainCamera.orthographicSize / 10;
        transform.localScale = new Vector3(scale, scale, 1);
    }
}

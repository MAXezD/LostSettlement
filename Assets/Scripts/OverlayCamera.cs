using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayCamera : MonoBehaviour
{
    [SerializeField] Camera _MainCamera;
    Camera _overlayCamera;
    void Update()
    {
        _overlayCamera = GetComponent<Camera>();
        _overlayCamera.fieldOfView = _MainCamera.fieldOfView;
    }
}

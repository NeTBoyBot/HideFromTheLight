using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPresenter : MonoBehaviour
{
    private Camera _camera;
    private RenderTexture _renderTexture;
    private MeshRenderer _displayRenderer;

    private void Awake()
    {
        _renderTexture = new RenderTexture(128, 120, 32);
        _camera = GetComponentInChildren<Camera>();
        _camera.targetTexture = _renderTexture;
        _displayRenderer = GetComponentInChildren<MeshRenderer>();
        _displayRenderer.material.SetTexture("_Texture2D",_renderTexture);
    }
}

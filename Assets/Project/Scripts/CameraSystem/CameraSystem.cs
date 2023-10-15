using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] private bool useMouse = false;
    [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;
    [SerializeField] private float moveSpeed = 50;
    [SerializeField] private float rotateSpeed = 150;
    [SerializeField] private float _edgeScrollSize = 20;
    [SerializeField] private float maxFieldOfView = 110;
    [SerializeField] private float minFieldOfView = 30;
    [SerializeField] private float minFollowOffset = 5f;
    [SerializeField] private float maxFollowOffset = 50f;
    private PlayerInput _input;
    private CinemachineTransposer _cinemachineTransposer;
    float targetFieldOfView = 50;
    private Vector3 _followOffset;

    private void Start()
    {
        _input = PlayerInput.Instance;
        _cinemachineTransposer = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        _followOffset = _cinemachineTransposer.m_FollowOffset;
    }

    private void Update()
    {
        HandleCameraMove();
        HandleCameraRotation();
        // HandleCameraZoom_FOV();

        HandleCameraZoomForward();
    }

    void HandleCameraMove()
    {
        Vector3 inputDir = new Vector3(_input.PlayerMovement.x, 0, _input.PlayerMovement.y);
       
        if(useMouse)
        {
            var mousePos2D = _input.MousePos;
            // screen space 原点在左下角
            if (mousePos2D.x < _edgeScrollSize) inputDir.x = -1f;
            if (mousePos2D.y < _edgeScrollSize) inputDir.z = -1f;
            if (mousePos2D.x > Screen.width - _edgeScrollSize) inputDir.x = 1f;
            if (mousePos2D.y > Screen.height - _edgeScrollSize) inputDir.z = 1f;
        }

        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    void HandleCameraRotation()
    {
        if (_input.Qpress)
        {
            transform.rotation = Quaternion.AngleAxis(rotateSpeed * Time.deltaTime, transform.up) * transform.rotation;
        }

        if (_input.Epress)
        {
            transform.rotation =
                Quaternion.AngleAxis(-rotateSpeed * Time.deltaTime, transform.up) * transform.rotation;
        }
    }

    void HandleCameraZoom_FOV()
    {
        if (_input.MouseScroll.y < 0)
        {
            targetFieldOfView += 5;
        }

        if (_input.MouseScroll.y > 0)
        {
            targetFieldOfView -= 5;
        }

        targetFieldOfView = Mathf.Clamp(targetFieldOfView, minFieldOfView, maxFieldOfView);
        _cinemachineVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(_cinemachineVirtualCamera.m_Lens.FieldOfView,
            targetFieldOfView, Time.deltaTime * 10f);
    }

    void HandleCameraZoomForward()
    {
        Vector3 zoomDir = _followOffset.normalized;

        if (_input.MouseScroll.y < 0)
        {
            _followOffset += zoomDir;
        }

        if (_input.MouseScroll.y > 0)
        {
            _followOffset -= zoomDir;
        }

        if (_followOffset.magnitude < minFollowOffset)
        {
            _followOffset = zoomDir * minFollowOffset;
        }

        if (_followOffset.magnitude > maxFollowOffset)
        {
            _followOffset = zoomDir * maxFollowOffset;
        }

        Debug.Log("scroll " + zoomDir);
        float zoomSpeed = 10f;
        _cinemachineTransposer.m_FollowOffset = Vector3.Lerp(_cinemachineTransposer.m_FollowOffset, _followOffset,
            Time.deltaTime * zoomSpeed);
    }
}
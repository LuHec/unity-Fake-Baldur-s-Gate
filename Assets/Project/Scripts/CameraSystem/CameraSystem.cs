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
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private float moveSpeed = 50;
    [SerializeField] private float rotateSpeed = 150;
    [SerializeField] private float edgeScrollSize = 20;
    [SerializeField] private float maxFieldOfView = 110;
    [SerializeField] private float minFieldOfView = 30;
    [SerializeField] private float minFollowOffset = 5f;
    [SerializeField] private float maxFollowOffset = 50f;
    [SerializeField] private float shakeIntensity = 1f;
    [SerializeField] private float shakeTime = 0.2f;

    private CinemachineBasicMultiChannelPerlin cbmp;

    private PlayerInput input;
    private CinemachineTransposer cinemachineTransposer;
    float targetFieldOfView = 50;
    private Vector3 followOffset;

    private void Start()
    {
        input = PlayerInput.Instance;
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        followOffset = cinemachineTransposer.m_FollowOffset;

        AddListener();
    }

    void AddListener()
    {
        MessageCenter.Instance.SubmitPlayerSelectActor(OnPlayerSelect);
    }

    private void Update()
    {
        HandleCameraMove();
        HandleCameraRotation();
        // HandleCameraZoom_FOV();

        HandleCameraZoomForward();
    }

    void OnPlayerSelect(object sender, EventArgsType.PlayerSelectMessage message)
    {
        var actor = ActorsManagerCenter.Instance.GetActorByDynamicId(TurnManager.Instance.GetCurrentPlayerId());

        StopCoroutine(nameof(PlayerSelectCoroutine));
        StartCoroutine(nameof(PlayerSelectCoroutine), new Vector3(actor.transform.position.x, transform.position.y,
            actor.transform.position.z));
    }

    IEnumerator PlayerSelectCoroutine(Vector3 postion)
    {
        while (Vector3.SqrMagnitude(postion - transform.position) > 1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, postion, moveSpeed * Time.deltaTime);

            yield return null;
        }
    }

    void HandleCameraMove()
    {
        Vector3 inputDir = new Vector3(input.PlayerMovement.x, 0, input.PlayerMovement.y);

        if (useMouse)
        {
            var mousePos2D = input.MousePos;
            // screen space 原点在左下角
            if (mousePos2D.x < edgeScrollSize) inputDir.x = -1f;
            if (mousePos2D.y < edgeScrollSize) inputDir.z = -1f;
            if (mousePos2D.x > Screen.width - edgeScrollSize) inputDir.x = 1f;
            if (mousePos2D.y > Screen.height - edgeScrollSize) inputDir.z = 1f;
        }

        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    void HandleCameraRotation()
    {
        if (input.Qpress)
        {
            transform.rotation = Quaternion.AngleAxis(rotateSpeed * Time.deltaTime, transform.up) * transform.rotation;
        }

        if (input.Epress)
        {
            transform.rotation =
                Quaternion.AngleAxis(-rotateSpeed * Time.deltaTime, transform.up) * transform.rotation;
        }
    }

    void HandleCameraZoom_FOV()
    {
        if (input.MouseScroll.y < 0)
        {
            targetFieldOfView += 5;
        }

        if (input.MouseScroll.y > 0)
        {
            targetFieldOfView -= 5;
        }

        targetFieldOfView = Mathf.Clamp(targetFieldOfView, minFieldOfView, maxFieldOfView);
        cinemachineVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(cinemachineVirtualCamera.m_Lens.FieldOfView,
            targetFieldOfView, Time.deltaTime * 10f);
    }

    void HandleCameraZoomForward()
    {
        Vector3 zoomDir = followOffset.normalized;

        if (input.MouseScroll.y < 0)
        {
            followOffset += zoomDir;
        }

        if (input.MouseScroll.y > 0)
        {
            followOffset -= zoomDir;
        }

        if (followOffset.magnitude < minFollowOffset)
        {
            followOffset = zoomDir * minFollowOffset;
        }

        if (followOffset.magnitude > maxFollowOffset)
        {
            followOffset = zoomDir * maxFollowOffset;
        }

        float zoomSpeed = 10f;
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, followOffset,
            Time.deltaTime * zoomSpeed);
    }

    public void SetUseMouse(bool use)
    {
        useMouse = use;
    }

    public void StartShakeCamera()
    {
        StartCoroutine(ShakeCameraCoroutine());
    }

    IEnumerator ShakeCameraCoroutine()
    {
        float timer = 0f;
        cbmp = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cbmp.m_AmplitudeGain = shakeIntensity;

        while (timer < shakeTime)
        {
            timer += Time.fixedDeltaTime;
            yield return null;
        }

        cbmp.m_AmplitudeGain = 0;
    }
}
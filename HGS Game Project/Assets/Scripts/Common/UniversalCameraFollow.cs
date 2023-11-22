using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalCameraFollow : MonoBehaviour
{
    public Transform player;
    public bool isMap2Camera;

    [Header("Common Settings")]
    public float smoothSpeed = 3f;
    public Vector2 offset;
    public bool adaptiveFollowSpeed = true;
    public float followSpeedMultiplier = 2f;

    [Header("Bounds Settings")]
    public bool enableBounds = true;
    private float limitMinX, limitMaxX, limitMinY, limitMaxY;

    private Camera mainCamera;
    private float cameraHalfWidth, cameraHalfHeight;
    private bool isCameraLocked = false;  // �߰�: ī�޶� ���

    private void Awake()
    {
        mainCamera = Camera.main;
        cameraHalfHeight = mainCamera.orthographicSize;
        cameraHalfWidth = cameraHalfHeight * mainCamera.aspect;
    }

    private void FixedUpdate()
    {
        if (!isMap2Camera)
        {
            FollowTarget();
        }
    }

    private void LateUpdate()
    {
        if (isMap2Camera)
        {
            Map2CameraLogic();
        }
    }

    public void SetCameraLimits(float minX, float maxX, float minY, float maxY)
    {
        limitMinX = minX + 3.53f;
        limitMaxX = maxX - 3.53f;
        limitMinY = minY;
        limitMaxY = maxY;
    }

    private void FollowTarget()
    {
        if (player == null) return;

        float currentFollowSpeed = smoothSpeed;

        if (adaptiveFollowSpeed)
        {
            float targetSpeed = player.GetComponent<Rigidbody2D>().velocity.magnitude;
            currentFollowSpeed += targetSpeed * followSpeedMultiplier;
        }

        // ���ϴ� ī�޶� ��ġ ���
        Vector3 desiredPosition = new Vector3(player.position.x + offset.x, player.position.y + offset.y, -10);

        // ī�޶� ��ġ�� ��� ���� �ֵ��� ����
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, limitMinX, limitMaxX);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, limitMinY, limitMaxY);

        // �ε巯�� ī�޶� �̵�
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * currentFollowSpeed);
    }

    private void Map2CameraLogic()
    {
        if (player)
        {
            Vector3 newCameraPosition = transform.position;

            newCameraPosition.x = Mathf.Clamp(player.position.x, limitMinX, limitMaxX);
            newCameraPosition.y = Mathf.Clamp(player.position.y, limitMinY, limitMaxY);

            if (player.position.x < transform.position.x && !isCameraLocked)
            {
                isCameraLocked = true;
            }
            else if (player.position.x > transform.position.x && isCameraLocked)
            {
                isCameraLocked = false;
            }

            if (isCameraLocked)
            {
                newCameraPosition.x = transform.position.x;
            }

            transform.position = newCameraPosition;
        }
    }

    public void RespawnCamera(Vector3 playerPosition)
    {
        Vector3 newCameraPosition = transform.position;

        // �÷��̾��� ������ ��ġ�� �������� ī�޶��� ��ġ�� �缳���մϴ�.
        newCameraPosition.x = Mathf.Clamp(playerPosition.x, limitMinX + cameraHalfWidth, limitMaxX - cameraHalfWidth);
        newCameraPosition.y = Mathf.Clamp(playerPosition.y, limitMinY + cameraHalfHeight, limitMaxY - cameraHalfHeight);

        transform.position = newCameraPosition;
    }
}

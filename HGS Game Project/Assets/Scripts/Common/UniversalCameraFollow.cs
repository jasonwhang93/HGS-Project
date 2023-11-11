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

        // ���� �̵� �Ѱ谪 ����
        if (isMap2Camera)
        {
            // ����Ʈ��2 ���� ������ �Ѱ谪
            limitMinX = 1.36f;
            limitMaxX = 110f;
            limitMinY = 0.9f;
            limitMaxY = 20f;
        }
        else
        {
            // ���κ����� ���� ������ �Ѱ谪
            limitMinX = -15.5f;
            limitMaxX = 20.74f;
            limitMinY = -9.45f;
            limitMaxY = 9.45f;
        }
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

    private void FollowTarget()
    {
        if (player == null) return;

        float currentFollowSpeed = smoothSpeed;

        if (adaptiveFollowSpeed)
        {
            float targetSpeed = player.GetComponent<Rigidbody2D>().velocity.magnitude;
            currentFollowSpeed += targetSpeed * followSpeedMultiplier;
        }

        Vector3 desiredPosition = new Vector3(player.position.x + offset.x, player.position.y + offset.y, -10);

        if (enableBounds)
        {
            desiredPosition = new Vector3(
                Mathf.Clamp(desiredPosition.x, limitMinX + cameraHalfWidth, limitMaxX - cameraHalfWidth),
                Mathf.Clamp(desiredPosition.y, limitMinY + cameraHalfHeight, limitMaxY - cameraHalfHeight),
                -10);
        }

        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * currentFollowSpeed);

        MainVillageTargetClamp();
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

            Map2TargetClamp();
        }
    }

    private void MainVillageTargetClamp()
    {
        // �÷��̾��� �ݶ��̴� ũ�⸦ ���մϴ� (����: BoxCollider2D�� ����Ѵٰ� ����)
        BoxCollider2D playerCollider = player.GetComponent<BoxCollider2D>();
        float playerWidth = playerCollider.size.x * player.localScale.x / 2; // �÷��̾� �ʺ��� ����
        float playerHeight = playerCollider.size.y * player.localScale.y / 2; // �÷��̾� ������ ����

        float targetMaxX = transform.position.x + cameraHalfWidth + playerWidth;
        float targetMinX = transform.position.x - cameraHalfWidth - playerWidth;
        float targetMaxY = transform.position.y + cameraHalfHeight + playerHeight;
        float targetMinY = transform.position.y - cameraHalfHeight - playerHeight;

        player.position = new Vector3(
            Mathf.Clamp(player.position.x, targetMinX, targetMaxX),
            Mathf.Clamp(player.position.y, targetMinY, targetMaxY),
            player.position.z);
    }

    private void Map2TargetClamp()
    {
        // �÷��̾��� �ݶ��̴� ũ�⸦ ���մϴ� (BoxCollider2D�� ����Ѵٰ� ����)
        BoxCollider2D playerCollider = player.GetComponent<BoxCollider2D>();
        float playerWidth = playerCollider.size.x * player.localScale.x / 2; // �÷��̾� �ʺ��� ����
        float playerHeight = playerCollider.size.y * player.localScale.y / 2; // �÷��̾� ������ ����

        // ī�޶� �並 �������� �÷��̾��� �̵� ������ �ִ�, �ּ� X ��ǥ�� ����մϴ�.
        float targetMaxX = transform.position.x + cameraHalfWidth - playerWidth;
        float targetMinX = transform.position.x - cameraHalfWidth + playerWidth;

        // ī�޶� �並 �������� �÷��̾��� �̵� ������ �ִ� Y ��ǥ�� ����մϴ�.
        // �Ʒ��� �������� ��쿡�� Y�� ������ ���� �ʽ��ϴ�.
        float targetMaxY = transform.position.y + cameraHalfHeight - playerHeight;

        // �÷��̾��� ���� Y ��ǥ�� yBottomLimit���� ������ Y ��ǥ ������ �������� �ʽ��ϴ�.
        // �̷��� �ϸ� �÷��̾ �Ʒ��� ������ �� ȭ�� ������ ��� �� �ֽ��ϴ�.
        player.position = new Vector3(
            Mathf.Clamp(player.position.x, targetMinX, targetMaxX),
            player.position.y < limitMinY ? player.position.y : Mathf.Clamp(player.position.y, limitMinY, targetMaxY),
            player.position.z);
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

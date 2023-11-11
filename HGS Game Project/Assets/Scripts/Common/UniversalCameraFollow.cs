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
    private bool isCameraLocked = false;  // 추가: 카메라 잠금

    private void Awake()
    {
        mainCamera = Camera.main;
        cameraHalfHeight = mainCamera.orthographicSize;
        cameraHalfWidth = cameraHalfHeight * mainCamera.aspect;

        // 씬별 이동 한계값 설정
        if (isMap2Camera)
        {
            // 퀘스트맵2 씬에 적합한 한계값
            limitMinX = 1.36f;
            limitMaxX = 110f;
            limitMinY = 0.9f;
            limitMaxY = 20f;
        }
        else
        {
            // 메인빌리지 씬에 적합한 한계값
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
        // 플레이어의 콜라이더 크기를 구합니다 (가정: BoxCollider2D를 사용한다고 가정)
        BoxCollider2D playerCollider = player.GetComponent<BoxCollider2D>();
        float playerWidth = playerCollider.size.x * player.localScale.x / 2; // 플레이어 너비의 절반
        float playerHeight = playerCollider.size.y * player.localScale.y / 2; // 플레이어 높이의 절반

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
        // 플레이어의 콜라이더 크기를 구합니다 (BoxCollider2D를 사용한다고 가정)
        BoxCollider2D playerCollider = player.GetComponent<BoxCollider2D>();
        float playerWidth = playerCollider.size.x * player.localScale.x / 2; // 플레이어 너비의 절반
        float playerHeight = playerCollider.size.y * player.localScale.y / 2; // 플레이어 높이의 절반

        // 카메라 뷰를 기준으로 플레이어의 이동 가능한 최대, 최소 X 좌표를 계산합니다.
        float targetMaxX = transform.position.x + cameraHalfWidth - playerWidth;
        float targetMinX = transform.position.x - cameraHalfWidth + playerWidth;

        // 카메라 뷰를 기준으로 플레이어의 이동 가능한 최대 Y 좌표를 계산합니다.
        // 아래로 떨어지는 경우에는 Y축 제한을 두지 않습니다.
        float targetMaxY = transform.position.y + cameraHalfHeight - playerHeight;

        // 플레이어의 현재 Y 좌표가 yBottomLimit보다 낮으면 Y 좌표 제한을 적용하지 않습니다.
        // 이렇게 하면 플레이어가 아래로 떨어질 때 화면 밖으로 벗어날 수 있습니다.
        player.position = new Vector3(
            Mathf.Clamp(player.position.x, targetMinX, targetMaxX),
            player.position.y < limitMinY ? player.position.y : Mathf.Clamp(player.position.y, limitMinY, targetMaxY),
            player.position.z);
    }

    public void RespawnCamera(Vector3 playerPosition)
    {
        Vector3 newCameraPosition = transform.position;

        // 플레이어의 리스폰 위치를 기준으로 카메라의 위치를 재설정합니다.
        newCameraPosition.x = Mathf.Clamp(playerPosition.x, limitMinX + cameraHalfWidth, limitMaxX - cameraHalfWidth);
        newCameraPosition.y = Mathf.Clamp(playerPosition.y, limitMinY + cameraHalfHeight, limitMaxY - cameraHalfHeight);

        transform.position = newCameraPosition;
    }
}

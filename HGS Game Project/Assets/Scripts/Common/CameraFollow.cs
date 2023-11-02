using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;     // 대상 (플레이어)

    [Header("Follow Settings")]
    public float smoothSpeed = 3f;        // 카메라 이동의 부드러움
    public Vector2 offset = new Vector2(0, 2);     // 카메라와 대상 사이의 거리
    public bool adaptiveFollowSpeed = true;       // 대상의 속도에 따른 카메라 추적 속도 조절 여부
    public float followSpeedMultiplier = 2f;       // 대상의 속도에 따른 카메라 추적 속도 배율

    [Header("Bounds Settings")]
    public bool enableBounds = true;  // 한계 영역 사용 여부
    public float limitMinX, limitMaxX, limitMinY, limitMaxY;

    private float cameraHalfWidth, cameraHalfHeight;

    private void Start()
    {
        adaptiveFollowSpeed = true;
        enableBounds = true;

        cameraHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
        cameraHalfHeight = Camera.main.orthographicSize;

        // 게임 시작 시 카메라 위치를 타겟 위치로 설정
        Vector3 desiredPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, -10);
        if (enableBounds)
        {
            desiredPosition = new Vector3(
                Mathf.Clamp(desiredPosition.x, limitMinX + cameraHalfWidth, limitMaxX - cameraHalfWidth),
                Mathf.Clamp(desiredPosition.y, limitMinY + cameraHalfHeight, limitMaxY - cameraHalfHeight),
                -10);
        }
        transform.position = desiredPosition;
    }

    private void FixedUpdate()
    {
        FollowTarget();
    }

    void FollowTarget()
    {
        if (target == null) return;

        float currentFollowSpeed = smoothSpeed;

        if (adaptiveFollowSpeed)
        {
            float targetSpeed = target.GetComponent<Rigidbody2D>().velocity.magnitude;
            currentFollowSpeed += targetSpeed * followSpeedMultiplier;
        }

        Vector3 desiredPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, -10);

        if (enableBounds)
        {
            desiredPosition = new Vector3(
                Mathf.Clamp(desiredPosition.x, limitMinX + cameraHalfWidth, limitMaxX - cameraHalfWidth),
                Mathf.Clamp(desiredPosition.y, limitMinY + cameraHalfHeight, limitMaxY - cameraHalfHeight),
                -10);
        }

        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * currentFollowSpeed);

        // 한계에 도달했을 때 플레이어의 위치를 제한합니다.
        TargetClamp();
    }

    void TargetClamp()
    {
        // 플레이어의 콜라이더 크기를 구합니다 (가정: BoxCollider2D를 사용한다고 가정)
        BoxCollider2D playerCollider = target.GetComponent<BoxCollider2D>();
        float playerWidth = playerCollider.size.x * target.localScale.x / 2; // 플레이어 너비의 절반
        float playerHeight = playerCollider.size.y * target.localScale.y / 2; // 플레이어 높이의 절반

        float targetMaxX = transform.position.x + cameraHalfWidth + playerWidth;
        float targetMinX = transform.position.x - cameraHalfWidth - playerWidth;
        float targetMaxY = transform.position.y + cameraHalfHeight + playerHeight;
        float targetMinY = transform.position.y - cameraHalfHeight - playerHeight;

        target.position = new Vector3(
            Mathf.Clamp(target.position.x, targetMinX, targetMaxX),
            Mathf.Clamp(target.position.y, targetMinY, targetMaxY),
            target.position.z);
    }
}
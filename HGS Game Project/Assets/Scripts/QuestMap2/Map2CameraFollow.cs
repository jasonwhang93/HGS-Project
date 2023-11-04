using UnityEngine;

public class Map2CameraFollow : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    public float xLeftLimit = 1.36f; // 왼쪽 제한
    public float xRightLimit = 110f; // 오른쪽 제한
    public float yTopLimit = 20f; // 위쪽 제한
    public float yBottomLimit = 0.9f; // 아래쪽 제한
    private bool isCameraLocked = false;

    private Camera mainCamera;
    private float cameraHalfHeight;
    private float cameraHalfWidth;

    private void Awake()
    {
        mainCamera = Camera.main;
        cameraHalfHeight = mainCamera.orthographicSize;
        cameraHalfWidth = cameraHalfHeight * mainCamera.aspect;
    }

    private void LateUpdate() // LateUpdate를 사용하여 플레이어의 움직임이 모두 처리된 후 카메라를 움직입니다.
    {
        if (player)
        {
            Vector3 newCameraPosition = transform.position;

            // 카메라 이동 제한
            newCameraPosition.x = Mathf.Clamp(player.position.x, xLeftLimit, xRightLimit);
            newCameraPosition.y = Mathf.Clamp(player.position.y, yBottomLimit, yTopLimit);

            // 왼쪽 또는 오른쪽으로 움직이려고 할 때 카메라 잠금 상태를 변경
            if (player.position.x < transform.position.x && !isCameraLocked)
            {
                isCameraLocked = true;
            }
            else if (player.position.x > transform.position.x && isCameraLocked)
            {
                isCameraLocked = false;
            }

            // 카메라가 잠겼다면 X 위치를 유지합니다.
            if (isCameraLocked)
            {
                newCameraPosition.x = transform.position.x;
            }

            transform.position = newCameraPosition;

            // 플레이어 위치 제한
            TargetClamp();
        }
    }

    void TargetClamp()
    {
        // 플레이어의 콜라이더 크기를 구합니다 (BoxCollider2D를 사용한다고 가정)
        BoxCollider2D playerCollider = player.GetComponent<BoxCollider2D>();
        float playerWidth = playerCollider.size.x * player.localScale.x / 2; // 플레이어 너비의 절반
        float playerHeight = playerCollider.size.y * player.localScale.y / 2; // 플레이어 높이의 절반

        float targetMaxX = transform.position.x + cameraHalfWidth - playerWidth;
        float targetMinX = transform.position.x - cameraHalfWidth + playerWidth;
        float targetMaxY = transform.position.y + cameraHalfHeight - playerHeight;
        float targetMinY = transform.position.y - cameraHalfHeight + playerHeight;

        player.position = new Vector3(
            Mathf.Clamp(player.position.x, targetMinX, targetMaxX),
            Mathf.Clamp(player.position.y, targetMinY, targetMaxY),
            player.position.z);
    }
}

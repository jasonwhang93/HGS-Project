using UnityEngine;

public class Map2CameraFollow : MonoBehaviour
{
    public Transform player;  // 플레이어의 Transform
    public float xLeftLimit = -10f;  // 왼쪽 제한
    public float xRightLimit = 10f;  // 오른쪽 제한
    public float yTopLimit = 10f;    // 위쪽 제한
    public float yBottomLimit = -10f; // 아래쪽 제한
    private bool isCameraLocked = false;

    private void LateUpdate()  // LateUpdate를 사용하여 플레이어의 움직임이 모두 처리된 후 카메라를 움직입니다.
    {
        if (player)
        {
            Vector3 newCameraPosition = new Vector3(player.position.x, player.position.y, transform.position.z);

            // 왼쪽으로 움직일 때
            if (player.position.x < transform.position.x && !isCameraLocked)
            {
                isCameraLocked = true;
            }

            // 오른쪽으로 움직이려고 할 때
            if (player.position.x > transform.position.x && isCameraLocked)
            {
                isCameraLocked = false;
            }

            if (isCameraLocked)
            {
                newCameraPosition.x = transform.position.x;
            }

            // 카메라 이동 제한
            newCameraPosition.x = Mathf.Clamp(newCameraPosition.x, xLeftLimit, xRightLimit);
            newCameraPosition.y = Mathf.Clamp(newCameraPosition.y, yBottomLimit, yTopLimit);

            transform.position = newCameraPosition;
        }
    }
}

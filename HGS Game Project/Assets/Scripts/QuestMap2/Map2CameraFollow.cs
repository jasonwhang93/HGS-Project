using UnityEngine;

public class Map2CameraFollow : MonoBehaviour
{
    public Transform player; // �÷��̾��� Transform
    public float xLeftLimit = 1.36f; // ���� ����
    public float xRightLimit = 110f; // ������ ����
    public float yTopLimit = 20f; // ���� ����
    public float yBottomLimit = 0.9f; // �Ʒ��� ����
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

    private void LateUpdate() // LateUpdate�� ����Ͽ� �÷��̾��� �������� ��� ó���� �� ī�޶� �����Դϴ�.
    {
        if (player)
        {
            Vector3 newCameraPosition = transform.position;

            // ī�޶� �̵� ����
            newCameraPosition.x = Mathf.Clamp(player.position.x, xLeftLimit, xRightLimit);
            newCameraPosition.y = Mathf.Clamp(player.position.y, yBottomLimit, yTopLimit);

            // ���� �Ǵ� ���������� �����̷��� �� �� ī�޶� ��� ���¸� ����
            if (player.position.x < transform.position.x && !isCameraLocked)
            {
                isCameraLocked = true;
            }
            else if (player.position.x > transform.position.x && isCameraLocked)
            {
                isCameraLocked = false;
            }

            // ī�޶� ���ٸ� X ��ġ�� �����մϴ�.
            if (isCameraLocked)
            {
                newCameraPosition.x = transform.position.x;
            }

            transform.position = newCameraPosition;

            // �÷��̾� ��ġ ����
            TargetClamp();
        }
    }

    void TargetClamp()
    {
        // �÷��̾��� �ݶ��̴� ũ�⸦ ���մϴ� (BoxCollider2D�� ����Ѵٰ� ����)
        BoxCollider2D playerCollider = player.GetComponent<BoxCollider2D>();
        float playerWidth = playerCollider.size.x * player.localScale.x / 2; // �÷��̾� �ʺ��� ����
        float playerHeight = playerCollider.size.y * player.localScale.y / 2; // �÷��̾� ������ ����

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

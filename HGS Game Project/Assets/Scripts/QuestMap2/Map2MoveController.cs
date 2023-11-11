using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map2MoveController : MonoBehaviour
{
    public float moveSpeed = 5.0f;  // 이동 속도
    public float jumpForce = 9.0f;  // 점프 힘
    public GameObject upGround; // UpGround 게임 오브젝트에 대한 참조
    public GameObject cylinderObject; // CylinderObject에 대한 참조 추가


    public Transform[] respawnPoints; // 리스폰 지점들
    private Vector3 lastFallPosition; // 플레이어가 화면 밖으로 벗어날 때의 마지막 위치

    // UI 하트 이미지 참조를 위한 배열
    public GameObject[] hearts;

    public MapController mapController;

    private Collider2D upGroundCollider;
    private Collider2D cylinderObjectCollider; // CylinderObject의 Collider

    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 characterScale;  // 캐릭터의 초기 크기 저장
    private bool isGrounded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (upGround != null)
        {
            upGroundCollider = upGround.GetComponent<Collider2D>();
        }
        if (cylinderObject != null)
        {
            cylinderObjectCollider = cylinderObject.GetComponent<Collider2D>();
        }

        // 캐릭터의 초기 스케일 설정 (오른쪽을 기본 방향으로 가정)
        characterScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        transform.localScale = characterScale; // 오른쪽을 바라보도록 설정

        PlayerData.InitData();
    }

    private void Update()
    {
        Move();
        Jump();
        HandlePlatformEffectorCollision(upGroundCollider); // UpGround 처리
        HandlePlatformEffectorCollision(cylinderObjectCollider); // CylinderObject 처리

        CheckFallOffScreen();

        // Animator 변수 업데이트
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isWalk", rb.velocity.x != 0 && isGrounded);
        animator.SetBool("isJump", !isGrounded);
        animator.SetBool("isLadder", false);
        animator.SetBool("isRope", false);

        if(PlayerData.playerRemainHeart <= 0)
        {
            // 모든 하트가 떨어진 경우, 게임 오버 처리를 할 수 있습니다.
            PlayerData.playerEarnCoin = 0;
            PlayerData.isMap2Cleared = false;
            mapController.isMapClearFailed = true;
        }
    }

    private void Move()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");

        // 캐릭터 방향 변경
        if (horizontalMove > 0) // 오른쪽으로 이동할 때
        {
            transform.localScale = new Vector3(-Mathf.Abs(characterScale.x), characterScale.y, characterScale.z);
        }
        else if (horizontalMove < 0) // 왼쪽으로 이동할 때
        {
            transform.localScale = new Vector3(Mathf.Abs(characterScale.x), characterScale.y, characterScale.z);
        }

        // 캐릭터 이동
        Vector2 moveDirection = new Vector2(horizontalMove * moveSpeed, rb.velocity.y);
        rb.velocity = moveDirection;
    }

    private void Jump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    // 플레이어가 화면 밖으로 벗어나는지 확인하고 마지막 위치를 저장하는 메서드
    private void CheckFallOffScreen()
    {
        Camera mainCamera = Camera.main;
        float lowerCameraBound = mainCamera.transform.position.y - mainCamera.orthographicSize;

        if (transform.position.y < lowerCameraBound)
        {
            lastFallPosition = transform.position; // 마지막 위치 저장
            RespawnAtNearestPoint();
        }
    }

    private void RespawnAtNearestPoint()
    {
        // 저장된 마지막 위치와 가장 가까운 리스폰 지점 찾기
        float nearestRespawnDistance = float.MaxValue;
        Transform nearestRespawnPoint = null;
        foreach (var respawnPoint in respawnPoints)
        {
            float distance = Mathf.Abs(lastFallPosition.x - respawnPoint.position.x);
            if (distance < nearestRespawnDistance)
            {
                nearestRespawnDistance = distance;
                nearestRespawnPoint = respawnPoint;
            }
        }

        // PlayerData의 하트 수를 감소시키고 UI 업데이트
        if (PlayerData.playerRemainHeart > 0)
        {
            PlayerData.playerRemainHeart--;
            UpdateHeartsUI();
        }

        // 가장 가까운 리스폰 지점으로 이동
        transform.position = nearestRespawnPoint.position;
        rb.velocity = Vector2.zero; // 속도를 리셋합니다.

        // 카메라의 위치를 리스폰 지점으로 이동시킵니다.
        //Camera.main.GetComponent<Map2CameraFollow>().RespawnCamera(nearestRespawnPoint.position);
        Camera.main.GetComponent<UniversalCameraFollow>().RespawnCamera(nearestRespawnPoint.position);
    }

    // HandlePlatformEffectorCollision 메소드를 변경하여 다양한 플랫폼을 처리할 수 있도록 함
    private void HandlePlatformEffectorCollision(Collider2D platformCollider)
    {
        if (CheckIfPlayerIsOnSide(platformCollider))
        {
            isGrounded = false;
            animator.SetBool("isJump", false);
        }
    }

    // CheckIfPlayerIsOnSideOfUpGround 메소드를 CheckIfPlayerIsOnSide로 일반화하여 다양한 플랫폼을 처리할 수 있도록 함
    private bool CheckIfPlayerIsOnSide(Collider2D platformCollider)
    {
        Collider2D playerCollider = GetComponent<Collider2D>();

        Bounds playerBounds = playerCollider.bounds;
        Bounds platformBounds = platformCollider.bounds;

        bool isOnSide = playerBounds.center.x < platformBounds.min.x || playerBounds.center.x > platformBounds.max.x;
        isOnSide &= playerBounds.center.y > platformBounds.min.y && playerBounds.center.y < platformBounds.max.y;

        return isOnSide;
    }

    // UI의 하트 이미지를 업데이트하는 메서드
    private void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < PlayerData.playerRemainHeart)
            {
                hearts[i].SetActive(true);
            }
            else
            {
                hearts[i].SetActive(false);
            }
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 오브젝트가 Ground 또는 UpGround 태그를 가지고 있으면 점프 가능 상태로 변경
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("UpGround") || collision.gameObject.CompareTag("Cylinder"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Ground 또는 UpGround에서 떨어지면 점프 불가능 상태로 변경
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("UpGround") || collision.gameObject.CompareTag("Cylinder"))
        {
            isGrounded = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MoveController : MonoBehaviour
{
    [SerializeField] private LayerMask GroundLayer;

    [Header("Rope Interaction")]
    public Tilemap ropeTilemap; // Rope 타일맵의 참조
    public LayerMask RopeLayer; // Rope 타일 레이어 마스크
    public LayerMask UpGroundLayer; // Rope 타일 레이어 마스크
    private bool isOnRope = false; // 현재 Rope 타일과 상호작용 중인지 여부
    private bool isRope = false;
    private Vector3 ropeTileCenter;
    private Collider2D ignoredCollider = null; // 무시된 collider를 저장할 변수

    [Header("Ladder Interaction")]
    public Tilemap ladderTilemap; // Ladder 타일맵의 참조
    public LayerMask LadderLayer; // Ladder 타일 레이어 마스크
    private bool isOnLadder = false; // 현재 Ladder 타일과 상호작용 중인지 여부
    private bool isLadder = false;
    private Vector3 ladderTileCenter;

    public float moveSpeed = 2f;
    public float jumpPower = 5f;

    Rigidbody2D rigidBody2D;
    Animator anim;
    Collider2D col2D;

    float horizontalMove = 0f;
    float verticalMove = 0f;

    bool isJumpKeyPressed = false;
    bool isJumping = false;
    bool isGrounded = false;

    float clickTime = 0f; // 마지막 클릭 시간
    float clickDelay = 0.5f; // 더블 클릭으로 간주할 최대 시간 간격

    // Start is called before the first frame update
    void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col2D = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameController.isPause)
        {
            UserKeySetting();
            CheckIsGrounded();
            CheckRopeInteraction();
            CheckLadderInteraction();
            CheckAnimCondition();
            InputMoveValue();
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void InputMoveValue()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");

        // 밧줄 상태나 사다리 상태에서는 점프 키 입력 무시
        if (Input.GetKeyDown(KeyCode.Space) && !isRope && !isLadder)
        {
            isJumpKeyPressed = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            anim.SetTrigger("Attack");
        }

        SetMoveDir();
    }

    private void MovePlayer()
    {
        // Rope Interaction
        if (isOnRope && verticalMove != 0)
        {
            isRope = true;
            isJumping = false;  // Ensure the player is not in jumping state
            rigidBody2D.velocity = new Vector2(0, verticalMove * moveSpeed);
            transform.position = new Vector3(ropeTileCenter.x, transform.position.y, transform.position.z);
        }
        else if (isRope && !isGrounded && verticalMove == 0)
        {
            rigidBody2D.gravityScale = 0;
            rigidBody2D.velocity = Vector2.zero;
            isRope = true;
        }
        else
        {
            isRope = false;
        }

        if (isRope && verticalMove < 0) // 밧줄에서 아래로 내려가려 할 때
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, UpGroundLayer);
            if (hit.collider != null && hit.collider.CompareTag("UpGround"))
            {
                Physics2D.IgnoreCollision(col2D, hit.collider, true);
                ignoredCollider = hit.collider; // 무시된 collider 저장
            }
        }

        // Ladder Interaction
        if (isOnLadder && verticalMove != 0)
        {
            // Check if a ladder tile exists below the player when moving downward
            if (verticalMove < 0 && !HasTileBelow(ladderTilemap))
            {
                return;
            }

            isLadder = true;
            isJumping = false;  // Ensure the player is not in jumping state
            rigidBody2D.velocity = new Vector2(0, verticalMove * moveSpeed);
            transform.position = new Vector3(ladderTileCenter.x, transform.position.y, transform.position.z);
        }
        else if (isLadder && !isGrounded && verticalMove == 0)
        {
            rigidBody2D.gravityScale = 0;
            rigidBody2D.velocity = Vector2.zero;
            isLadder = true;
        }
        else
        {
            isLadder = false;
        }

        // 사다리에서 아래로 내려가려 할 때
        if (isLadder && verticalMove < 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, UpGroundLayer);

            if (hit.collider != null && hit.collider.CompareTag("UpGround"))
            {
                Physics2D.IgnoreCollision(col2D, hit.collider, true);
                ignoredCollider = hit.collider; // 무시된 collider 저장
            }
        }

        // Handle ignored colliders for rope and ladder
        if (ignoredCollider != null)
        {
            if (col2D.bounds.max.y < ignoredCollider.bounds.min.y)
            {
                Physics2D.IgnoreCollision(col2D, ignoredCollider, false);
                ignoredCollider = null; // 무시된 collider 초기화
            }
        }

        // Normal Movement
        if (!isRope && !isLadder)
        {
            rigidBody2D.gravityScale = 1;
            rigidBody2D.velocity = new Vector2(horizontalMove * moveSpeed, rigidBody2D.velocity.y);

            if (isJumpKeyPressed && isGrounded)
            {
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, jumpPower);
                isJumpKeyPressed = false;
                isJumping = true;
            }
        }
    }


    private void SetMoveDir()
    {
        if (horizontalMove < 0) transform.localScale = new Vector3(1, 1, 1);
        if (horizontalMove > 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    private void CheckIsGrounded()
    {
        int combinedLayerMask = GroundLayer | UpGroundLayer;
        float extraHeight = 0.05f;
        RaycastHit2D hit = Physics2D.Raycast(col2D.bounds.center + new Vector3(0, -col2D.bounds.extents.y, 0), Vector2.down, extraHeight, combinedLayerMask);

        bool wasGrounded = isGrounded;
        isGrounded = (hit.collider != null);

        if (isGrounded) isJumping = false;

        // 밧줄에서 내려와 땅에 닿은 경우
        if (isRope && wasGrounded == false && isGrounded == true && rigidBody2D.velocity.y <= 0.1f)
        {
            isRope = false;
        }

        // 사다리에서 내려와 땅에 닿은 경우
        if (isLadder && wasGrounded == false && isGrounded == true && rigidBody2D.velocity.y <= 0.1f)
        {
            isLadder = false; // 사다리 상태 해제
            isOnLadder = false; // 사다리 상호작용 상태 해제
        }

        // 땅에 완전히 착지하고 아래 방향키를 계속 누르고 있는 경우
        if (isRope && isGrounded && verticalMove < 0)
        {
            isRope = false;
        }

        // 사다리에서 땅에 완전히 착지하고 아래 방향키를 계속 누르고 있는 경우
        if (isLadder && isGrounded && verticalMove < 0)
        {
            isLadder = false;
            isOnLadder = false;
        }
    }

    private bool HasTileBelow(Tilemap tilemap)
    {
        Vector3 footPosition = col2D.bounds.center + new Vector3(0, -col2D.bounds.extents.y - 0.1f, 0);  // Just below the feet
        Vector3Int cellPosition = tilemap.WorldToCell(footPosition);
        return tilemap.HasTile(cellPosition);
    }

    private bool HasTileAbove(Tilemap tilemap)
    {
        Vector3 headPosition = col2D.bounds.center + new Vector3(0, col2D.bounds.extents.y + 0.1f, 0);  // Just above the head
        Vector3Int cellPosition = tilemap.WorldToCell(headPosition);
        return tilemap.HasTile(cellPosition);
    }


    private void CheckRopeInteraction()
    {
        CheckTileInteraction(ref isOnRope, ref isRope, ref ropeTileCenter, RopeLayer, -0.2f);
    }

    private void CheckLadderInteraction()
    {
        CheckTileInteraction(ref isOnLadder, ref isLadder, ref ladderTileCenter, LadderLayer, 0);
    }

    private void CheckTileInteraction(ref bool isOnTile, ref bool isTile, ref Vector3 tileCenter, LayerMask tileLayer, float offset)
    {
        RaycastHit2D hit = Physics2D.BoxCast(col2D.bounds.center, col2D.bounds.size, 0f, Vector2.up, 0.02f, tileLayer);

        if (hit.collider != null)
        {
            isOnTile = true;
            Debug.Log("On Tile: " + hit.collider.name);

            // Get the tile's actual center position
            var tilemap = hit.collider.GetComponent<Tilemap>();
            Vector3Int cellPosition = tilemap.WorldToCell(hit.point);
            Vector3 actualTileCenter = tilemap.GetCellCenterWorld(cellPosition);

            // Apply the offset
            tileCenter = actualTileCenter + new Vector3(offset, 0, 0);
        }
        else
        {
            isOnTile = false;
            isTile = false;
            Debug.Log("Not on any tile");

            // Check if we have an ignored collider and if it's "Floor3 Top"
            if (ignoredCollider != null && ignoredCollider.name == "Floor3 Top")
            {
                Debug.Log("Stopped ignoring collision with: " + ignoredCollider.name);
                Physics2D.IgnoreCollision(col2D, ignoredCollider, false);
                ignoredCollider = null;
            }
        }
    }

    private void CheckAnimCondition()
    {
        anim.SetBool("isWalk", horizontalMove != 0);
        anim.SetBool("isJump", isJumping);
        anim.SetBool("isRope", isRope);
        anim.SetBool("isLadder", isLadder);
        anim.SetBool("isGrounded", isGrounded);
    }

    private void UserKeySetting()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            EventManager.instance.SendEvent("Inventory :: UIToggle");
        }

        if (Input.GetMouseButtonDown(0)) // 왼쪽 마우스 버튼을 클릭했는지 확인
        {
            if (Time.time - clickTime < clickDelay)
            {
                DoubleClickEvent("ShopNPC", "Shop :: UIToggle");
                DoubleClickEvent("QuestNPC", "Quest :: UIToggle");
            }
            clickTime = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerData.PrintData();
        }
    }

    private void DoubleClickEvent(string tag, string eventName)
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag(tag))
        {
            // ShopNPC 오브젝트에 더블 클릭 했을 때의 로직을 여기에 작성하세요.
            EventManager.instance.SendEvent(eventName, hit.collider.gameObject); // Send NPC GameObject as a parameter
        }
    }
}

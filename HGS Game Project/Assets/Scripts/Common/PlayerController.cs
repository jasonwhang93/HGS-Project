using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public TilemapController tilemapController;

    public float moveSpeed = 2f;
    public float jumpPower = 5f;

    private Rigidbody2D rigidBody2D;
    private Animator anim;
    private Collider2D col2D;

    private bool isOnRope = false; // 현재 Rope 타일과 상호작용 중인지 여부
    private bool isRope = false;
    private Vector3 ropeTileCenter;

    private bool isOnLadder = false; // 현재 Ladder 타일과 상호작용 중인지 여부ropeTileCenter
    private bool isLadder = false;
    private Vector3 ladderTileCenter;

    private float horizontalMove = 0f;
    private float verticalMove = 0f;

    private bool isJumpKeyPressed = false;
    private bool isJumping = false;
    private bool isGrounded = false;

    private float clickTime = 0f; // 마지막 클릭 시간
    private float clickDelay = 0.5f; // 더블 클릭으로 간주할 최대 시간 간격

    // Start is called before the first frame update
    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col2D = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!GameController.isPause)
        {
            UserKeySetting();
            CheckIsGrounded();
            tilemapController.CheckRopeInteraction(col2D, ref isOnRope, ref isRope, ref ropeTileCenter);
            tilemapController.CheckLadderInteraction(col2D, ref isOnLadder, ref isLadder, ref ladderTileCenter);
            CheckAnimCondition();
            InputMoveValue();

            // Floor1 Top에서 Floor2 StairSide 콜라이더 통과 로직
            if ((IsPlayerOnFloor1Top() || !IsPlayerOnUpGround()) && !isJumping)
            {
                // Floor2 StairSide 콜라이더를 무시
                tilemapController.IgnoreCollider(col2D, tilemapController.floor2Side);
                tilemapController.IgnoreCollider(col2D, tilemapController.floor2StairTopTilemapCol);
                tilemapController.IgnoreCollider(col2D, tilemapController.floor2StairTopCompositeCol);
                tilemapController.IgnoreCollider(col2D, tilemapController.floor2TopTilemapCol);
                tilemapController.IgnoreCollider(col2D, tilemapController.floor2TopCompositeCol);
            }
            else
            {
                // 콜라이더 무시 해제
                tilemapController.ResetIgnoredCollider(col2D);
            }

            Vector3Int tilePosition = tilemapController.GetCurrentTilePosition(transform);

            // 특정 타일 위치에서 콜라이더 무시 해제
            tilemapController.ResetIgnoredColliderAtSpecificTile(col2D, tilePosition);
            //Debug.Log("현재 타일 위치: " + tilePosition);
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

    // HandleLanding 메소드 수정
    private void HandleLanding()
    {
        if (!isJumping && isGrounded)
        {
            Vector3Int currentTilePosition = tilemapController.GetCurrentTilePosition(transform);

            // 겹쳐진 영역이 아닌 경우에만 콜라이더 활성화
            if (!tilemapController.IsOverlappingArea(currentTilePosition))
            {
                tilemapController.ResetIgnoredCollider(col2D);
            }
            else
            {
                Debug.Log("HandleLanding");
                // 겹쳐진 영역인 경우, 계속 떨어짐
                isGrounded = false;
            }
        }
    }

    private bool IsPlayerOnUpGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.0f, tilemapController.UpGroundLayer);

        return hit.collider != null;
    }


    private bool IsPlayerOnFloor1Top()
    {
        // 플레이어의 위치에서 바로 아래로 Raycast를 발사
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.0f, tilemapController.GroundLayer);

        if (hit.collider != null)
        {
            // Raycast가 GroundLayer에 닿았고, 해당 오브젝트의 이름이 "Floor1 Top"인지 확인
            return (hit.collider.gameObject.name == "Floor1 Top" || hit.collider.gameObject.name == "Floor1 SideTop");
        }

        return false;
    }

    private void MovePlayer()
    {
        // Rope Interaction
        if (isOnRope && verticalMove != 0)
        {
            isRope = true;
            isJumping = false;
            rigidBody2D.velocity = new Vector2(0, verticalMove * moveSpeed);
            transform.position = new Vector3(ropeTileCenter.x, transform.position.y, transform.position.z);

            // 아래로 이동 중일 때 UpGround 콜라이더 무시
            if (verticalMove < 0)
            {
                tilemapController.IgnoreColliderOnDownward(col2D, "UpGround");
            }
        }
        else if (isRope && !isGrounded && verticalMove == 0)
        {
            rigidBody2D.gravityScale = 0;
            rigidBody2D.velocity = Vector2.zero;
        }
        else
        {
            isRope = false;
        }

        // Ladder Interaction
        if (isOnLadder && verticalMove != 0)
        {
            isLadder = true;
            isJumping = false;
            rigidBody2D.velocity = new Vector2(0, verticalMove * moveSpeed);
            transform.position = new Vector3(ladderTileCenter.x, transform.position.y, transform.position.z);

            // 아래로 이동 중일 때 UpGround 콜라이더 무시
            if (verticalMove < 0)
            {
                tilemapController.IgnoreColliderOnDownward(col2D, "UpGround");
            }
        }
        else if (isLadder && !isGrounded && verticalMove == 0)
        {
            rigidBody2D.gravityScale = 0;
            rigidBody2D.velocity = Vector2.zero;
        }
        else
        {
            isLadder = false;
        }

        if(!isGrounded && !isOnRope && !isOnLadder)
        {
            tilemapController.ResetIgnoredCollider(col2D);
        }

        if(isGrounded && !tilemapController.HasTileBelow(col2D, tilemapController.GetLadderTilemap()) && 
            tilemapController.HasTileAbove(col2D, tilemapController.GetLadderTilemap()))
        {
            tilemapController.ResetIgnoredCollider(col2D);
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
        bool wasGrounded = isGrounded;

        isGrounded = tilemapController.IsGrounded(col2D, ref wasGrounded);

        if (isGrounded)
        {
            isJumping = false;
        }

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

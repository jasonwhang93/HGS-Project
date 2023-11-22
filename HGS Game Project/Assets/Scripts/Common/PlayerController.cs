using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public TilemapController tilemapController;
    public ColliderController colliderController;

    public float moveSpeed = 2f;
    public float jumpPower = 5f;

    private Rigidbody2D rigidBody2D;
    private Animator anim;
    private Collider2D col2D;

    private bool isOnRope = false; // 현재 Rope 타일과 상호작용 중인지 여부
    private bool isRope = false;

    private bool isOnLadder = false; // 현재 Ladder 타일과 상호작용 중인지 여부ropeTileCenter
    private bool isLadder = false;

    private float horizontalMove = 0f;
    private float verticalMove = 0f;

    private bool isJumpKeyPressed = false;
    private bool isJumping = false;

    private int groundLayerInfo = -1;

    private bool isGrounded = false;
    public float raycastDistance = 0.5f;
    // 현재 플레이어가 Floor1 Top에 있는지 여부를 저장하는 변수 추가
    private bool jumpedFromFloor1Top = false;

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
            CheckAnimCondition();
            InputMoveValue();         

            tilemapController.PrintIgnoredColliders();
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
        if (Input.GetKeyDown(KeyCode.Space) && !isRope && !isLadder && !isJumping)
        {
            isJumpKeyPressed = true;
            // 점프했을 때 플레이어가 Floor1 Top에 있는지 확인
            jumpedFromFloor1Top = tilemapController.IsPlayerOnFloor1Top(transform, raycastDistance);
        }

        // 밧줄과 사다리 상호작용 업데이트
        UpdateConnectTileInteraction();

        SetMoveDir();
    }


    private void MovePlayer()
    {
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
        else
        {
            // 밧줄이나 사다리에 대한 상호작용 업데이트
            if (isOnRope)
            {
                tilemapController.CheckAndUpdateConnectInteraction(col2D, tilemapController.ropeTilemap);
            }
            if (isOnLadder)
            {
                tilemapController.CheckAndUpdateConnectInteraction(col2D, tilemapController.ladderTilemap);
            }

            rigidBody2D.gravityScale = 0;
            rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, verticalMove * moveSpeed);

            if (isJumpKeyPressed)
            {
                isJumpKeyPressed = false;
                isJumping = false;
            }
        }
    }


    private void SetMoveDir()
    {
        if (horizontalMove < 0) transform.localScale = new Vector3(1, 1, 1);
        if (horizontalMove > 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    private void UpdateConnectTileInteraction()
    {
        // 밧줄 사용 검사
        if (isGrounded && isOnRope && verticalMove != 0)
        {
            isRope = true;
        }
        else if (((1 << groundLayerInfo) == tilemapController.UpGroundLayer) && isRope)
        {
            isRope = true;
        }
        else if(isJumping && isOnRope && verticalMove != 0)
        {
            isRope = true;
            isJumpKeyPressed = false;
            isJumping = false;
        }
        else if (((1 << groundLayerInfo) == tilemapController.GroundLayer) && isRope)
        {
            isRope = false;
        }
        else if(!isGrounded && !isOnRope)
        {
            isRope = false;
        }

        // 사다리 사용 검사
        if (isGrounded && isOnLadder && verticalMove != 0)
        {
            isLadder = true;
        }
        else if (((1 << groundLayerInfo) == tilemapController.UpGroundLayer) && isLadder)
        {
            isLadder = true;
        }
        else if (isJumping && isOnLadder && verticalMove != 0)
        {
            isLadder = true;
            isJumpKeyPressed = false;
            isJumping = false;
        }
        else if (((1 << groundLayerInfo) == tilemapController.GroundLayer) && isLadder)
        {
            isLadder = false;
        }
        else if (!isGrounded && !isOnLadder)
        {
            isLadder = false;
        }
    }


    private void CheckIsGrounded()
    {
        bool wasGrounded = isGrounded;

        isGrounded = tilemapController.IsGrounded(col2D, ref wasGrounded, out groundLayerInfo);

        if (isGrounded)
        {
            isJumping = false;
        }
    }


    private void CheckAnimCondition()
    {
        anim.SetBool("isWalk", horizontalMove != 0);
        anim.SetBool("isJump", isJumping);
        anim.SetBool("isRope", isRope);
        anim.SetBool("isLadder", isLadder);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isOnRope", isOnRope);
        anim.SetBool("isOnLadder", isOnLadder);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Stair"))
        {
            if (tilemapController.IsPlayerOnFloor1Top(transform, raycastDistance))
            {
                var physicalCollider = tilemapController.GetPhysicalColliderForStair(other.name);

                if (physicalCollider != null && !tilemapController.GetIgnoredColliders().Contains(physicalCollider))
                {
                    colliderController.IgnoreCollider(col2D, physicalCollider);
                }
            }

            // Floor1 Top에서 점프한 상태에서 Floor2 TriggerStairSide에 접촉했을 때
            if (isJumping && jumpedFromFloor1Top && other.name == "Floor2 TriggerStairSide")
            {
                colliderController.IgnoreCollider(col2D, tilemapController.floor2StairTopTilemapCol);
            }
        }

        // Floor3 TriggerTop 타일에 접촉하고, 로프를 사용 중일 때
        if (isRope && other.name == "Floor3 TriggerTop")
        {
            if (!tilemapController.GetIgnoredColliders().Contains(tilemapController.floor3TopTilemapCol))
            {
                colliderController.IgnoreCollider(col2D, tilemapController.floor3TopTilemapCol);
            }
            if (!tilemapController.GetIgnoredColliders().Contains(tilemapController.floor3TopCompositeCol))
            {
                colliderController.IgnoreCollider(col2D, tilemapController.floor3TopCompositeCol);
            }

        }

        if (isLadder && other.name == "Floor3 TriggerTop")
        {
            if (!tilemapController.GetIgnoredColliders().Contains(tilemapController.floor3TopTilemapCol))
            {
                colliderController.IgnoreCollider(col2D, tilemapController.floor3TopTilemapCol);
            }
            if (!tilemapController.GetIgnoredColliders().Contains(tilemapController.floor3TopCompositeCol))
            {
                colliderController.IgnoreCollider(col2D, tilemapController.floor3TopCompositeCol);
            }
        }

        if ((1 << other.gameObject.layer) == tilemapController.RopeLayer)
        {
            isOnRope = true;
        }

        if ((1 << other.gameObject.layer) == tilemapController.LadderLayer)
        {
            isOnLadder = true;
            // 사다리에 대한 추가 처리
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Stair"))
        {
            if (tilemapController.IsPlayerOnFloor1Top(transform, raycastDistance))
            {
                var physicalCollider = tilemapController.GetPhysicalColliderForStair(other.name);

                if (physicalCollider != null && !tilemapController.GetIgnoredColliders().Contains(physicalCollider))
                {
                    colliderController.IgnoreCollider(col2D, physicalCollider);
                }
            }
        }

        // Floor3 TriggerTop 타일 영역 내에 있고, 로프를 사용 중일 때
        if (isRope && other.name == "Floor3 TriggerTop")
        {
            if (!tilemapController.GetIgnoredColliders().Contains(tilemapController.floor3TopTilemapCol))
            {            
                colliderController.IgnoreCollider(col2D, tilemapController.floor3TopTilemapCol);
            }
            if (!tilemapController.GetIgnoredColliders().Contains(tilemapController.floor3TopCompositeCol))
            {
                colliderController.IgnoreCollider(col2D, tilemapController.floor3TopCompositeCol);
            }
        }

        if (isLadder && other.name == "Floor3 TriggerTop")
        {
            if (!tilemapController.GetIgnoredColliders().Contains(tilemapController.floor3TopTilemapCol))
            {
                colliderController.IgnoreCollider(col2D, tilemapController.floor3TopTilemapCol);
            }
            if (!tilemapController.GetIgnoredColliders().Contains(tilemapController.floor3TopCompositeCol))
            {
                colliderController.IgnoreCollider(col2D, tilemapController.floor3TopCompositeCol);
            }
        }

        if ((1 << other.gameObject.layer) == tilemapController.RopeLayer)
        {
            isOnRope = true;
        }

        if ((1 << other.gameObject.layer) == tilemapController.LadderLayer)
        {
            isOnLadder = true;
            // 사다리 탈출에 대한 추가 처리
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Stair"))
        {
            var physicalCollider = tilemapController.GetPhysicalColliderForStair(other.name);

            if (physicalCollider != null)
            {
                colliderController.ResetIgnoredCollider(col2D, physicalCollider);
            }

            // Floor1 Top에서 점프한 상태에서 Floor2 TriggerStairSide를 벗어났을 때
            if (isJumping && jumpedFromFloor1Top && other.name == "Floor2 TriggerStairSide")
            {
                colliderController.ResetIgnoredCollider(col2D, tilemapController.floor2StairTopTilemapCol);
            }
        }

        // Floor3 TriggerTop 타일 영역을 벗어났을 때
        if (other.name == "Floor3 TriggerTop")
        {
            colliderController.ResetIgnoredCollider(col2D, tilemapController.floor3TopTilemapCol);
            colliderController.ResetIgnoredCollider(col2D, tilemapController.floor3TopCompositeCol);
        }

        if ((1 << other.gameObject.layer) == tilemapController.RopeLayer)
        {
            isOnRope = false;
        }

        if ((1 << other.gameObject.layer) == tilemapController.LadderLayer)
        {
            isOnLadder = false;
            // 사다리 탈출에 대한 추가 처리
        }
    }
}

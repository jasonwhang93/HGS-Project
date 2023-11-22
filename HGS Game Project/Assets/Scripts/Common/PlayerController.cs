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

    private bool isOnRope = false; // ���� Rope Ÿ�ϰ� ��ȣ�ۿ� ������ ����
    private bool isRope = false;

    private bool isOnLadder = false; // ���� Ladder Ÿ�ϰ� ��ȣ�ۿ� ������ ����ropeTileCenter
    private bool isLadder = false;

    private float horizontalMove = 0f;
    private float verticalMove = 0f;

    private bool isJumpKeyPressed = false;
    private bool isJumping = false;

    private int groundLayerInfo = -1;

    private bool isGrounded = false;
    public float raycastDistance = 0.5f;
    // ���� �÷��̾ Floor1 Top�� �ִ��� ���θ� �����ϴ� ���� �߰�
    private bool jumpedFromFloor1Top = false;

    private float clickTime = 0f; // ������ Ŭ�� �ð�
    private float clickDelay = 0.5f; // ���� Ŭ������ ������ �ִ� �ð� ����

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

        // ���� ���³� ��ٸ� ���¿����� ���� Ű �Է� ����
        if (Input.GetKeyDown(KeyCode.Space) && !isRope && !isLadder && !isJumping)
        {
            isJumpKeyPressed = true;
            // �������� �� �÷��̾ Floor1 Top�� �ִ��� Ȯ��
            jumpedFromFloor1Top = tilemapController.IsPlayerOnFloor1Top(transform, raycastDistance);
        }

        // ���ٰ� ��ٸ� ��ȣ�ۿ� ������Ʈ
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
            // �����̳� ��ٸ��� ���� ��ȣ�ۿ� ������Ʈ
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
        // ���� ��� �˻�
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

        // ��ٸ� ��� �˻�
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

        if (Input.GetMouseButtonDown(0)) // ���� ���콺 ��ư�� Ŭ���ߴ��� Ȯ��
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
            // ShopNPC ������Ʈ�� ���� Ŭ�� ���� ���� ������ ���⿡ �ۼ��ϼ���.
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

            // Floor1 Top���� ������ ���¿��� Floor2 TriggerStairSide�� �������� ��
            if (isJumping && jumpedFromFloor1Top && other.name == "Floor2 TriggerStairSide")
            {
                colliderController.IgnoreCollider(col2D, tilemapController.floor2StairTopTilemapCol);
            }
        }

        // Floor3 TriggerTop Ÿ�Ͽ� �����ϰ�, ������ ��� ���� ��
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
            // ��ٸ��� ���� �߰� ó��
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

        // Floor3 TriggerTop Ÿ�� ���� ���� �ְ�, ������ ��� ���� ��
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
            // ��ٸ� Ż�⿡ ���� �߰� ó��
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

            // Floor1 Top���� ������ ���¿��� Floor2 TriggerStairSide�� ����� ��
            if (isJumping && jumpedFromFloor1Top && other.name == "Floor2 TriggerStairSide")
            {
                colliderController.ResetIgnoredCollider(col2D, tilemapController.floor2StairTopTilemapCol);
            }
        }

        // Floor3 TriggerTop Ÿ�� ������ ����� ��
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
            // ��ٸ� Ż�⿡ ���� �߰� ó��
        }
    }
}

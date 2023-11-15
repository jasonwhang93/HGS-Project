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

    private bool isOnRope = false; // ���� Rope Ÿ�ϰ� ��ȣ�ۿ� ������ ����
    private bool isRope = false;
    private Vector3 ropeTileCenter;

    private bool isOnLadder = false; // ���� Ladder Ÿ�ϰ� ��ȣ�ۿ� ������ ����ropeTileCenter
    private bool isLadder = false;
    private Vector3 ladderTileCenter;

    private float horizontalMove = 0f;
    private float verticalMove = 0f;

    private bool isJumpKeyPressed = false;
    private bool isJumping = false;
    private bool isGrounded = false;

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
            tilemapController.CheckRopeInteraction(col2D, ref isOnRope, ref isRope, ref ropeTileCenter);
            tilemapController.CheckLadderInteraction(col2D, ref isOnLadder, ref isLadder, ref ladderTileCenter);
            CheckAnimCondition();
            InputMoveValue();

            // Floor1 Top���� Floor2 StairSide �ݶ��̴� ��� ����
            if ((IsPlayerOnFloor1Top() || !IsPlayerOnUpGround()) && !isJumping)
            {
                // Floor2 StairSide �ݶ��̴��� ����
                tilemapController.IgnoreCollider(col2D, tilemapController.floor2Side);
                tilemapController.IgnoreCollider(col2D, tilemapController.floor2StairTopTilemapCol);
                tilemapController.IgnoreCollider(col2D, tilemapController.floor2StairTopCompositeCol);
                tilemapController.IgnoreCollider(col2D, tilemapController.floor2TopTilemapCol);
                tilemapController.IgnoreCollider(col2D, tilemapController.floor2TopCompositeCol);
            }
            else
            {
                // �ݶ��̴� ���� ����
                tilemapController.ResetIgnoredCollider(col2D);
            }

            Vector3Int tilePosition = tilemapController.GetCurrentTilePosition(transform);

            // Ư�� Ÿ�� ��ġ���� �ݶ��̴� ���� ����
            tilemapController.ResetIgnoredColliderAtSpecificTile(col2D, tilePosition);
            //Debug.Log("���� Ÿ�� ��ġ: " + tilePosition);
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

    // HandleLanding �޼ҵ� ����
    private void HandleLanding()
    {
        if (!isJumping && isGrounded)
        {
            Vector3Int currentTilePosition = tilemapController.GetCurrentTilePosition(transform);

            // ������ ������ �ƴ� ��쿡�� �ݶ��̴� Ȱ��ȭ
            if (!tilemapController.IsOverlappingArea(currentTilePosition))
            {
                tilemapController.ResetIgnoredCollider(col2D);
            }
            else
            {
                Debug.Log("HandleLanding");
                // ������ ������ ���, ��� ������
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
        // �÷��̾��� ��ġ���� �ٷ� �Ʒ��� Raycast�� �߻�
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.0f, tilemapController.GroundLayer);

        if (hit.collider != null)
        {
            // Raycast�� GroundLayer�� ��Ұ�, �ش� ������Ʈ�� �̸��� "Floor1 Top"���� Ȯ��
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

            // �Ʒ��� �̵� ���� �� UpGround �ݶ��̴� ����
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

            // �Ʒ��� �̵� ���� �� UpGround �ݶ��̴� ����
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

        // ���ٿ��� ������ ���� ���� ���
        if (isRope && wasGrounded == false && isGrounded == true && rigidBody2D.velocity.y <= 0.1f)
        {
            isRope = false;
        }

        // ��ٸ����� ������ ���� ���� ���
        if (isLadder && wasGrounded == false && isGrounded == true && rigidBody2D.velocity.y <= 0.1f)
        {
            isLadder = false; // ��ٸ� ���� ����
            isOnLadder = false; // ��ٸ� ��ȣ�ۿ� ���� ����
        }

        // ���� ������ �����ϰ� �Ʒ� ����Ű�� ��� ������ �ִ� ���
        if (isRope && isGrounded && verticalMove < 0)
        {
            isRope = false;
        }

        // ��ٸ����� ���� ������ �����ϰ� �Ʒ� ����Ű�� ��� ������ �ִ� ���
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map2MoveController : MonoBehaviour
{
    public float moveSpeed = 5.0f;  // �̵� �ӵ�
    public float jumpForce = 9.0f;  // ���� ��
    public GameObject upGround; // UpGround ���� ������Ʈ�� ���� ����
    public GameObject cylinderObject; // CylinderObject�� ���� ���� �߰�

    private Collider2D upGroundCollider;
    private Collider2D cylinderObjectCollider; // CylinderObject�� Collider

    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 characterScale;  // ĳ������ �ʱ� ũ�� ����
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

        // ĳ������ �ʱ� ������ ���� (�������� �⺻ �������� ����)
        characterScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        transform.localScale = characterScale; // �������� �ٶ󺸵��� ����
    }

    private void Update()
    {
        Move();
        Jump();
        HandlePlatformEffectorCollision(upGroundCollider); // UpGround ó��
        HandlePlatformEffectorCollision(cylinderObjectCollider); // CylinderObject ó��

        // Animator ���� ������Ʈ
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isWalk", rb.velocity.x != 0 && isGrounded);
        animator.SetBool("isJump", !isGrounded);
        animator.SetBool("isLadder", false);
        animator.SetBool("isRope", false);
    }

    private void Move()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");

        // ĳ���� ���� ����
        if (horizontalMove > 0) // ���������� �̵��� ��
        {
            transform.localScale = new Vector3(-Mathf.Abs(characterScale.x), characterScale.y, characterScale.z);
        }
        else if (horizontalMove < 0) // �������� �̵��� ��
        {
            transform.localScale = new Vector3(Mathf.Abs(characterScale.x), characterScale.y, characterScale.z);
        }

        // ĳ���� �̵�
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

    // HandlePlatformEffectorCollision �޼ҵ带 �����Ͽ� �پ��� �÷����� ó���� �� �ֵ��� ��
    private void HandlePlatformEffectorCollision(Collider2D platformCollider)
    {
        if (CheckIfPlayerIsOnSide(platformCollider))
        {
            isGrounded = false;
            animator.SetBool("isJump", false);
        }
    }

    // CheckIfPlayerIsOnSideOfUpGround �޼ҵ带 CheckIfPlayerIsOnSide�� �Ϲ�ȭ�Ͽ� �پ��� �÷����� ó���� �� �ֵ��� ��
    private bool CheckIfPlayerIsOnSide(Collider2D platformCollider)
    {
        Collider2D playerCollider = GetComponent<Collider2D>();

        Bounds playerBounds = playerCollider.bounds;
        Bounds platformBounds = platformCollider.bounds;

        bool isOnSide = playerBounds.center.x < platformBounds.min.x || playerBounds.center.x > platformBounds.max.x;
        isOnSide &= playerBounds.center.y > platformBounds.min.y && playerBounds.center.y < platformBounds.max.y;

        return isOnSide;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �浹�� ������Ʈ�� Ground �Ǵ� UpGround �±׸� ������ ������ ���� ���� ���·� ����
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("UpGround") || collision.gameObject.CompareTag("Cylinder"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Ground �Ǵ� UpGround���� �������� ���� �Ұ��� ���·� ����
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("UpGround") || collision.gameObject.CompareTag("Cylinder"))
        {
            isGrounded = false;
        }
    }
}
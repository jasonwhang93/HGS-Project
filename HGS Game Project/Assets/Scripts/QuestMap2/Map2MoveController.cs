using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class Map2MoveController : MonoBehaviour
{
    public float moveSpeed = 5.0f;  // 이동 속도
    public float jumpForce = 7.0f;  // 점프 힘
    private bool isJumping = false;  // 점프 중인지 여부

    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 characterScale;  // 캐릭터의 초기 크기 저장

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        characterScale = transform.localScale;
    }

    private void Update()
    {
        Move();
        Jump();

        // Animator 변수 업데이트
        animator.SetBool("isWalk", rb.velocity.x != 0 && !isJumping);
        animator.SetBool("isJump", isJumping);
    }

    private void Move()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");

        // 캐릭터 방향 변경
        if (horizontalMove < 0)
            transform.localScale = characterScale;
        else if (horizontalMove > 0)
            transform.localScale = new Vector3(-characterScale.x, characterScale.y, characterScale.z);

        Vector2 moveDirection = new Vector2(horizontalMove * moveSpeed, rb.velocity.y);
        rb.velocity = moveDirection;
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            isJumping = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 플레이어가 땅에 닿으면 다시 점프 가능 상태로 변경
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
    }
}

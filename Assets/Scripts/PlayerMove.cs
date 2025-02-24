using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed = 6f; // 최대 속도
    public float jumpForce = 10f; // 점프 힘
    public LayerMask groundLayer; // 바닥 감지 레이어
    public LayerMask wallLayer;   // 벽 감지 레이어

    public SpriteRenderer spriteRenderer;
    public Animator anim;
    public BoxCollider2D boxCollider;
    public Rigidbody2D rb;

    private Vector2 originalOffset; // 기본 offset 값 저장
    private bool isTouchingWall = false; // 벽에 부딪혔는지 체크
    private bool isJumping = false; // 점프 상태 추가

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 3f; // 중력 스케일 조정
        rb.freezeRotation = true; // 회전 방지

        originalOffset = boxCollider.offset;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");

        // 점프 중일 때 벽에 닿아도 수평 이동 가능
        if (isTouchingWall && h != 0 && !isJumping)
        {
            anim.SetBool("isRunning", true);  // 이동 애니메이션 유지
            spriteRenderer.flipX = h < 0;
            FlipCollider(spriteRenderer.flipX);
        }
        else
        {
            // 이동 적용 (바닥에서는 자유롭게 이동 가능)
            rb.linearVelocity = new Vector2(h * maxSpeed, rb.linearVelocity.y);

            // 좌우 키 입력에 따라 flipX 설정
            if (h != 0)
            {
                spriteRenderer.flipX = h < 0;
                FlipCollider(spriteRenderer.flipX);
                anim.SetBool("isRunning", true);
            }
            else
            {
                anim.SetBool("isRunning", false);
            }
        }

        // 점프 입력 처리
        if (boxCollider.IsTouchingLayers(groundLayer) && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            anim.SetBool("isJumping", true);
            anim.SetBool("isFalling", false);
            isJumping = true; // 점프 시작
        }

        // 애니메이션 처리
        HandleAnimations();
    }

    void FlipCollider(bool isFlipped)
    {
        // Collider의 offset을 반전시킴
        boxCollider.offset = new Vector2(isFlipped ? -originalOffset.x : originalOffset.x, originalOffset.y);
    }

    void HandleAnimations()
    {
        bool isGrounded = boxCollider.IsTouchingLayers(groundLayer);

        if (rb.linearVelocity.y < 0 && !isGrounded)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", true);
        }

        if (isGrounded && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", false);
            isJumping = false; // 바닥에 닿으면 점프 종료
        }
    }

    // 벽에 부딪혔을 때 (한 번만 감지)
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            bool touchedWall = false;
            bool touchedGround = false;

            foreach (ContactPoint2D contact in collision.contacts)
            {
                Vector2 normal = contact.normal;
                float angle = Vector2.Angle(normal, Vector2.up);

                // 벽 판정: 75도 이상 각도
                if (angle > 75f)
                {
                    touchedWall = true;
                }

                // 바닥 판정: 30도 이하 각도
                if (angle < 30f)
                {
                    touchedGround = true;
                }
            }

            // 벽에 닿았으면 이동 제한
            if (touchedWall && !isJumping)
            {
                isTouchingWall = true;
            }

            // 바닥에 닿았으면 벽 판정 해제
            if (touchedGround)
            {
                isTouchingWall = false;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            // 벽에서 벗어나면 벽 상태 해제
            isTouchingWall = false;
        }
    }

    // 벽인지 확인하는 함수 (wallLayer만 감지)
    bool IsWall(Collision2D collision)
    {
        return ((1 << collision.gameObject.layer) & wallLayer) != 0;
    }
}

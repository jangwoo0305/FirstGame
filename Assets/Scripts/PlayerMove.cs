using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 5f; // 최대 속도
    public float jumpForce = 7f; // 점프 힘
    public Transform groundCheck; // 바닥 체크용 오브젝트
    public LayerMask groundLayer; // 바닥 감지 레이어

    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private BoxCollider2D boxCollider;

    private bool isGrounded = false;
    private Vector2 originalOffset; // 기본 offset 값 저장

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.freezeRotation = true; // 회전 방지
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        originalOffset = boxCollider.offset;     
    }


    void Update()
    {
        // 좌우 이동 입력
        float h = Input.GetAxisRaw("Horizontal");

        // 방향 전환
        if (h != 0)
        {
            spriteRenderer.flipX = h < 0;
            FlipCollider(spriteRenderer.flipX);
        }

        // 바닥 체크 (Raycast)
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.9f, groundLayer);

        //Debug.DrawRay(transform.position, Vector2.down * 0.3f, Color.red); Debug.DrawRay(transform.position, Vector2.down * 1f, Color.red);

        // 점프 처리
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, jumpForce);
            anim.SetBool("isJumping", true);
            anim.SetBool("isFalling", false);
        }


        // 하강 애니메이션
        if (rigid.linearVelocity.y < 0)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", true);
        }

        // 착지 시 애니메이션 업데이트
        if (isGrounded)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", false);
        }

        // 애니메이션 업데이트
        anim.SetBool("isRunning", h != 0);
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");

        // 즉각적인 반응을 위해 linearVelocity를 직접 조정
        rigid.linearVelocity = new Vector2(h * maxSpeed, rigid.linearVelocity.y);

    }
    void FlipCollider(bool isFlipped)
    {
        boxCollider.offset = new Vector2(isFlipped ? -originalOffset.x : originalOffset.x, originalOffset.y);
    }
}

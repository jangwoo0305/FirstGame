using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 5f; // 최대 속도
    public float jumpForce = 8f; // 점프 힘
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
        float h = Input.GetAxisRaw("Horizontal");

        // 좌우 키 입력에 따라 flipX 설정 (스프라이트 반전만 담당)
        if (h != 0)
        {
            spriteRenderer.flipX = h < 0;  // 좌측 방향일 때 flipX 활성화
            FlipCollider(spriteRenderer.flipX);
            anim.SetBool("isRunning", true); // 좌우 키 입력 시 달리기 애니메이션
        }
        else
        {
            anim.SetBool("isRunning", false); // 좌우 키 입력 없으면 달리기 애니메이션 끄기
        }

        // 바닥 체크 (Raycast)
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.5f, groundLayer);

        // 🔍 isGrounded 값 확인하기
        Debug.Log("Is Grounded: " + isGrounded);

        // Ray를 그려서 확인
        Debug.DrawRay(groundCheck.position, Vector2.down * 0.5f, isGrounded ? Color.green : Color.red);

        // 점프 입력 처리
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Debug.Log("Jumping!");  // 점프 입력 감지 확인
            anim.SetBool("isJumping", true);
            anim.SetBool("isFalling", false);
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, jumpForce);
        }

        // 낙하 애니메이션 처리
        if (rigid.linearVelocity.y < 0 && !isGrounded)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", true);
        }

        // 바닥에 닿았을 때 애니메이션 처리
        if (isGrounded && rigid.linearVelocity.y <= 0)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", false);
        }
        
        
    }





    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        rigid.linearVelocity = new Vector2(h * maxSpeed, rigid.linearVelocity.y);
    }

    void FlipCollider(bool isFlipped)
    {
        boxCollider.offset = new Vector2(isFlipped ? -originalOffset.x : originalOffset.x, originalOffset.y);
    }
}

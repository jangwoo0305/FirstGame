using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed = 5f; // 최대 속도
    public float jumpForce = 8f; // 점프 힘
    public Transform groundCheck; // 바닥 체크용 오브젝트
    public LayerMask groundLayer; // 바닥 감지 레이어

    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private BoxCollider2D boxCollider;

    private bool isGrounded = false;
    private Vector2 originalOffset; // 기본 offset 값 저장
    private Vector2 originallocalPosition;
    private Vector3 velocity; // 이동 속도 관리용 변수

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        originalOffset = boxCollider.offset;
        originallocalPosition = groundCheck.localPosition;
        velocity = Vector3.zero; // 초기 속도 설정
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");

        // 좌우 키 입력에 따라 flipX 설정 (스프라이트 반전만 담당)
        if (h != 0)
        {
            spriteRenderer.flipX = h < 0;  // 좌측 방향일 때 flipX 활성화
            FlipCollider(spriteRenderer.flipX);
            anim.SetBool("isRunning", true); // 달리기 애니메이션
        }
        else
        {
            anim.SetBool("isRunning", false);
        }

        // 바닥 체크 (OverlapCircle)
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        Debug.Log("Is Grounded: " + isGrounded);
        Debug.DrawRay(groundCheck.position, Vector2.down * 0.1f, isGrounded ? Color.green : Color.red);

        // 점프 입력 처리
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Debug.Log("Jumping!");  
            anim.SetBool("isJumping", true);
            anim.SetBool("isFalling", false);
            velocity.y = jumpForce; // 점프 시 Y축 속도 추가
        }

        // 중력 적용
        if (!isGrounded)
        {
            velocity.y += Physics2D.gravity.y * Time.deltaTime; // 중력 적용
        }
        else if (velocity.y < 0)
        {
            velocity.y = 0; // 바닥에 닿으면 낙하 멈춤
        }

        // 애니메이션 처리
        if (velocity.y < 0 && !isGrounded)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", true);
        }

        if (isGrounded && velocity.y <= 0)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", false);
        }

        // 실제 이동 적용
        transform.Translate(new Vector3(h * maxSpeed * Time.deltaTime, velocity.y * Time.deltaTime, 0));
    }

    void FlipCollider(bool isFlipped)
    {
        boxCollider.offset = new Vector2(isFlipped ? -originalOffset.x : originalOffset.x, originalOffset.y);
        groundCheck.localPosition = new Vector2(isFlipped? -originallocalPosition.x : originallocalPosition.x, originallocalPosition.y);
    }


}
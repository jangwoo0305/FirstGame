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
        spriteRenderer.flipX = h < 0;
        FlipCollider(spriteRenderer.flipX);
        anim.SetBool("isRunning", true);
    }
    else
    {
        anim.SetBool("isRunning", false);
    }

    // 바닥 체크 (수직 방향으로도 체크)
    isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer) || CheckGroundBelow();

    Debug.DrawRay(groundCheck.position, Vector2.down * 0.1f, isGrounded ? Color.green : Color.red);

    // 점프 입력 처리
    if (Input.GetButtonDown("Jump") && isGrounded)
    {
        velocity.y = jumpForce;
        anim.SetBool("isJumping", true);
        anim.SetBool("isFalling", false);
    }

    // 중력 적용
    if (!isGrounded)
    {
        velocity.y += Physics2D.gravity.y * Time.deltaTime;
    }
    else if (velocity.y < 0)
    {
        velocity.y = 0;
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

    // 경사 감지 및 보정 이동 적용
    Vector3 slopeAdjustment = CheckSlope(h);

    // 실제 이동 적용 (경사 보정 포함)
    transform.Translate(new Vector3(h * maxSpeed * Time.deltaTime, velocity.y * Time.deltaTime, 0) + slopeAdjustment);
}

/// <summary>
/// 좌우 방향으로 레이를 발사하여 경사로를 감지하고 보정 이동을 계산
/// </summary>
Vector3 CheckSlope(float direction)
{
    if (direction == 0) return Vector3.zero;

    float rayDistance = 0.5f;
    Vector2 rayOrigin = (Vector2)transform.position + Vector2.down * 0.75f;  // 발 밑으로 0.75f 내려줌
    Vector2 rayDirection = new Vector2(direction, 0);

    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, groundLayer);
    
    Debug.DrawRay(rayOrigin, rayDirection * rayDistance, hit ? Color.blue : Color.red);

    if (hit)
    {
        float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

        if (slopeAngle > 0 && slopeAngle < 45f)
        {
            float moveY = Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * maxSpeed * Time.deltaTime;
            return new Vector3(0, moveY, 0);
        }
    }

    return Vector3.zero;
}

/// <summary>
/// 꼭대기에서 떨어지는 걸 방지하기 위해 캐릭터 발 아래 방향으로 추가 레이 발사
/// </summary>
/// <summary>
/// 언덕 꼭대기에서도 정확하게 감지하기 위해 여러 개의 레이를 발사
/// </summary>
bool CheckGroundBelow()
{
    float rayDistance = 0.1f;
    Vector2 rayOriginCenter = (Vector2)transform.position + Vector2.down * 0.75f;  // 발 밑으로 0.75f 내려줌
    Vector2 rayOriginLeft = rayOriginCenter + Vector2.left * 0.1f;  // 왼쪽 발 끝
    Vector2 rayOriginRight = rayOriginCenter + Vector2.right * 0.1f; // 오른쪽 발 끝

    RaycastHit2D hitCenter = Physics2D.Raycast(rayOriginCenter, Vector2.down, rayDistance, groundLayer);
    RaycastHit2D hitLeft = Physics2D.Raycast(rayOriginLeft, Vector2.down, rayDistance, groundLayer);
    RaycastHit2D hitRight = Physics2D.Raycast(rayOriginRight, Vector2.down, rayDistance, groundLayer);

    Debug.DrawRay(rayOriginCenter, Vector2.down * rayDistance, hitCenter ? Color.yellow : Color.red);
    Debug.DrawRay(rayOriginLeft, Vector2.down * rayDistance, hitLeft ? Color.yellow : Color.red);
    Debug.DrawRay(rayOriginRight, Vector2.down * rayDistance, hitRight ? Color.yellow : Color.red);

    // 어느 하나라도 바닥이 감지되면 true 반환
    return hitCenter.collider != null || hitLeft.collider != null || hitRight.collider != null;
}

    void FlipCollider(bool isFlipped)
    {
        boxCollider.offset = new Vector2(isFlipped ? -originalOffset.x : originalOffset.x, originalOffset.y);
        groundCheck.localPosition = new Vector2(isFlipped? -originallocalPosition.x : originallocalPosition.x, originallocalPosition.y);
    }
}
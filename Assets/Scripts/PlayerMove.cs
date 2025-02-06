using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public float maxSpeed = 5f;  // 최대 속도
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.freezeRotation = true; // 회전 방지
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 이동
        float h = Input.GetAxisRaw("Horizontal"); // -1(왼쪽), 0(정지), 1(오른쪽)


        if (h != 0)
        {
            spriteRenderer.flipX = h < 0; // 왼쪽으로 이동하면 flipX를 true로 설정
        }
        if (h != 0)
        {
            spriteRenderer.flipX = h < 0; // 왼쪽으로 이동하면 flipX를 true로 설정
        }
        // 이동 중일 때만 방향 전환

    // 하강 중일 때 (falling)
    if (rigid.linearVelocity.y < 0) 
    {
        // 하강 속도 강제로 증가
        rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, Mathf.Max(rigid.linearVelocity.y, -15f));
        anim.SetBool("isJumping", false); // 점프 상태 종료
        anim.SetBool("isFalling", true); // 하강 애니메이션 계속
    }


        // Running 애니메이션
        if (rigid.linearVelocity.normalized.x == 0)
            anim.SetBool("isRunning", false);
        else
            anim.SetBool("isRunning", true);

        // 점프 (grounded check)
        if (Input.GetButtonDown("Jump") && Mathf.Abs(rigid.linearVelocity.y) < 0.1f)
        {
            rigid.AddForce(Vector2.up * 7f, ForceMode2D.Impulse); // 위쪽으로 힘을 가해 점프
            anim.SetBool("isJumping", true); // 점프 시작
            anim.SetBool("isFalling", false); // 하강 애니메이션 중지
        }

        // 하강 중일 때 (falling)
        if (rigid.linearVelocity.y < 0)
        {
            // 하강 속도 강제로 증가
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, Mathf.Max(rigid.linearVelocity.y, -15f));
            anim.SetBool("isJumping", false); // 점프 상태 종료
            anim.SetBool("isFalling", true); // 하강 애니메이션 계속
        }

        // 착지 확인 (y 속도가 0에 가까워지면 착지)
        if (Mathf.Abs(rigid.linearVelocity.y) < 0.1f)
        {
            anim.SetBool("isJumping", false); // 점프 끝
            anim.SetBool("isFalling", false); // 하강 끝
        }
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");

        // velocity 직접 조절 (즉각적인 반응)
        rigid.linearVelocity = new Vector2(h * maxSpeed, rigid.linearVelocity.y);
    }

}

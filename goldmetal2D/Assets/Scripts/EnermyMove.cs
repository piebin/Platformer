using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnermyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;
    Animator anim;
    SpriteRenderer spriteR;
    CapsuleCollider2D cpCollider;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteR = GetComponent<SpriteRenderer>();
        cpCollider = GetComponent<CapsuleCollider2D>();
        Invoke("Think", 1);
        //게임이 시작된 후 1초 뒤에 enermy가 생각을 시작함.
        
    }

    void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //Platform check

        Vector2 frontVec = new Vector2(rigid.position.x + nextMove*0.3f, rigid.position.y);
        //물체의 한칸 앞 벡터

        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));

        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        //물체의 한칸 앞에서 아랫방향으로 레이 쏴서 플랫폼 있는지 확인.

        if (rayHit.collider == null)
        {
            Turn();
        }
        //플랫폼 없으면 -> 낭떠러지면 turn함수 실행.
    }

    //enermy think
    void Think()
    {
        nextMove = Random.Range(-1, 2);
        //다음 이동 방향을 -1부터 2중 랜덤수로 받음.
        float ThinkTime = Random.Range(2f, 5f);
        //다음 생각을 위한 시간도 랜덤으로 받음.

        Invoke("Think", ThinkTime);
        anim.SetInteger("WalkSpeed", nextMove);
        //walkspeed변수를 랜덤으로 받은 nextMove로 설정함.

        if(nextMove!=0)
            spriteR.flipX = nextMove == 1;
        //nextMove가 1이라면 즉, 오른방향이라면 flipX는 참이 된다. ->원 방향이 왼쪽이므로 flipX를 참으로 하면 오른방향을 바라보게됨.
    }

    //enermy turn
    void Turn()
    {
        nextMove *= -1;
        //방향을 반대로 바꿔줌
        spriteR.flipX = nextMove == 1;
        CancelInvoke();
        Invoke("Think", 5);
        //다음 think함수를 실행하기 위한 시간을 중지하고 다시 invoke함.
    }

    //enermy damaged
    public void OnDamaged()
    {
        spriteR.color = new Color(1, 1, 1, 0.4f);
        spriteR.flipY = true;
        cpCollider.enabled = false;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        Invoke("DeActive", 5);
    }

    //DeActive->오브젝트 비활성화
    void DeActive()
    {
        gameObject.SetActive(false);
    }
}

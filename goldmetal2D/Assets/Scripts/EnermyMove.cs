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
        //������ ���۵� �� 1�� �ڿ� enermy�� ������ ������.
        
    }

    void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //Platform check

        Vector2 frontVec = new Vector2(rigid.position.x + nextMove*0.3f, rigid.position.y);
        //��ü�� ��ĭ �� ����

        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));

        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        //��ü�� ��ĭ �տ��� �Ʒ��������� ���� ���� �÷��� �ִ��� Ȯ��.

        if (rayHit.collider == null)
        {
            Turn();
        }
        //�÷��� ������ -> ���������� turn�Լ� ����.
    }

    //enermy think
    void Think()
    {
        nextMove = Random.Range(-1, 2);
        //���� �̵� ������ -1���� 2�� �������� ����.
        float ThinkTime = Random.Range(2f, 5f);
        //���� ������ ���� �ð��� �������� ����.

        Invoke("Think", ThinkTime);
        anim.SetInteger("WalkSpeed", nextMove);
        //walkspeed������ �������� ���� nextMove�� ������.

        if(nextMove!=0)
            spriteR.flipX = nextMove == 1;
        //nextMove�� 1�̶�� ��, ���������̶�� flipX�� ���� �ȴ�. ->�� ������ �����̹Ƿ� flipX�� ������ �ϸ� ���������� �ٶ󺸰Ե�.
    }

    //enermy turn
    void Turn()
    {
        nextMove *= -1;
        //������ �ݴ�� �ٲ���
        spriteR.flipX = nextMove == 1;
        CancelInvoke();
        Invoke("Think", 5);
        //���� think�Լ��� �����ϱ� ���� �ð��� �����ϰ� �ٽ� invoke��.
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

    //DeActive->������Ʈ ��Ȱ��ȭ
    void DeActive()
    {
        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxS;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteR;
    Animator anim;
    public GameManager gm;
    BoxCollider2D boxCollider;

    public AudioClip AudioJump;
    public AudioClip AudioDie;
    public AudioClip AudioAttack;
    public AudioClip AudioDamaged;
    public AudioClip AudioItem;
    public AudioClip AudioFinish;

    AudioSource audioSource;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteR = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        boxCollider = GetComponent<BoxCollider2D>();
        //사용할거 초기화는 필수. 
    }

    //상황별 오디오
    void playSound(string action)
    {
        switch(action)
        {
            case "JUMP":
                audioSource.clip = AudioJump;
                break;
            case "ATTACK":
                audioSource.clip = AudioAttack;
                break;
            case "DIE":
                audioSource.clip = AudioDie;
                break;
            case "DAMAGED":
                audioSource.clip = AudioDamaged;
                break;
            case "ITEM":
                audioSource.clip = AudioItem;
                break;
            case "FINISH":
                audioSource.clip = AudioFinish;
                break;
        }
        audioSource.Play();  

    }


    void Update()
    {
        //멈출때 속도
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f,
                rigid.velocity.y);
        }

        //점프
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);

            playSound("JUMP");
        }
        


        //캐릭터 좌우 방향 변경
        if (Input.GetButton("Horizontal"))
            spriteR.flipX = Input.GetAxisRaw("Horizontal") == -1;

        //캐릭터 애니메이션 변경
        if (Mathf.Abs(rigid.velocity.x) < 0.5)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);
    }

    void FixedUpdate()
    {
        //캐릭터 이동
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //최대 이동속도
        //우
        if (rigid.velocity.x > maxS)
            rigid.velocity = new Vector2(maxS, rigid.velocity.y);
        //좌
        else if (rigid.velocity.x < maxS*(-1))
            rigid.velocity = new Vector2(maxS*(-1), rigid.velocity.y);

        //ray
        if(rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(1, 0, 0));

                    RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            //내 위치에서 아래방향으로 레이 쏴서 플랫폼 있는지 확인

                    if(rayHit.collider != null)
                    {
                        if(rayHit.distance < 0.5f)
                            anim.SetBool("isJumping", false);
                        //플랫폼과의 거리가 가까우면 jump상태가 아님.
                    }
        }
        
    }

    //몬스터와의 충돌
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enermy")
        {
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
            }else
                OnDamaged(collision.transform.position);
        }
    }

    //몬스터 외 다른 오브젝트와의 충돌
    void OnTriggerEnter2D(Collider2D collision)
    {
        //Item
        if (collision.gameObject.tag == "Item")
        {
            bool Isbronze = collision.gameObject.name.Contains("Bonrze");
            bool Isgold = collision.gameObject.name.Contains("Gold");
            bool Issilver = collision.gameObject.name.Contains("Silver");

            if (Isbronze)
                gm.stagePoint += 10;
            else if (Issilver)
                gm.stagePoint += 50;
            else if (Isgold)
                gm.stagePoint += 100;

            playSound("ITEM");
            collision.gameObject.SetActive(false);
        }
        //Finish
        else if(collision.gameObject.tag == "Finish")
        {
            playSound("FINISH");
            gm.NextStage();
        }
            
    }

    //Attack
    void OnAttack(Transform enermy)
    {
        
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        gm.stagePoint += 200;


        EnermyMove enermyMove = enermy.GetComponent<EnermyMove>();
        enermyMove.OnDamaged();
        //enermyMove의 onDamged함수를 실행시킴.

        playSound("ATTACK");
    }

    //Damaged
    void OnDamaged(Vector2 targetPos)
    {
        gm.HealthDown();
        //gameManager의 healthDown함수 실행.

        gameObject.layer = 11;
        spriteR.color = new Color(1, 0, 0, 1);
        //플레이어의 레이어를 damaged레이어로 바꿈.->무적상태.

        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1)*7, ForceMode2D.Impulse);

        playSound("DAMAGED");

        anim.SetTrigger("doDamaged");
        //doDamaged트리거를 셋함->damaged애니메이션 실행위해.

        Invoke("OffDamaged", 3);
        //OffDamaged함수를 3초 뒤 실행함.


    }
    //damaged상태가 끝난 후
    void OffDamaged()
    {
        gameObject.layer = 10;
        spriteR.color = new Color(1, 1, 1, 1);
    }

    //Die
    public void OnDie()
    {
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        spriteR.color = new Color(1, 1, 1, 0.7f);
        spriteR.flipY = true;
        boxCollider.enabled = false;

        playSound("DIE");

    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
        //속도를 0으로 만들어줌.
    }
}

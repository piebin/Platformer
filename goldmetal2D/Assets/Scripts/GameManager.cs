using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI관리
using UnityEngine.SceneManagement; //씬 관리

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] Stages;
    //스테이지 여러개

    public Image[] UIhealth;
    //목숨 나타내는 캐릭터 여러개
    public GameObject UIRestart;
    public Text UIpoint;
    public Text UIstage;

    //UIPoint text
    private void Update()
    {
        UIpoint.text = (totalPoint + stagePoint).ToString();
    }

    //NextStage
    public void NextStage()
    {

        if(stageIndex < Stages.Length-1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            //현재스테이지 비활성화 다음스테이지 활성화
            PlayerReposition();
            //캐릭터는 첫 위치로 이동

            //현재 스테이지 표시
            UIstage.text = "STAGE " + (stageIndex + 1);
        }
        //Finish Stage Clear
        else
        {
            Time.timeScale = 0;
            
            Text btnText = UIRestart.GetComponentInChildren<Text>();
            btnText.text = "Game Clear";
            UIRestart.SetActive(true);
        }

        totalPoint += stagePoint;
        stagePoint = 0;
    }

    // 플레이어랑 충돌할 때
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if(health > 1)
                PlayerReposition();

            HealthDown();
        }
    }
    //플레이어 첫 위치로 이동
    void PlayerReposition()
    {
        player.transform.position = new Vector3(-4.5f, 2.5f, 0);
        player.VelocityZero();
    }

    //HealthDown
    public void HealthDown()
    {
        //목숨 남았을 때. ex)2, 1
        if (health > 1)
        {
            health--;
            UIhealth[health].color = new Color(1, 0, 0, 0.1f);
        }
        //목숨 안남을때. 
        else if(health <= 1)
        {
            player.OnDie();
            UIhealth[health].color = new Color(1, 0, 0, 0.1f);
            UIRestart.SetActive(true);
            

        }
    }
    //Restart
    public void Restart()
    {
        SceneManager.LoadScene(0);
        //0번씬으로 변경.
    }


}

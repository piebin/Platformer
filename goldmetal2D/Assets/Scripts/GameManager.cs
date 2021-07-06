using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] Stages;

    public Image[] UIhealth;
    public GameObject UIRestart;
    public Text UIpoint;
    public Text UIstage;


    private void Update()
    {
        UIpoint.text = (totalPoint + stagePoint).ToString();
    }


    public void NextStage()
    {

        if(stageIndex < Stages.Length-1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIstage.text = "STAGE " + (stageIndex + 1);
        }
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

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if(health > 1)
                PlayerReposition();

            HealthDown();
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(-4.5f, 2.5f, 0);
        player.VelocityZero();
    }
    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
            UIhealth[health].color = new Color(1, 0, 0, 0.1f);
        }
        else if(health <= 1)
        {
            player.OnDie();
            UIhealth[health].color = new Color(1, 0, 0, 0.1f);
            UIRestart.SetActive(true);
            //UI

        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }


}

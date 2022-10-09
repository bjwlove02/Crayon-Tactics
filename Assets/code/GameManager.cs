using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{

    public bool isGameStart = false;
    public bool isGameEnd = false;

    public int time_Limit = 180;
    public int load_time = 3;
    private int current_time;
    public int green_team_score = 0;
    public int purple_team_score = 0;


    public GameObject gameResultPanel;
    public GameObject gameStartPanel;
    public GameObject scoreBoard;
    public GameObject buttons;
    public GameObject players;
  

    public PhotonView PV;

    public Transform[] purple_spawnPos;
    public Transform[] green_spawnPos;

    private GameObject player_objs;

    public Text timeLeft;
    public Text GameResultText;
    public Text gameOverText;
    public Text countDown;
    public Text green_score_text;
    public Text purple_score_text;

    // Start is called before the first frame update
    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
        PV = GetComponent<PhotonView>();
    }
    void Start()
    {
        
        StartCoroutine("GameInit");
    }
    public void Init()
    {
        scoreBoard.SetActive(false);
        gameStartPanel.SetActive(false);
        gameResultPanel.SetActive(false);
        green_team_score = 0;
        purple_team_score = 0;
        current_time = time_Limit;
        //player_objs = GameObject.FindWithTag("Player");
        //player_objs.transform.position = purple_spawnPos[5].position;

    }
    public void PositionInit()
    {
        for (int i = 0; i < green_spawnPos.Length; i++)
        {
            int random_num = Random.Range(i, green_spawnPos.Length);

            Transform tmp = green_spawnPos[random_num];
            green_spawnPos[random_num] = green_spawnPos[i];
            green_spawnPos[i] = tmp;
        }
    }

    IEnumerator GameInit()
    {
        Init();
        green_team_score = 0;
        purple_team_score = 0;
        gameStartPanel.SetActive(true);
        while (load_time > 0)
        {
            yield return new WaitForSeconds(1.0f);
            load_time -= 1;
        }
        GameStart();
    }
    IEnumerator GameProgress()
    {

        gameStartPanel.SetActive(false);
        scoreBoard.SetActive(true);
        while (current_time > 0)
        {
            current_time--;
            yield return new WaitForSeconds(1.0f);
        }
        GameOver();
    }
    public void GameStart()
    {
        isGameEnd = false;
        isGameStart = true;
        StartCoroutine("GameProgress");
    }
    public void GameOver()
    {
        isGameStart = false;
        isGameEnd = true;
        gameResultPanel.SetActive(true);
        gameOverText.text = "게임 종료!!";
        StartCoroutine("calculateScore");
        
    }

    IEnumerator calculateScore()
    {
        GameResultText.text = "";
        buttons.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        
        if (green_team_score == purple_team_score)
        {
            GameResultText.text = "무승부...";
        }
        else if (green_team_score < purple_team_score)
        {
            GameResultText.text = "보라팀 승리!";
        }
        else if (green_team_score > purple_team_score)
        {
            GameResultText.text = "초록팀 승리!";
        }
        yield return new WaitForSeconds(0.5f);
        buttons.SetActive(true);
    }
 
    public void ReturnLobby()
    {
        //SceneManager.LoadScene("LobbyScene");
        PhotonNetwork.LoadLevel("RoomScene");
    }
    public void Restart()
    {
        StartCoroutine("GameInit");
    }

    void Update()
    {
        if (isGameStart == true)
        {
            green_score_text.text = "초록팀 = " + green_team_score.ToString();
            purple_score_text.text = "보라팀 = " + purple_team_score.ToString();
            if (current_time <= 10)
            {
                timeLeft.color = Color.red;
            }
            else
            {
                timeLeft.color = Color.black;
            }
            timeLeft.text = current_time.ToString()+" Seconds Left!";
        }
        else
        {
            timeLeft.text = "";
            countDown.text = "Game Start In "+load_time.ToString() + "!";
        }  
    }
}

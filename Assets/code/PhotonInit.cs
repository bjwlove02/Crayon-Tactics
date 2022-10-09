using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PhotonInit : MonoBehaviourPunCallbacks
{
    #region MainSceneInit
    public static PhotonInit instance;
    public InputField EnterIDIF;
    bool isGameStart = false;
    bool isGameReady = false;
    bool isLogIn = false;
    string playerName = "";
    public string chatMessage;
    Text nick;
    string connectionState;
    public byte maxplayers;

    public PhotonView pv;

    Text connectionInfoText;
    #endregion


    #region LobbySceneCanvas
    [Header("LobbyCanvas")] public GameObject LobbyCanvas;
    public GameObject LoginPanel;
    public GameObject LobbyPanel;
    public GameObject RoomPanelComBine;
    public GameObject RoomBtnPanel;
    public GameObject RoomCreateOKBtn;
    public GameObject PwPanel;
    public GameObject PwOKBtn;
    public GameObject PwErrorLog;
    public GameObject MakeRoomPanel;
    public InputField RoomNameIF;
    public InputField RoomPwIF;
    public InputField EnterRoomPwIF;
    public Button BackToLoginBtn;
    public Button CreateRoomBtn;
    public Button PreviousBtn;
    public Button NextBtn;
    public Toggle PwToggle;
    public bool LockState = false;
    public string privateRoom;
    public Button[] RoomBtn;
    public Button[] RoomPeopleSettingBtn;
    public int hashTableCount;

    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple, roomnumber;
    #endregion


    private void Awake()
    {
       
    }
    private void Start()
    {
        pv = GetComponent<PhotonView>();
        PhotonNetwork.GameVersion = "MyFps 1.0";
        PhotonNetwork.ConnectUsingSettings();

        if (GameObject.Find("Nick") != null)
            nick = GameObject.Find("Nick").GetComponent<Text>();

        if (GameObject.Find("ConnectionInfoText") != null)
            connectionInfoText = GameObject.Find("ConnectionInfoText").GetComponent<Text>();

        connectionState = "마스터 서버에 접속 중...";

        if (connectionInfoText)
            connectionInfoText.text = connectionState;
        DontDestroyOnLoad(gameObject);
        //신 유지 함수
    }

    private void Update()
    {
       
        if (PlayerPrefs.GetInt("LogIn") == 1)
            isLogIn = true;

        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            if (isGameStart == false && isLogIn == true)
            {
                isGameStart = true;

                StartCoroutine(CreatePlayer());
            }
        }
    }

    public static PhotonInit Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(PhotonInit)) as PhotonInit;

                if (instance == null)
                    Debug.Log("no singleton obj");
            }

            return instance;
        }
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected && isGameReady)
        {
            connectionState = "로비에 접속...";
            if (connectionInfoText)
                connectionInfoText.text = connectionState;

            LoginPanel.SetActive(false);
            LobbyPanel.SetActive(true);
            isLogIn = true;

            PhotonNetwork.JoinLobby();
        }
        else
        {
            connectionState = "오프라인 : 마스터 서버와 연결되지 않음\n접속 재시도 중...";
            if (connectionInfoText)
                connectionInfoText.text = connectionState;

            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnconnectedToServer");
        isGameReady = true;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectionState = "No Room";
        if (connectionInfoText)
            connectionInfoText.text = connectionState;

        Debug.Log("No Room");
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        connectionState = "Finish make a room";
        if (connectionInfoText)
            connectionInfoText.text = connectionState;

        Debug.Log("Finish make a room");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        connectionState = "Joined Room";
        if (connectionInfoText)
            connectionInfoText.text = connectionState;

        Debug.Log("joined Room");
        PlayerPrefs.SetInt("Login", 1);
        PhotonNetwork.LoadLevel("RoomScene");
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("LogIn", 0);
    }

    IEnumerator CreatePlayer()
    {
        while (!isGameStart)
        {
            yield return new WaitForSeconds(0.3f);
        }
        GameObject networkPlayer = PhotonNetwork.Instantiate("My_Player", new Vector3(5, 0, 0), Quaternion.identity, 0);//.transform.parent = GameObject.Find("Map1").transform;
        networkPlayer.GetComponent<PlayerCtrl>().SetPlayerName(playerName);
        pv = GetComponent<PhotonView>();
        yield return null;
    }

    private void OnGUI()
    {
        GUILayout.Label(connectionState);
    }

    public void SetPlayerName()
    {
        Debug.Log(EnterIDIF.text + "를 입력 하셨습니다.");

        Connect();
        PhotonNetwork.LocalPlayer.NickName = EnterIDIF.text;
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);
        EnterIDIF.text = string.Empty;

    }

    [PunRPC]
    #region 룸 생성 및 접속 관련
    public void CreateRoomBtnOnClick()
    {
        MakeRoomPanel.SetActive(true);
    }

    public void OKBtnOnClick()
    {
        MakeRoomPanel.SetActive(false);
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(RoomNameIF.text == "" ? "Game" + Random.Range(0, 100) : RoomNameIF.text);
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
        LobbyPanel.SetActive(false);
        LoginPanel.SetActive(true);
        connectionState = "서버 접속 해제...";
        if (connectionInfoText)
            connectionInfoText.text = connectionState;
        isGameReady = false;
        isGameStart = false;
        isLogIn = false;
        PlayerPrefs.SetInt("LogIn", 0);
    }

    public void SetMaxPlayers2()
    {
        maxplayers = 2;
    }

    public void SetMaxPlayers4()
    {
        maxplayers = 4;
    }

    public void SetMaxPlayers6()
    {
        maxplayers = 6;
    }

    public void CreateNewRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxplayers;
        Debug.Log(roomOptions.MaxPlayers);

        roomOptions.CustomRoomProperties = new Hashtable()
        {
            { "password", RoomPwIF.text}
        };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "password" };

        PhotonNetwork.CreateRoom(RoomNameIF.text == "" ? "Game" + Random.Range(0, 100) : RoomNameIF.text, roomOptions);

        MakeRoomPanel.SetActive(false);
    }

    public void RoomListClick(int num)
    {
        if (num == -2)
        {
            --currentPage;
            RoomListRenewal();
        }
        else if (num == -1)
        {
            ++currentPage;
            RoomListRenewal();
        }
        else if (myList[multiple + num].CustomProperties["password"] != null)
        {
            PwPanel.SetActive(true);
            if (RoomPwIF.interactable == false)
            {
                PwPanel.SetActive(false);
            }
        }
        else
        {
            PhotonNetwork.JoinRoom(myList[multiple + num].Name);
            RoomListRenewal();
        }
    }

    public void RoomPw(int number)
    {
        switch (number)
        {
            case 0:
                roomnumber = 0;
                break;
            case 1:
                roomnumber = 1;
                break;
            case 2:
                roomnumber = 2;
                break;
            case 3:
                roomnumber = 3;
                break;
            default:
                break;
        }
    }

    public void EnterRoomWithPw()
    {
        if ((string)myList[multiple + roomnumber].CustomProperties["password"] == EnterRoomPwIF.text)
        {
            PhotonNetwork.JoinRoom(myList[multiple + roomnumber].Name);
            RoomListRenewal();
            PwPanel.SetActive(false);
        }
        else
        {
            StartCoroutine("ShowPwWrongMsg");
        }
    }

    IEnumerator ShowPwWrongMsg()
    {
        if (!PwErrorLog.activeSelf)
        {
            PwErrorLog.SetActive(true);
            yield return new WaitForSeconds(3.0f);
            PwErrorLog.SetActive(false);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate : " + roomList.Count);
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!myList.Contains(roomList[i]))
                    myList.Add(roomList[i]);
                else
                    myList[myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myList.IndexOf(roomList[i]) != -1)
                myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        RoomListRenewal();
    }
    void RoomListRenewal()
    {
        maxPage = (myList.Count % RoomBtn.Length == 0) ? myList.Count / RoomBtn.Length : myList.Count / RoomBtn.Length + 1;

        PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        multiple = (currentPage - 1) * RoomBtn.Length;
        for (int i = 0; i < RoomBtn.Length; i++)
        {
            RoomBtn[i].transform.GetChild(0).GetComponent<Text>().text
                = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
        }
    }
    #endregion
}

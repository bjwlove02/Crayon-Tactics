using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    bool isGameReady = false;
    bool isHost = false;

    #region RoomSceneCanvas
    [Header("RoomCanvas")] public GameObject RoomCanvas;
    public GameObject RoomPanel;
    public GameObject TeamPanel;
    public GameObject TeamPanelCombine;
    public Button GameStartBtn;
    public Button TeamChangeBtn;
    public Button RoomExitBtn;
    private PhotonView PV;
    public InputField[] GreenTeams;
    public InputField[] PurpleTeams;
    #endregion

    private void Awake()
    {

    }
    private void Start()
    {
        PV = GetComponent<PhotonView>();
        InputNickName();
    }

    private void Update()
    {
        SetRoomPanel();
    }

    public void InputNickName()
    {
        int index = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Debug.Log(player.NickName);
            if (index % 2 == 0)
                GreenTeams[index / 2].text = player.NickName;
            else
                PurpleTeams[(index - 1) / 2].text = player.NickName;
            index++;
        }
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("LobbyScene");
    }

    public void CheckRoomPlayers()
    {
        if (PhotonNetwork.PlayerList.Length == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            isGameReady = true;
            GameStartBtn.GetComponent<Button>().interactable = true;
        }
    }

    public void SetRoomPanel()
    {
        if (PhotonNetwork.CurrentRoom.MaxPlayers == 2)
        {
            GreenTeams[1].interactable = false;
            GreenTeams[2].interactable = false;
            PurpleTeams[1].interactable = false;
            PurpleTeams[2].interactable = false;
        }
        else if (PhotonNetwork.CurrentRoom.MaxPlayers == 4)
        {
            GreenTeams[2].interactable = false;
            PurpleTeams[2].interactable = false;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        InputNickName();
    }


    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        InputNickName();
    }

    public void ChangeBtnClicked()
    {
        if (PV.IsMine)
        {
            PV.RPC("OnChangeTeam", RpcTarget.All);
        }
    }

    [PunRPC]
    public void OnChangeTeam()
    {
        string temp = "";
        if (PhotonNetwork.CurrentRoom.MaxPlayers == 2)
        {
            temp = GreenTeams[0].text;
            GreenTeams[0].text = PurpleTeams[0].text;
            PurpleTeams[0].text = temp;
        }
    }

    public void StartBtnClicked()
    {
        if (PV.IsMine)
        {
            PV.RPC("GameStart", RpcTarget.All);
        }
    }

    [PunRPC]
    public void GameStart()
    {
        PhotonNetwork.LoadLevel("MainScene");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;



public class WallCtrl : MonoBehaviourPun
{

    // Start is called before the first frame update

    public BoxCollider box_collider;
    public PlayerCtrl playerCtrl;
    public GameObject player;
    GameManager gameManager;
    public TeamType currentType;
    public Sprite[] _sprites;
    public SpriteRenderer spriteRenderer;
    bool istrigger = false;
    PhotonView PV;

    void Start()
    {
        
        gameManager = GameObject.Find("GameObject").GetComponent<GameManager>();
        PV = gameManager.PV;
        istrigger = this.GetComponent<BoxCollider>().isTrigger;
        playerCtrl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
        playerCtrl = player.GetComponent<PlayerCtrl>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckType();
        if (playerCtrl.team_Type == currentType)
        {
            istrigger = true;
        }
        else
        {
            istrigger = false;
        }
    }

    public void CheckType()
    {
        switch (currentType)
        {
            case TeamType.GREEN:
                spriteRenderer.sprite = _sprites[1];

                break;
            case TeamType.PURPLE:
                spriteRenderer.sprite = _sprites[0];
                break;
        }
    }
    public void SetType(TeamType type_set)
    {
        currentType = type_set;
    }
    public void ChangeType()
    {
        if (currentType == TeamType.GREEN)
        {
            SetType(TeamType.PURPLE);
        }
        else if (currentType == TeamType.PURPLE)
        {
            SetType(TeamType.GREEN);
        }

    }
}

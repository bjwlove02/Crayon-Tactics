using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public enum TeamType
{
    GREEN=0,
    PURPLE=1
}

public class PlayerCtrl : MonoBehaviour, IPunObservable
{
    private float h = 0f;
    private float v = 0f;
    private float angle;

   
    private bool isHold;
    private bool key_interaction;

    private bool triggerTemp;
    private Rigidbody this_rigidbody;
    private Transform this_tr;
    private Transform grabTr;
    private Transform angleTransform;

    public Sprite[] _sprites;
    public SpriteRenderer spriteRenderer;
    public TeamType team_Type;
    public float speed = 10f;

    public Transform[] spawnSpot;
    
    private PhotonView photonView;

   
    private GameManager gameManager;
    private GameObject triggerd_Item;
    //public Text playerName;
    public TopVeiwCam mainCamera;
    public string name = "";

    [PunRPC]
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        angleTransform = transform.Find("PlayerSprite_preset").GetComponent<Transform>();
        grabTr = transform.Find("PlayerSprite_preset").transform.Find("Catcher").transform.Find("GrabPosition").GetComponent<Transform>();

        
        this_tr = GetComponent<Transform>();
        photonView = GetComponent<PhotonView>();
        this_rigidbody = GetComponent<Rigidbody>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<TopVeiwCam>();

        mainCamera.targetTr =this_tr;
    }
    public void CheckType()
    {

        switch (team_Type)
        {
            case TeamType.GREEN:
                spriteRenderer.sprite = _sprites[1];

                break;
            case TeamType.PURPLE:
                spriteRenderer.sprite = _sprites[0];
                break;
        }

    }
    void Update()
    {
        
        if (photonView.IsMine&&gameManager.isGameStart == true)
        {
            RigidbodyInput();
            InputHandler();
           
        }
        else
        {
            this_rigidbody.velocity = Vector3.zero;
        }
        CheckType();
    }
    private void LateUpdate()
    {
        CarryItem();
    }
    public void CarryItem()
    {
        if (isHold)
        {
            if (triggerd_Item)
            {
                triggerd_Item.transform.position = grabTr.position;
            }
        }
       
    }


    public void InputHandler()
    {
        
        if (Input.GetKey(KeyCode.H))
        {
            key_interaction = true;
            isHold = true;
        }
        else
        {
            key_interaction = false;
            isHold = false;
        }
    }
    public void SetPlayerName(string name)
    {
        this.name = name;
        //playerName.text = this.name;
    }
    void TransformInput()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        if (h != 0 || v != 0)
        {
            Vector3 movedir = (Vector3.forward * v) + (Vector3.right * h);
            this_tr.Translate(movedir * speed * Time.deltaTime, Space.Self);
            angle = Mathf.Atan2(h, v) * Mathf.Rad2Deg;
            
            angleTransform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            
        }
    }

    public void RigidbodyInput()
    {
  
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        Vector3 accelerate;
        accelerate = new Vector3(h, 0, v);

        if (this_rigidbody.velocity.magnitude >= speed)
        {
            this_rigidbody.velocity = accelerate.normalized * speed;
        }
        else
        {
            this_rigidbody.velocity = accelerate * speed;
        }
        
        if (this_rigidbody.velocity.magnitude != 0)
        {
            angle = Mathf.Atan2(this_rigidbody.velocity.x, this_rigidbody.velocity.z) * Mathf.Rad2Deg;
            angleTransform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item"&&other.GetComponent<ItemCtrl>().itemtype==team_Type )
        {
            if (isHold == true&&triggerTemp==false)
            {
                triggerd_Item = other.gameObject;
                triggerTemp = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == triggerd_Item)
        {
            triggerd_Item = null;
            triggerTemp = false;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this_tr.position);
            stream.SendNext(this_tr.rotation);
            stream.SendNext(name);
        }
        else
        {
            //currentPos = (Vector3)stream.ReceiveNext();
            //currentRot = (Quaternion)stream.ReceiveNext();
            SetPlayerName((string)stream.ReceiveNext());
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCtrl : MonoBehaviour
{
    public TeamType itemtype;
    // Start is called before the first frame update
    private GameManager gameManager;
    public Sprite[] _sprites;
    public SpriteRenderer spriteRenderer;
    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
       
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    public void CheckType()
    {
       
            if (itemtype == TeamType.GREEN)
            {
                spriteRenderer.sprite = _sprites[1];
            }
            else if (itemtype == TeamType.PURPLE)
            {
                spriteRenderer.sprite = _sprites[0];
            }
        
    }
    void Update()
    {
        CheckType();
      
    }
    public void ChangeType()
    {
        if (itemtype == TeamType.GREEN)
        {
            itemtype = TeamType.PURPLE;
        }
        else if (itemtype == TeamType.PURPLE)
        {
            itemtype = TeamType.GREEN;
        }
        CheckType();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "GreenCollider")
        {
            if (itemtype == TeamType.GREEN)
            {
                gameManager.green_team_score++;
            }
        }
        else if (other.tag == "PurpleCollider")
        {
            if (itemtype == TeamType.PURPLE)
            {
                gameManager.purple_team_score++;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "GreenCollider")
        {
            if (itemtype == TeamType.GREEN)
            {
                gameManager.green_team_score--;
            }
        }
        else if (other.tag == "PurpleCollider")
        {
            if (itemtype == TeamType.PURPLE)
            {
                gameManager.purple_team_score--;
            }
        }
    }
    public bool GreenCoin()
    {
        return true;
    }
    public bool PurpleCoin()
    {
        return true;
    }
  
}

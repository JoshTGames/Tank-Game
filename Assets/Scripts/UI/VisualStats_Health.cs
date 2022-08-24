using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PopEm.Multiplayer.Lobby;
using UnityEngine.UI;
using JoshsAI;

public class VisualStats_Health : MonoBehaviour{
    [SerializeField] float smoothingSpeed;
    float vel;

    [SerializeField] Image thisImage;
    [SerializeField] SpiderMechAI ai;
    [SerializeField] PLR_MOVE_SINGLEPLR player;
    [SerializeField] Player_Movement multiplayerPLR;

    public float fillAmt = 0f;
    
    bool isOwned = false;

    [Tooltip("Every x frames, this will reset to 0 again")][Range(0,10)][SerializeField] int frameOffset;
    int updateOffsetIndex;
    float percent = 100;
    void Update(){
        updateOffsetIndex = (updateOffsetIndex + 1) % frameOffset;
        switch(updateOffsetIndex){
            case 0:
                if(!ai && !player && !multiplayerPLR){
                    Transform target = transform.parent.GetComponent<VisualStats_Follower>().target;
                    if (target){
                        ai = target?.GetComponent<SpiderMechAI>();
                        player = target?.GetComponent<PLR_MOVE_SINGLEPLR>();
                        multiplayerPLR = target?.GetComponent<Player_Movement>();
                        isOwned = true;
                    }            
                }
                break;
            case 1:
                if (ai) { percent = (ai.GetHealth() / ai.healthSettings.maxHealth); }
                else if (player) { percent = (player.health / player.maxHealth); }
                else if (multiplayerPLR) { percent = (multiplayerPLR.health / multiplayerPLR.maxHealth); }
                break;
            case 2:
                if(isOwned && !(ai || player || multiplayerPLR)){ Destroy(transform.parent.gameObject); }
                break;
        }
       
        fillAmt = Mathf.SmoothDamp(thisImage.fillAmount, percent, ref vel, smoothingSpeed);
        thisImage.fillAmount = fillAmt;
    }
}

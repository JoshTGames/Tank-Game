using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PopEm.Multiplayer.Lobby;
using UnityEngine.UI;
using JoshsAI;

public class VisualStats_Reload : MonoBehaviour{
    [SerializeField] float smoothingSpeed;
    float vel;

    [SerializeField] Image thisImage;
    [SerializeField] SpiderMechAI ai;
    [SerializeField] WeaponController player;
    [SerializeField] WeaponControllerMULTI playerMULTI;

    private void Start(){
        if (!player) { GM_UTILITIES.current.UpdateTrackingTarget += UpdateTarget; }
    }

    

    void UpdateTarget(Transform target){
        player = target.GetComponent<WeaponController>();
    }
    
    [Tooltip("Every x frames, this will reset to 0 again")][Range(0,10)][SerializeField] int frameOffset;
    int updateOffsetIndex;
    float percent = 100;
    void Update(){
        updateOffsetIndex = (updateOffsetIndex + 1) % frameOffset;
        switch(updateOffsetIndex){
            case 0:
                if(!ai && !player){
                    Transform target = transform.parent.GetComponent<VisualStats_Follower>().target;
                    if (target){
                        ai = target?.GetComponent<SpiderMechAI>();
                        player = target?.GetComponent<WeaponController>();
                        playerMULTI = target?.GetComponent<WeaponControllerMULTI>();
                    }
                }
                break;
            case 1:
                if (ai) { percent = 1 - (ai.curCooldown / ai.attackCooldown); }
                else if (player){ percent = 1 - (player.cooldown / player.attackCooldown); }
                else if (playerMULTI){ percent = 1 - (playerMULTI.cooldown / playerMULTI.attackCooldown); }
                break;
        }

        thisImage.fillAmount = Mathf.SmoothDamp(thisImage.fillAmount, percent, ref vel, smoothingSpeed);
    }
}

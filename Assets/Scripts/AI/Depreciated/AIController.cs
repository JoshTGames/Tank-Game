using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour{
    [SerializeField] Transform target;
    [SerializeField] float speed, stoppingDistanceFromTarget;

    Vector3 moveVel;    
    [SerializeField] float speedSmoothing, rotateSmoothing;
    float rotateVel;

    [SerializeField] Animator anim;

    float damping, move;
    float vel;
    void Update() {         
        move = Mathf.SmoothDamp(move, Mathf.Abs(Mathf.Round(damping*100)/100), ref vel, .1f);                          
    }


    void FixedUpdate() => Move();

    void Move(){
        Vector2 dirFromAI = (target.position - transform.position).normalized; // IN PERSPECTIVE OF THIS AI
        Vector3 dirFromTarget = (transform.position - target.position); // IN PERSPECTIVE OF THE TARGET
        
        Vector3 stoppingPosition = (target.position + dirFromTarget.normalized * stoppingDistanceFromTarget);
        
        float distFromTarget = (stoppingPosition - transform.position).magnitude;
        damping = Remap(distFromTarget, stoppingDistanceFromTarget/2, stoppingDistanceFromTarget, -1, 1);

        Vector3 targetPos = transform.position + transform.right * (dirFromAI.magnitude * speed * Time.fixedDeltaTime) * damping;

        if (dirFromAI != Vector2.zero){
            float angle = Mathf.Atan2(dirFromAI.y, dirFromAI.x) * Mathf.Rad2Deg;
            angle = Mathf.SmoothDampAngle(transform.rotation.eulerAngles.z, angle, ref rotateVel, rotateSmoothing);
            Quaternion newRot = Quaternion.AngleAxis(angle, Vector3.forward);

            transform.rotation = newRot;
        }
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref moveVel, speedSmoothing);        

        damping = Mathf.Abs(Mathf.Round(damping*100)/100);
        anim.SetFloat("Move", damping);        
    }



    float Remap(float inputValue, float fromMin, float fromMax, float toMin, float toMax){
        float i = (((inputValue - fromMin) / (fromMax - fromMin)) * (toMax - toMin) + toMin);
        i = Mathf.Clamp(i, toMin, toMax);
        return i;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(target.position, stoppingDistanceFromTarget*1.5f);
    }
}

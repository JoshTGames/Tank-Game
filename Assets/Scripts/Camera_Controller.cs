using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour {
    public Transform target;

    [SerializeField] float speedSmoothing;
    Vector3 moveVelocity;

    Camera cam;

    [SerializeField] float[] clampBoundaries;
    float camRadiusLengthHorizontalView, camRadiusLengthVerticalView;

    private void Start() {
        cam = GetComponent<Camera>();
        #region CALCULATE CAMERA RADIUS
        Vector2 centerView = cam.ViewportToWorldPoint(new Vector3(.5f, .5f, cam.nearClipPlane));
        Vector2 bottomLeftView = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, cam.nearClipPlane));
        Vector2 bottomView = cam.ViewportToWorldPoint(new Vector3(0.5f, 0f, cam.nearClipPlane));

        camRadiusLengthHorizontalView = Mathf.Abs((bottomLeftView - centerView).magnitude);
        camRadiusLengthVerticalView = Mathf.Abs((bottomView - centerView).magnitude);
        #endregion

        GM_UTILITIES.current.UpdateBoundaries += UpdateBoundaries;
        GM_UTILITIES.current.UpdateTrackingTarget += UpdateTarget;
    }

    Vector3 prvPos;
    Vector3 targetPos = new Vector3(0, 0, -10f);

    [Tooltip("Every x frames, this will reset to 0 again")][Range(0,10)][SerializeField] int frameOffset;

    int lateUpdateOffsetIndex;
    Vector3 newPos;
    private void LateUpdate(){
        if (!target) { return; }

        lateUpdateOffsetIndex = (lateUpdateOffsetIndex + 1) % frameOffset;        
        switch(lateUpdateOffsetIndex){
            case 0:
                targetPos = new Vector3(target.position.x, target.position.y, transform.position.z); 
                newPos = GetPos();
                break;           
        }  
        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref moveVelocity, speedSmoothing);
    }

    Vector3 GetPos(){
        return (target) ? new Vector3(
            Mathf.Clamp(targetPos.x, (clampBoundaries[0] + camRadiusLengthHorizontalView) -.5f, (clampBoundaries[1] - camRadiusLengthHorizontalView) +.5f),
            Mathf.Clamp(targetPos.y, (clampBoundaries[2] + camRadiusLengthVerticalView) -.5f, (clampBoundaries[3] - camRadiusLengthVerticalView) +.5f),
            targetPos.z
        ) : new Vector3(
                (clampBoundaries[0] + camRadiusLengthHorizontalView) - .5f,
                (clampBoundaries[2] + camRadiusLengthVerticalView) - .5f,
                targetPos.z
        );
    }

    void UpdateTarget(Transform _target) {         
        target = _target;
        targetPos = new Vector3(target.position.x, target.position.y, transform.position.z);  
        transform.position = targetPos;
        transform.position = GetPos(); 
    }
    public void UpdateBoundaries(float[] boundaries){        
        clampBoundaries = boundaries;               
        transform.position = GetPos();
    }
    int fixedUpdateOffsetIndex;
    private void FixedUpdate(){ 
        fixedUpdateOffsetIndex = (fixedUpdateOffsetIndex + 1) % frameOffset;
        switch(fixedUpdateOffsetIndex){
            case 1:
                prvPos = transform.position; 
                break;
        }        
    }
}

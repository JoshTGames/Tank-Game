using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthHit : MonoBehaviour{
    public VisualStats_Health hp;
    Image panel;
    private void Start() {
        panel = GetComponent<Image>();
    }
    void Update(){
        panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, 1 - hp.fillAmt);
    }
}

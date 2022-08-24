using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.SceneManagement;
[Serializable] public class Pause : UnityEvent<bool> { }
[Serializable] public class CloseOnPlay : UnityEvent { }
[Serializable] public class ApplySettings : UnityEvent { }


public class PauseMenuHandler : MonoBehaviour{    
    [SerializeField] Pause pause;
    [SerializeField] CloseOnPlay closeOnPlay;
    [SerializeField] ApplySettings applySettings;
    public static bool isPaused = false, hasSettingsChanged = false;    
    
    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)){
            isPaused = !isPaused;
            pause?.Invoke(isPaused);
            if (!isPaused) { closeOnPlay?.Invoke(); }
            if(hasSettingsChanged){ applySettings?.Invoke(); }
        }
        Time.timeScale = (isPaused && SceneManager.GetActiveScene().name != "TankGame") ? 0 : 1;
    }

    public void SetPaused(bool _isPaused){ isPaused = _isPaused; }
}

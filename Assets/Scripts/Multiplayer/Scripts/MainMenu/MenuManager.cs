using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PopEm.Multiplayer.Lobby{
    public class MenuManager : MonoBehaviour{
        [SerializeField] NetworkManagerLobby networkManager;
        [Header("UI")] [SerializeField] GameObject landingPanel;

        public void HostLobby(){
            networkManager.StartHost();
            landingPanel.SetActive(false);
        }
        public void LoadScene(string sceneName){
            SceneManager.LoadScene(sceneName);
            PauseMenuHandler.isPaused = false;
            if(landingPanel){ landingPanel.SetActive(false); }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace PopEm.Multiplayer.Lobby{
    public class JoinMenu : MonoBehaviour{
        [SerializeField] NetworkManagerLobby networkManager;
        [Header("UI")] [SerializeField] GameObject landingPanel;
        [SerializeField] TMP_InputField inputField;
        [SerializeField] Button confirmButton;

        private void Start() => SetupInputField();
        string playerPrefsIpKey = "IP";
        void SetupInputField(){
            if (!PlayerPrefs.HasKey(playerPrefsIpKey)){
                inputField.text = "localhost";
                return;
            }
            string savedName = PlayerPrefs.GetString(playerPrefsIpKey);

            inputField.text = savedName;
            IsValidIp(savedName);
        }

        #region LISTENERS
        private void OnEnable(){
            NetworkManagerLobby.OnClientConnected += HandleClientConnected;
            NetworkManagerLobby.OnClientDisconnected += HandleClientDisconnected;
        }

        private void OnDisable(){
            NetworkManagerLobby.OnClientConnected -= HandleClientConnected;
            NetworkManagerLobby.OnClientDisconnected -= HandleClientDisconnected;
        }
        #endregion


        public void JoinLobby(){
            string ip = inputField.text;
            networkManager.StartClient();

            confirmButton.interactable = false;
            inputField.interactable = false;
        }

        public void IsValidIp(string ip) => confirmButton.interactable = !string.IsNullOrEmpty(ip);

        void HandleClientConnected(){
            confirmButton.interactable = true;
            inputField.interactable = true;

            gameObject.SetActive(false);
            landingPanel.SetActive(false);
        }

        void HandleClientDisconnected()        {
            confirmButton.interactable = true;
            inputField.interactable = true;
        }
    }
}

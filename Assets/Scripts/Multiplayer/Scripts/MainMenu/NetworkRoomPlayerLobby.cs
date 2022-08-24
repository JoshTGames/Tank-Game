using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System;

namespace PopEm.Multiplayer.Lobby{
    public class NetworkRoomPlayerLobby : NetworkBehaviour {
        [Header("UI")]
        [SerializeField] GameObject lobbyUI;
        [SerializeField] private TMP_Text[] plrNameText, plrReadyText;
        [SerializeField] Button startButton, readyButton;

        [SyncVar(hook = nameof(HandleNameChanged))] public string displayName = "Loading...";
        [SyncVar(hook = nameof(HandleRdyStatChanged))] public bool isReady = false;

        bool isLeader;
        public bool IsLeader {
            set {
                isLeader = value;
                startButton.gameObject.SetActive(value);                
            }
        }

        NetworkManagerLobby room;
        NetworkManagerLobby Room {
            get {
                if (room) { return room; }
                return room = NetworkManager.singleton as NetworkManagerLobby;
            }
        }

        public override void OnStartAuthority(){
            CMDSetName(PlayerNameHandler.DISPLAY_NAME);
            lobbyUI.SetActive(true);
        }

        public override void OnStartClient(){
            Room.roomPlrs.Add(this);
            UpdateDisplay();
        }
        public override void OnStopClient(){
            Room.roomPlrs.Remove(this);
            UpdateDisplay();
        }


        [Command] private void CMDSetName(string name) => displayName = name;
        [Command] public void CMDReadyUp(){
            isReady = !isReady;            
            Room.NotifyPlrsRdyState();
        }
        [Command] public void CMDStartGame(){
            if(Room.roomPlrs[0].connectionToClient != connectionToClient) { return; }
            Room.StartGame();
        }


        public void HandleNameChanged(string oldValue, string newValue) => UpdateDisplay();
        public void HandleRdyStatChanged(bool oldValue, bool newValue) => UpdateDisplay();

        private void UpdateDisplay(){
            if (!hasAuthority){
                foreach (NetworkRoomPlayerLobby plr in Room.roomPlrs){
                    if (plr.hasAuthority){
                        plr.UpdateDisplay();
                        break;
                    }
                }
                return;
            }

            for (int i = 0; i < plrNameText.Length; i++){
                plrNameText[i].text = "Waiting For player(s)...";
                plrReadyText[i].text = string.Empty;
            }
            for (int i = 0; i < Room.roomPlrs.Count; i++){
                plrNameText[i].text = Room.roomPlrs[i].displayName;
                plrReadyText[i].text = Room.roomPlrs[i].isReady ?
                    "<color=green>Ready</color>" :
                    "<color=red>Not Ready</color>"
                ;
            }

            readyButton.GetComponent<Image>().color = (isReady) ? new Color32(0, 255, 0, 255) : new Color32(255, 0, 0, 255);
            readyButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (isReady) ? "Ready" : "Not Ready";            
        }

        public void HandleReady(bool isRdy) => startButton.interactable = (isLeader) ? isRdy : false;
    }
}

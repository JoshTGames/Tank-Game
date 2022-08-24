using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System;

namespace PopEm.Multiplayer.Lobby{
    public class NetworkGamePlayerLobby : NetworkBehaviour {
        [SyncVar()] string displayName = "Loading...";
        
        NetworkManagerLobby room;
        NetworkManagerLobby Room {
            get {
                if (room) { return room; }
                return room = NetworkManager.singleton as NetworkManagerLobby;
            }
        }

        public override void OnStartClient(){
            DontDestroyOnLoad(gameObject);
            Room.gamePlrs.Add(this);
        }
        public override void OnStopClient(){
            Room.gamePlrs.Remove(this);
        }

        [Server] public void SetDisplayName(string displayName) => this.displayName = displayName;        
    }
}

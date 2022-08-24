using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

namespace PopEm.Multiplayer.Lobby{
    public class NetworkManagerLobby : NetworkManager{
        [SerializeField] int minPlayers = 2;
        public int MinPlrs { get { return minPlayers; } }
        [SerializeField] string menuScene = string.Empty;
        [SerializeField] string gameSceneName = string.Empty;

        [Header("Room")] [SerializeField] NetworkRoomPlayerLobby roomPlayerPrefab = null;
        [Header("Game")] [SerializeField] NetworkGamePlayerLobby gamePlrPrefab;
        [SerializeField] GameObject plrSpawnSystem;
        public static event Action OnClientConnected, OnClientDisconnected;
        public static event Action<NetworkConnection> OnServerReadied;

        public List<NetworkRoomPlayerLobby> roomPlrs { get; } = new List<NetworkRoomPlayerLobby>();
        public List<NetworkGamePlayerLobby> gamePlrs { get; } = new List<NetworkGamePlayerLobby>();

        /// <summary>This function will load all the prefabs on the server so all logic can be applied to them</summary>
        public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

        /// <summary>This function will load all the prefabs on the client so all logic can be applied to them</summary>
        public override void OnStartClient(){
            GameObject[] spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

            foreach (GameObject prefab in spawnablePrefabs) { NetworkClient.RegisterPrefab(prefab); }
        }


        #region CLIENT CONNECTIVITY
        public override void OnClientConnect(NetworkConnection conn){
            base.OnClientConnect(conn);
            OnClientConnected?.Invoke();
        }
        public override void OnClientDisconnect(NetworkConnection conn){
            base.OnClientDisconnect(conn);
            OnClientDisconnected?.Invoke();
        }
        #endregion

        #region SERVER CONNECTIVITY
        public override void OnServerConnect(NetworkConnection conn){
            if (numPlayers >= maxConnections){
                conn.Disconnect();
                return;
            }
            if (SceneManager.GetActiveScene().name != menuScene){
                conn.Disconnect();
                return;
            }           
        }

        public override void OnServerDisconnect(NetworkConnection conn){
            if (conn.identity){
                NetworkRoomPlayerLobby plr = conn.identity.GetComponent<NetworkRoomPlayerLobby>();
                roomPlrs.Remove(plr);
                NotifyPlrsRdyState();
            }
            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer() => roomPlrs.Clear();

        public override void OnServerAddPlayer(NetworkConnection conn){
            if (SceneManager.GetActiveScene().name == menuScene){
                bool isLeader = roomPlrs.Count == 0;

                NetworkRoomPlayerLobby roomPlrInstance = Instantiate(roomPlayerPrefab);
                roomPlrInstance.IsLeader = isLeader;

                NetworkServer.AddPlayerForConnection(conn, roomPlrInstance.gameObject);
            }
        }
        #endregion

        public void NotifyPlrsRdyState(){
            foreach (NetworkRoomPlayerLobby plr in roomPlrs){ plr.HandleReady(IsReady()); }
        }

        bool IsReady(){
            if (numPlayers < minPlayers) { return false; }
            foreach (NetworkRoomPlayerLobby plr in roomPlrs){
                if (!plr.isReady) { return false; }
            }
            return true;
        }


        public void StartGame(){
            if(SceneManager.GetActiveScene().name == menuScene){
                if (!IsReady()) { return; }
                ServerChangeScene(gameSceneName);
            }
        }

        public override void ServerChangeScene(string newSceneName){
            if(SceneManager.GetActiveScene().name == menuScene){
                
                for (int i = roomPlrs.Count - 1; i >=0; i--){
                    NetworkConnection conn = roomPlrs[i].connectionToClient;
                    var gamePlrInstance = Instantiate(gamePlrPrefab);
                    
                    gamePlrInstance.SetDisplayName(roomPlrs[i].displayName);

                    NetworkServer.Destroy(conn.identity.gameObject);
                    NetworkServer.ReplacePlayerForConnection(conn, gamePlrInstance.gameObject, true);                    
                }                
            }
            base.ServerChangeScene(newSceneName);
        }

        public override void OnServerSceneChanged(string sceneName){
            if(sceneName.StartsWith(gameSceneName)){ NetworkServer.Spawn(Instantiate(plrSpawnSystem)); }
        }
        
        public override void OnServerReady(NetworkConnection conn){
            base.OnServerReady(conn);
            OnServerReadied?.Invoke(conn);
        }
    }
}

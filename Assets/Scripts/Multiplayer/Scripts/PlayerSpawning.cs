using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

namespace PopEm.Multiplayer.Lobby{
    public class PlayerSpawning : NetworkBehaviour{    
        [SerializeField] GameObject playerPrefab;
        static List<Transform> spawnPoints = new List<Transform>();
        int nextIndex = 0;


        //NEED A FUNCTION TO CALCULATE WHERE TO SPAWN PLAYERS SUCH AS IF UR PLAYING A TEAM GAME

        public static void AddSpawnPoint(Transform obj){
            spawnPoints.Add(obj);
            spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
        }
        public static void RemoveSpawnPoint(Transform obj) => spawnPoints.Remove(obj);

        public override void OnStartServer() => NetworkManagerLobby.OnServerReadied += SpawnPlayer;
        [ServerCallback] private void OnDestroy() => NetworkManagerLobby.OnServerReadied -= SpawnPlayer;

        [Server] public void SpawnPlayer(NetworkConnection conn){            
            Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);
            if(!spawnPoint){ Debug.LogError($"Missing spawn point for player {nextIndex}"); return; }
            
            GameObject playerInstance = Instantiate(playerPrefab, spawnPoints[nextIndex].position, spawnPoints[nextIndex].rotation, GameObject.Find("CharacterFolder").transform);
            NetworkServer.Spawn(playerInstance, conn);            
            nextIndex++;
        }
    }
}
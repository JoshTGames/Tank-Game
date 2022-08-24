using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopEm.Multiplayer.Lobby{
    public class SpawnPoint : MonoBehaviour{
        private void Awake() => PlayerSpawning.AddSpawnPoint(transform);
        private void OnDestroy() => PlayerSpawning.RemoveSpawnPoint(transform);
    }    
}

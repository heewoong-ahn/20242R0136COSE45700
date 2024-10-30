using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;

        public void PlayerJoined(PlayerRef player)
        {
            if (player == Runner.LocalPlayer)
            {

            Quaternion spawnRotation = Quaternion.Euler(0, 60, 0); // Y축으로 90도 회전
            Runner.Spawn(PlayerPrefab, new Vector3(-0.8517556f, 2f, -1.216699f), spawnRotation);
        }
        }
}
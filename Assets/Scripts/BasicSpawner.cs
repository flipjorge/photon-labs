using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef PlayerPrefab;
    
    private NetworkRunner _runner;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new();

    async void StartGame(GameMode mode)
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "Test Room",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }
    
    private void OnGUI()
    {
        if (_runner) return;
        
        if (GUI.Button(new Rect(0,0,200,40), "Host"))
        {
            StartGame(GameMode.Host);
        }
        if (GUI.Button(new Rect(0,40,200,40), "Join"))
        {
            StartGame(GameMode.Client);
        }
    }
    
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        //
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        //
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            var spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.PlayerCount) * 3, 1, 0);
            var networkPlayerObject = runner.Spawn(PlayerPrefab, spawnPosition, Quaternion.identity, player);
            
            _spawnedCharacters.Add(player, networkPlayerObject);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (!_spawnedCharacters.TryGetValue(player, out var networkObject)) return;
        
        runner.Despawn(networkObject);
        _spawnedCharacters.Remove(player);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W))
            data.Direction += Vector3.forward;

        if (Input.GetKey(KeyCode.S))
            data.Direction += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            data.Direction += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            data.Direction += Vector3.right;

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        //
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        //
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        //
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        //
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        //
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        //
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        //
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        //
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        //
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        //
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        //
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        //
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        //
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        //
    }
}
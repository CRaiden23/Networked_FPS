using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{

    public static GameManager singleton;
    
    public MatchSettings matchSettings;

    private void Awake()
    {
        if (singleton != null)
        {
            Debug.LogError("There can only be one GameManager in a scene");
            Destroy(this);
        }
        else
        {
            singleton = this;
        }
    }

    #region PlayerTracking

    private const string PLAYERID_PREFIX = "Player ";
    
    //                        <key,object>
    private static Dictionary<string, PlayerManager> _players = new Dictionary<string, PlayerManager>();

    public static void RegisterPlayer(string netId, PlayerManager player)
    {
        string playerId = PLAYERID_PREFIX + netId;
        _players.Add(playerId, player);
        player.transform.name = playerId;
    }

    public static void DeregisterPlayer(string playerId) => _players.Remove(playerId);
    public static PlayerManager GetPlayer(string playerId) => _players[playerId];  

    #endregion
    

    /*private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(200,200,200,500));
        GUILayout.BeginVertical();
        foreach (var player in _players.Keys)
        {
            GUILayout.Label(player + " - " + _players[player].transform.name);
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }*/
}

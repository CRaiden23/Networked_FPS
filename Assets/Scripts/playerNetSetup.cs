using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerManager))]
public class playerNetSetup : NetworkBehaviour
{
    [SerializeField]
    private List<Behaviour> componentsToDisable = new List<Behaviour>();

    [SerializeField] 
    private string _remoteLayerName = "RemotePlayer";

    [SerializeField] 
    private string _dontDrawLayerName = "DontDraw";

    [SerializeField] 
    private GameObject _playerGFX;

    [SerializeField] 
    private string _playerUISceneName = "UI";
    
    private Camera _sceneCamera;
    
    private void Start()
    {
        if (!isLocalPlayer) // is a remote player
        {
            DisableComponents();
            Util.SetLayerRecursively(gameObject, LayerMask.NameToLayer(_remoteLayerName));
        }
        else // is a local player
        {
            _sceneCamera = Camera.main;

            if (_sceneCamera != null)
                _sceneCamera.gameObject.SetActive(false);
            
            // Disable player GFX for Local
            Util.SetLayerRecursively(_playerGFX, LayerMask.NameToLayer(_dontDrawLayerName));
            
            // Create PlayerUI
            if(!_playerUISceneName.Equals(""))
                SceneManager.LoadScene(_playerUISceneName, LoadSceneMode.Additive); // make this happen in a local function
            else
                Debug.LogError("PlayerNetSetup: No UI Scene found");
        }

        GetComponent<PlayerManager>().Setup();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        string netID = GetComponent<NetworkIdentity>().netId.ToString();
        PlayerManager player = GetComponent<PlayerManager>();
        
        GameManager.RegisterPlayer(netID, player);
    }

    private void OnDisable()
    {
        if (_sceneCamera != null)
            _sceneCamera.gameObject.SetActive(true);

        GameManager.DeregisterPlayer(transform.name);
        SceneManager.UnloadSceneAsync(_playerUISceneName);
    }

    private void AssignRemoteLayer(GameObject obj, int layer) => obj.layer = layer; // set layer to our remote layer
    
    private void DisableComponents()
    {
        foreach (var component in componentsToDisable)
            component.enabled = false;
    }
}

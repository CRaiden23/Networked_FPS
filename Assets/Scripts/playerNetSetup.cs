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
    private string _remoteGunLayerName = "RemoteGun";
    
    [SerializeField] 
    private string _dontDrawLayerName = "DontDraw";

    [SerializeField] 
    private GameObject _playerGFX;

    [SerializeField] 
    private GameObject _playerWeapon;

    [SerializeField] 
    private string _playerUISceneName = "UI";
    
    private Camera _sceneCamera;
    
    private void Start()
    {
        if (!isLocalPlayer) // is a remote player
        {
            DisableComponents();
            AssignRemoteLayer(gameObject, LayerMask.NameToLayer(_remoteLayerName));
            SetLayerRecursively(_playerWeapon, LayerMask.NameToLayer(_remoteGunLayerName)); // this might need to move to PlayerShoot.cs
        }
        else // is a local player
        {
            _sceneCamera = Camera.main;

            if (_sceneCamera != null)
                _sceneCamera.gameObject.SetActive(false);
            
            // Disable player GFX for Local
            SetLayerRecursively(_playerGFX, LayerMask.NameToLayer(_dontDrawLayerName));
            
            // Create PlayerUI
            if(!_playerUISceneName.Equals(""))
                SceneManager.LoadScene(_playerUISceneName, LoadSceneMode.Additive);
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

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}

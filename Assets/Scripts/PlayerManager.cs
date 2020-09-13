using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(playerNetSetup))]
public class PlayerManager : NetworkBehaviour
{
    [SyncVar] // syncVar tells network manager to sync this variable
    private bool _isDead = false;

    [SerializeField]
    public bool IsDead
    {
        get => _isDead;
        protected set => _isDead = value;
    }

    [SerializeField]
    private int maxHealth = 100;
    [SyncVar]
    private int _currentHealth;

    [SerializeField] 
    private Behaviour[] disableOnDeath; // list of components to disable on death
    private bool[] _wasEnabled; // list of components' states prior to death
    private bool firstTimeSetup = true;
    
    [SerializeField] 
    private GameObject[] disableGameObjectsOnDeath;

    [SerializeField] 
    private GameObject _DeathEffect;

    //Setup on spawn
    public void PlayerSetup()
    {
        if (isLocalPlayer)
        {
            // switch to player camera and enable UI
            GameManager.singleton.SetSceneCameraActive(false);
            GetComponent<playerNetSetup>().playerUIInstance.SetActive(true);
        }
        
        CmdBroadcastNewPlayerSetup();
    }

    [Command]
    private void CmdBroadcastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if (firstTimeSetup) // do our first time checks
        {
            _wasEnabled = new bool[disableOnDeath.Length]; // instantiate array to length of components to check
            for (int i = 0; i < _wasEnabled.Length; i++) // take states of components
                _wasEnabled[i] = disableOnDeath[i].enabled;
            
            firstTimeSetup = false;
        }

        SetDefaults();
    }

    [ClientRpc] // (Client RPCs get called on all clients)
    public void RpcTakeDamage(int damage) // this will update the damage on this player for all clients
    {
        if(_isDead)
            return;
        
        _currentHealth -= damage;
        
        Debug.Log(transform.name + " now has " + _currentHealth + " health");

        if (_currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        _isDead = true;

        for (int i = 0; i < disableOnDeath.Length; i++) // disable components
            disableOnDeath[i].enabled = false;
        
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++) // disable GameObjects
            disableGameObjectsOnDeath[i].SetActive(false);
        
        CharacterController cc = GetComponent<CharacterController>(); // disable collider for CC
        if (cc != null)
            cc.detectCollisions = false;

        // instantiate a death effect
        GameObject deathEffect = (GameObject)Instantiate(_DeathEffect, transform.position, Quaternion.identity);
        Destroy(deathEffect, 3f);

        if (isLocalPlayer) // switch to scene camera and disable UI
        {
            GameManager.singleton.SetSceneCameraActive(true);
            GetComponent<playerNetSetup>().playerUIInstance.SetActive(false);
        }
        
        Debug.Log(transform.name + " is DEAD!");

        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
        
        yield return new WaitForSeconds(GameManager.singleton.matchSettings.respawnTime);
        
        PlayerSetup();
        
        Debug.Log(transform.name + " respawned.");
    }
    
    public void SetDefaults()
    {
        _isDead = false;

        _currentHealth = maxHealth;

        for (int i = 0; i < disableOnDeath.Length; i++) // loop through components to their original state
            disableOnDeath[i].enabled = _wasEnabled[i];
        
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++) // disable GameObjects
            disableGameObjectsOnDeath[i].SetActive(true);
        
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
            cc.detectCollisions = true;

        WeaponManager wm = GetComponent<WeaponManager>();
        if(wm != null)
            wm.EquipWeapon(wm._startingWeapon);
    }
}

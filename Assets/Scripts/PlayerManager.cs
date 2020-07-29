using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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

    [SerializeField] private Behaviour[] disableOnDeath; // list of components to disable on death
    private bool[] _wasEnabled; // list of components' states prior to death

    [SerializeField]
    private MeshRenderer _playerGFX;

    private void Awake()
    {
        if(_playerGFX != null)
            _playerGFX = GetComponentInChildren<MeshRenderer>();
    }
        

    //Setup on spawn
    public void Setup()
    {
        _wasEnabled = new bool[disableOnDeath.Length]; // instantiate array to length of components to check
        
        for (int i = 0; i < _wasEnabled.Length; i++) // take states of components
            _wasEnabled[i] = disableOnDeath[i].enabled;
        
        SetDefaults();
    }

    [ClientRpc]
    public void RpcTakeDamage(int damage)
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
        
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
            cc.detectCollisions = false;

        // disable player graphics
        _playerGFX.enabled = false;
        
        // instantiate a corpse
        
        Debug.Log(transform.name + " is DEAD!");

        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
        
        yield return new WaitForSeconds(GameManager.singleton.matchSettings.respawnTime);
        
        SetDefaults();
        
        Debug.Log(transform.name + " respawned.");
    }
    
    public void SetDefaults()
    {
        _isDead = false;
        
        _currentHealth = maxHealth;

        for (int i = 0; i < disableOnDeath.Length; i++) // loop through components to their original state
            disableOnDeath[i].enabled = _wasEnabled[i];
        
        // reenable player graphics
        _playerGFX.enabled = true;

        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
            cc.detectCollisions = true;
    }
}

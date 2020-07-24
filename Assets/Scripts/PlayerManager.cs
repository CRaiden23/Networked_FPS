using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour
{
    [SyncVar]
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

    [SerializeField] private Behaviour[] disableOnDeath;
    private bool[] _wasEnabled;

    public void Setup()
    {
        _wasEnabled = new bool[disableOnDeath.Length];
        
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
        
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
        
        Debug.Log(transform.name + "is DEAD!");

        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.singleton.matchSettings.respawnTime);
        
        SetDefaults();
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
        
        Debug.Log(transform.name + " respawned.");
    }
    
    public void SetDefaults()
    {
        _isDead = false;
        
        _currentHealth = maxHealth;

        for (int i = 0; i < disableOnDeath.Length; i++) // loop through components to their original state
            disableOnDeath[i].enabled = _wasEnabled[i];

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour
{

    private const string PLAYER_TAG = "Player";
    
    [SerializeField] private Camera _cam;
    [SerializeField] private LayerMask _whatCanBeShot;

    public PlayerWeapon weapon;
    // Start is called before the first frame update
    void Start()
    {
        if (_cam == null)
        {
            Debug.LogError("PlayerShoot: No Camera Referenced!");
            this.enabled = false;            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            Shoot();
    }
    
    [Client] // happens only on clients
    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out hit, weapon.range, _whatCanBeShot))
            if (hit.collider.CompareTag(PLAYER_TAG)) // tells the server that a player has been hit
                CmdPlayerShot(hit.collider.name, weapon.damage);
    }

    [Command] // happens only on servers
    void CmdPlayerShot(string playerId, int damage)
    {
        Debug.Log(playerId + " has been shot");

        PlayerManager player = GameManager.GetPlayer(playerId);
        player.RpcTakeDamage(damage); // deal damage to player
    }
}

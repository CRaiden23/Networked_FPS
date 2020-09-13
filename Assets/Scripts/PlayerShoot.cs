using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";
    
    [SerializeField] private Camera _cam;
    [SerializeField] private LayerMask _whatCanBeShot;

    private PlayerWeapon _currentWeapon;
    private WeaponManager _weaponManager;

    // Start is called before the first frame update
    void Start()
    {
        if (_cam == null)
        {
            Debug.LogError("PlayerShoot: No Camera Referenced!");
            this.enabled = false;            
        }

        _weaponManager = GetComponent<WeaponManager>();
    }

    // Update is called once per frame
    void Update()
    {
        _currentWeapon = _weaponManager.GetCurrentWeapon();

        if (Input.GetButtonDown("Reload") && _currentWeapon.currentAmmo != _currentWeapon.maxAmmo)
        {
            _weaponManager.Reload();
            return;
        }
        
        if (_currentWeapon.fireRate <= 0f)
        {
            if (Input.GetButtonDown("Fire1"))
                Shoot();
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
                InvokeRepeating("Shoot", 0f, 1f/_currentWeapon.fireRate);
            else if (Input.GetButtonUp("Fire1"))
                CancelInvoke(nameof(Shoot));
        }
    }

    [Command] // always called on shoot by the server
    void CmdOnShoot() => RpcDoShootEffect();

    [ClientRpc] // called on all clients when doing shoot effect
    void RpcDoShootEffect() => _weaponManager.getCurrentGraphics().muzzleFlash.Play();

    [Command] // called when hitting non player
    void CmdOnHit(Vector3 pos, Vector3 normal) => RpcDoImpactEffect(pos, normal);

    [ClientRpc] // spawns impact effect on normal of object hit
    void RpcDoImpactEffect(Vector3 pos, Vector3 normal)
    {
        GameObject FX = (GameObject)Instantiate(_weaponManager.getCurrentGraphics().hitEffectPrefab, pos, Quaternion.LookRotation(normal));
        Destroy(FX, 5f);
    } 

    [Command] // called when hitting player
    void CmdOnPlayerHit(Vector3 pos, Vector3 normal) => RpcDoPlayerHitEffect(pos, normal);
    
    [ClientRpc] // spawns damage effect when hitting player
    void RpcDoPlayerHitEffect(Vector3 pos, Vector3 normal)
    {
        GameObject FX = (GameObject)Instantiate(_weaponManager.getCurrentGraphics().playerHitEffectPrefab, pos, Quaternion.LookRotation(normal));
        Destroy(FX, 2f);
    }

    [Client] // happens only on clients
    void Shoot()
    {
        if (!isLocalPlayer && !_weaponManager._isReloading) // only local player decides when they shoot
            return;

        if (_currentWeapon.currentAmmo <= 0)
        {
            _weaponManager.Reload();
            return;
        }

        _currentWeapon.currentAmmo--;
        
        CmdOnShoot();

        RaycastHit hit;
        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out hit, _currentWeapon.range,
            _whatCanBeShot))
        {
            if (hit.collider.CompareTag(PLAYER_TAG)) // tells the server that a player has been hit
            {
                CmdPlayerShot(hit.collider.name, _currentWeapon.damage);
                CmdOnPlayerHit(hit.point, hit.normal);
            }
            else if (hit.collider.CompareTag("Ground")) // we hit ground
                CmdOnHit(hit.point, hit.normal);
                
        }
    }

    [Command] // happens only on servers
    void CmdPlayerShot(string playerId, int damage)
    {
        Debug.Log(playerId + " has been shot");

        PlayerManager player = GameManager.GetPlayer(playerId);
        player.RpcTakeDamage(damage); // deal damage to player
    }
}

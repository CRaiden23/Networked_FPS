using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField] 
    public PlayerWeapon _startingWeapon;

    [SyncVar] [SerializeField] private PlayerWeapon _currentWeapon;
    private WeaponGraphics _currentGraphics;
    private GameObject weaponIns;
    
    [SerializeField] 
    private string _remoteGunLayerName = "RemoteGun";

    [SerializeField] private string _pickupTag = "Pickup";

    public bool _isReloading = false;

    private Animator anim;

    [SerializeField]
    private Transform weaponHolder;

    private Camera mainCam;
    
    void Start()
    {
        mainCam = Camera.main;
        EquipWeapon(_startingWeapon);
        anim = weaponHolder.GetComponent<Animator>();
    }

    private void Update()
    {
        Ray ray = mainCam.ViewportPointToRay(new Vector3(.5f, .5f, 0));;
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag(_pickupTag) && Input.GetButtonDown("Interact"))
            {
                EquipWeapon(hit.collider.GetComponent<WeaponPickup>().GetWeapon());                
                Destroy(hit.collider.gameObject);
            }
        }
    }

    public PlayerWeapon GetCurrentWeapon()
    {
        return _currentWeapon;
    }
    
    public WeaponGraphics getCurrentGraphics()
    {
        return _currentGraphics;
    }

    public void EquipWeapon(PlayerWeapon newWeapon)
    {
        if(weaponIns != null)
                    Destroy(weaponIns);
                
        _currentWeapon = newWeapon;

        weaponIns = (GameObject)Instantiate(newWeapon.graphics, weaponHolder.position, weaponHolder.rotation);
        weaponIns.transform.SetParent(weaponHolder);

        _currentGraphics = weaponIns.GetComponentInChildren<WeaponGraphics>();
        
        if(_currentGraphics == null)
            Debug.LogError("WeaponManager: No WeaponGraphics component on the weapon object: " + weaponIns.name);
        
        if (!isLocalPlayer) // if we are a remote player
            Util.SetLayerRecursively(weaponIns, LayerMask.NameToLayer(_remoteGunLayerName));
    }

    public void Reload()
    {
        if (_isReloading)
            return;

        StartCoroutine(Reload_Coroutine());
    }

    private IEnumerator Reload_Coroutine()
    {
        _isReloading = true;
        
        CmdOnReload();
        
        yield return new WaitForSeconds(_currentWeapon.reloadSpeedInSeconds);
        
        anim.SetTrigger("DoneReloading");
        
        _currentWeapon.currentAmmo = _currentWeapon.maxAmmo;
        
        _isReloading = false;
    }

    [Command]
    void CmdOnReload()
    {
        RpcOnReload();
    }

    [ClientRpc]
    void RpcOnReload()
    {
        if (anim != null)
        {
            anim.Play(Animator.StringToHash("ReloadAnim"));
        }
    }
}


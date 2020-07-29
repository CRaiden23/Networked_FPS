using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField] 
    private PlayerWeapon _startingWeapon;

    private PlayerWeapon _currentWeapon;
    private WeaponGraphics _currentGraphics;
    
    [SerializeField] 
    private string _remoteGunLayerName = "RemoteGun";

    [SerializeField]
    private Transform weaponHolder;
    
    void Start()
    {
        EquipWeapon(_startingWeapon);
    }

    public PlayerWeapon GetCurrentWeapon()
    {
        return _currentWeapon;
    }
    
    public WeaponGraphics getCurrentGraphics()
    {
        return _currentGraphics;
    }
    
    void EquipWeapon(PlayerWeapon newWeapon)
    {
        _currentWeapon = newWeapon;

        GameObject weaponIns = (GameObject)Instantiate(newWeapon.graphics, weaponHolder.position, weaponHolder.rotation);
        weaponIns.transform.SetParent(weaponHolder);

        _currentGraphics = weaponIns.GetComponentInChildren<WeaponGraphics>();
        if(_currentGraphics == null)
            Debug.LogError("WeaponManager: No WeaponGraphics component on the weapon object: " + weaponIns.name);
        
        if (!isLocalPlayer) // if we are a remote player
            Util.SetLayerRecursively(weaponIns, LayerMask.NameToLayer(_remoteGunLayerName));
    }
}

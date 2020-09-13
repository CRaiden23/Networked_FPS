using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private Transform _weaponGraphics;
    [SerializeField] private string _pickupLayerName = "Default";
    
    [SerializeField] private bool randomPickup;
    [SerializeField] private PlayerWeapon _weaponInfo;

    private void Start()
    {
        if(randomPickup)
            PickFromLibrary();
        
        GameObject weapon = (GameObject)Instantiate(_weaponInfo.graphics, _weaponGraphics);
        Util.SetLayerRecursively(weapon, LayerMask.NameToLayer(_pickupLayerName));
    }

    private void PickFromLibrary()
    {
        int rand = UnityEngine.Random.Range(0, GameManager.singleton.weaponLibrary.Count);
        
        _weaponInfo = GameManager.singleton.weaponLibrary[rand];
    }

    public PlayerWeapon GetWeapon() => _weaponInfo;
}

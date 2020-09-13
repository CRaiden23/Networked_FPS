using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerWeapon
{
   public string name = "Pistol";

   public int damage = 10;
   public float range = 100f;

   public float fireRate = 0f;
   public int maxAmmo = 8;
   public float reloadSpeedInSeconds = 1f;
   [HideInInspector]
   public int currentAmmo;
   
   public GameObject graphics;

   public PlayerWeapon() => currentAmmo = maxAmmo;
}

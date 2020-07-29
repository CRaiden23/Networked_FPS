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
   public int ammo = 8;
   
   public GameObject graphics;
}

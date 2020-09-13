using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Slider HealthBar;

    private PlayerManager _player;

    public void SetPlayerManager(PlayerManager player)
    {
        _player = player;
    }
    
    // Update is called once per frame
    private void Update()
    {
        SetHealth(_player.GetHealth());
    }

    private void SetHealth(int health)
    {
        HealthBar.value = health;
    }
}

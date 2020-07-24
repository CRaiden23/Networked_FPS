using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{

    public float mouseSensitivity = 100f;
    private float _mouseX;
    private float _mouseY;
    
    [SerializeField]
    private Transform player;

    [SerializeField]
    private float _xRotation = 0f;
    
    void Start()
    {
        if (player == null)
            player = GetComponentInParent<Transform>();
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        _mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        _mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        _xRotation -= _mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        
        transform.localRotation = Quaternion.Euler(_xRotation,0f,0f);
        player.Rotate(Vector3.up * _mouseX);
    }
}

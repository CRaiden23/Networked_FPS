using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    private float _inputX;
    private float _inputZ;
    private Vector3 _velocity;
    
    private CharacterController _controller;
    [SerializeField]
    private Animator _animator;
    
    [SerializeField]
    public Transform _groundCheck;

    public float checkRadius;
    public LayerMask groundMask;
    private bool _isGrounded = false;
    
    public float moveSpeed = 12f;
    public float gravity = -9.8f;
    public float jumpHeight = 2f;
    
    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, checkRadius, groundMask);

        if (_isGrounded && _velocity.y < 0)
            _velocity.y = -2f;

        //Ground Movement
        #region Move
        
        _inputX = Input.GetAxis("Horizontal");
        _inputZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * _inputX + transform.forward * _inputZ;
        
        _controller.Move(move * (moveSpeed * Time.deltaTime));        

        #endregion

        if (Input.GetButtonDown("Jump") && _isGrounded)
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        
        //Gravity
        #region ApplyGravity
        
        _velocity.y += gravity * Time.deltaTime;

        _controller.Move(_velocity * Time.deltaTime);
        
        #endregion
        
        _animator.SetFloat("MoveSpeed", move.magnitude);
    }
}

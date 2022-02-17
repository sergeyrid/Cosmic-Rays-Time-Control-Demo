using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    public float velocity = 10;
    public float angularVelocity = 10;
    private Rigidbody m_Rigidbody;
    private Transform m_Transform;
    private Vector2 m_TurnVector = new Vector2(0, 0);

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Transform = transform;
    }

    private void Update()
    {
        m_Transform.Rotate(Vector3.up, m_TurnVector.x * Time.deltaTime * angularVelocity, Space.World);
        m_Transform.Rotate(Vector3.right, -m_TurnVector.y * Time.deltaTime * angularVelocity, Space.Self);
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        var movementInput = context.ReadValue<Vector3>() * velocity;
        m_Rigidbody.velocity = movementInput.x * m_Transform.right +
                               movementInput.y * m_Transform.up +
                               movementInput.z * m_Transform.forward;
    }

    public void OnTurn(InputAction.CallbackContext context)
    {
        var turnInput = context.ReadValue<Vector2>() * angularVelocity;
        m_TurnVector = turnInput;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] VariableJoystick joystick;

    private void Update()
    {
        Move();
    }

    public void Move()
    {
        float horizontal = joystick.Horizontal * Time.deltaTime/* + Input.GetAxis("Horizontal")*/;
        float vertical = joystick.Vertical * Time.deltaTime /*+ Input.GetAxis("Vertical")*/;

        Vector3 direction = Vector3.right * horizontal * moveSpeed  + Vector3.forward * vertical * moveSpeed;
        transform.position += direction;
        if (direction.magnitude > 0f)  
            Rotate(direction);
    }
    public void Rotate(Vector3 moveDirection)
    {
        float currentYRotation = transform.rotation.eulerAngles.y;
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        targetRotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }
}

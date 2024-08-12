using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody playerRb;
    public Rigidbody trolleyRb;
    public float moveSpeed;
    public float rotateSpeed;
    public float pushForce;
    public bool isPushing = false;
    public VariableJoystick variableJoystick;
    public Animator animator;
    public bool isMoveUsingJoystick;
    public Transform playerHand;
    public Transform trolleyHandle;


    private void FixedUpdate()
    {
        if (Input.touchCount > 0 && !isMoveUsingJoystick)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane + 1f));

            Vector3 movement = new Vector3(touchPosition.x - transform.position.x, 0.0f, touchPosition.z - transform.position.z);
            movement = Vector3.ClampMagnitude(movement, 1.0f);

            

            // Rotate character to face the movement direction
            if (movement != Vector3.zero)
            {
                playerRb.MovePosition(transform.position + movement * moveSpeed * Time.deltaTime);
                Quaternion targetRotation = Quaternion.LookRotation(movement);
                playerRb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f));
                animator.SetBool("isWalking", true);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }
        else if (isMoveUsingJoystick)
        {
            Vector3 direction = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;
            direction.Normalize();

            playerRb.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);
            //transform.Translate(direction * moveSpeed * Time.fixedDeltaTime, Space.World);
            if (direction != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                //transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotateSpeed * Time.fixedDeltaTime);
                playerRb.MoveRotation(Quaternion.Slerp(transform.rotation, toRotation, Time.fixedDeltaTime * 10f));
                animator.SetBool("isWalking", true);
                if (isPushing)
                {
                    animator.SetBool("isPushing", true);
                    trolleyRb.gameObject.transform.SetParent(this.transform);
                }
                else
                {
                    animator.SetBool("isPushing", false);
                    trolleyRb.gameObject.transform.SetParent(null);
                }
            }
            else
            {
                animator.SetBool("isWalking", false);

                animator.SetBool("isPushing", false);
                isPushing = false;
            }

        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Trolley"))
        {
            //trolleyHandle.position = playerHand.transform.position;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Trolley")) //
        {
            isPushing = true;
            if (isPushing)
            {
                
                trolleyRb = collision.gameObject.GetComponent<Rigidbody>();

                Vector3 pushDir = transform.forward;

                trolleyRb.AddForce(pushDir * pushForce, ForceMode.Force);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Trolley"))
        {
            isPushing = false;
        }
    }
}

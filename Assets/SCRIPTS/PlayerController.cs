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
    public VariableJoystick variableJoystick;
    public Animator animator;
    public bool isMoveUsingJoystick;
    public Transform playerHand;
    public Transform trolleyHandle;
    public Transform playerTrolleyHolder;
    public GameObject trolley;
    public float offset;
    public List<Animator> trolleyWheelAnims;

    private Vector3 joyStickDirection;
    public float idleTime = 0f;
    public float detachTime = 5f;

    public bool isHoldingTrolley = false;
    public bool isPushing = false;
    public bool isMoveWithTrolley, isDethached;

    private void FixedUpdate()
    {

        if (isMoveUsingJoystick)
        {
            joyStickDirection = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;
            joyStickDirection.Normalize();

            MovePlayer(joyStickDirection);

            if (joyStickDirection != Vector3.zero )
            {
                isMoveWithTrolley = true;
                foreach (Animator anims in trolleyWheelAnims)
                {
                    anims.enabled = true;
                }
            }
            else
            {
                isMoveWithTrolley = false;
                DetachTrolley();
                foreach(Animator anims in trolleyWheelAnims)
                {
                    anims.enabled = false;
                }
            }
        }
        
        else if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane + 1f));
            Vector3 movement = new Vector3(touchPosition.x - transform.position.x, 0.0f, touchPosition.z - transform.position.z);
            movement = Vector3.ClampMagnitude(movement, 1.0f);

            MovePlayer(movement);
        }

        if (isPushing)
        {
            trolley.transform.position = playerTrolleyHolder.position;
            trolley.transform.rotation = playerTrolleyHolder.rotation;
        }
        if (joyStickDirection == Vector3.zero)
        {
            DetachTrolley();
        }
        else
        {
            idleTime = 0f;
        }
    }

    private void MovePlayer(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            playerRb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            playerRb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime));
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    private void MoveTrolley()
    {
        if (trolleyRb != null)
        {
            Vector3 targetPosition = playerTrolleyHolder.position;
            trolleyRb.MovePosition(targetPosition);
            Quaternion targetRotation = Quaternion.LookRotation(joyStickDirection);
            trolleyRb.MoveRotation(Quaternion.RotateTowards(trolleyRb.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime));
        }
    }

    public void AttachTrolley()
    {
        /*isHoldingTrolley = true;
        isPushing = true;
        trolley.transform.position = playerTrolleyHolder.position;
        trolley.transform.rotation = playerTrolleyHolder.rotation;
        trolley.transform.SetParent(transform);
        animator.SetBool("isPushing", true);*/
        if (!isDethached)
        {
            StartCoroutine(SmoothAttachTrolley());
            animator.SetBool("isPushing", true);
        }
    }

    private IEnumerator SmoothAttachTrolley()
    {
        float elapsedTime = 0f;
        float attachDuration = 0.5f; 
        Vector3 initialPosition = trolley.transform.position;
        Quaternion initialRotation = trolley.transform.rotation;
        trolley.transform.position = Vector3.Lerp(initialPosition, playerTrolleyHolder.position, elapsedTime / attachDuration);
        trolley.transform.rotation = Quaternion.Slerp(initialRotation, playerTrolleyHolder.rotation, elapsedTime / attachDuration);

        while (elapsedTime < attachDuration)
        {
            //trolley.transform.position = Vector3.Lerp(initialPosition, playerTrolleyHolder.position, elapsedTime / attachDuration);
            //trolley.transform.rotation = Quaternion.Slerp(initialRotation, playerTrolleyHolder.rotation, elapsedTime / attachDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        trolley.transform.position = playerTrolleyHolder.position;
    }

    public void DetachTrolley()         
    {
        isHoldingTrolley = false;
        isPushing = false;
        trolley.transform.SetParent(null);
        animator.SetBool("isPushing", false);
        idleTime = 0f;
        moveSpeed = 0;
        trolley.GetComponent<Rigidbody>().velocity = Vector3.zero;
        StartCoroutine(ResetDethacHValue());
    }

    private IEnumerator ResetDethacHValue()
    {
        yield return new WaitForSeconds(0.2f);
        moveSpeed = 2;
        //trolley.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Trolley") && isMoveWithTrolley)
        {
            trolley = collision.gameObject;
            AttachTrolley();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Trolley") && isMoveWithTrolley)
        {
            AttachTrolley();
            isDethached = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Trolley"))
        {
            isPushing = false;
            isDethached = true;    
            animator.SetBool("isPushing", false);
        }
    }
}

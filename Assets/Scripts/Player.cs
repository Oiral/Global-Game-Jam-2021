using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    Rigidbody controller;

    public float playerAcceleration;

    public float maxSpeed;

    private void Start()
    {
        controller = GetComponent<Rigidbody>();
    }

    public Vector3 targetMoveDir;

    public Transform visuals;

    public Animator animator;

    // Update is called once per frame
    void Update()
    {

        targetMoveDir = new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical"));

        if (targetMoveDir.magnitude < 0.2f)
        {
            animator.SetBool("Moving", false);
            return;
        }
        animator.SetBool("Moving", true);

        Debug.Log("Moving");
        targetMoveDir = Vector3.ClampMagnitude(targetMoveDir, 1);

        targetMoveDir = targetMoveDir * playerAcceleration;
        

        visuals.LookAt(transform.position + controller.velocity, Vector3.up);
    }

    private void FixedUpdate()
    {
        Vector3 moveDir = controller.velocity + targetMoveDir;

        moveDir = Vector3.ClampMagnitude(moveDir, maxSpeed);

        controller.velocity = moveDir;
    }
}

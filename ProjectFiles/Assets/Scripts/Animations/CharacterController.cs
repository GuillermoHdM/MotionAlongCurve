using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    Animator animator;

    int velocity_x_hash;
    int velocity_z_hash;

    public Camera camera;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        velocity_x_hash = Animator.StringToHash("velocity_x");
        velocity_z_hash = Animator.StringToHash("velocity_z");

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            move += (camera.transform.forward * Time.deltaTime * 3);
        }
        if (Input.GetKey(KeyCode.S))
        {
            move +=  -(camera.transform.forward * Time.deltaTime * 3);
        }
        if (Input.GetKey(KeyCode.A))
        {
            move += -(camera.transform.right * Time.deltaTime * 3);
        }
        if (Input.GetKey(KeyCode.D))
        {
            move += (camera.transform.right * Time.deltaTime * 3);
        }

        transform.position += move;

        Vector3 velocity = move / Time.deltaTime;
        Vector3 local_velocity = transform.InverseTransformDirection(velocity);

        animator.SetFloat(velocity_x_hash, local_velocity.x);
        animator.SetFloat(velocity_z_hash, local_velocity.z);

    }
}

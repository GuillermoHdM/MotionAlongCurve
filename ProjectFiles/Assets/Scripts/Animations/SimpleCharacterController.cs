using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class SimpleCharacterController : MonoBehaviour
{
    public Camera camera;

    int velocity_hash;
    Animator animator;

    float t = 0.0f;
    float t_max = 2.0f;

    float max_velocity = 45.0f;
    float curr_velocity = 0.0f;

    float increment = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        velocity_hash = Animator.StringToHash("Velocity");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        increment = Mathf.Lerp(0, t_max, t);
        curr_velocity = max_velocity * t/t_max;

        Vector3 move = Vector3.zero;

        if (!Input.anyKey)
        {
            t = 0;
            curr_velocity = 0.0f;
            animator.SetFloat(velocity_hash, 0);
            return;
        }

        if (Input.GetKey(KeyCode.W))
        {
            move += (camera.transform.forward * curr_velocity * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            move += -(camera.transform.forward * curr_velocity * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            move += -(camera.transform.right * curr_velocity * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            move += (camera.transform.right * curr_velocity * Time.deltaTime);
        }

        transform.position += move;

        animator.SetFloat(velocity_hash, curr_velocity / max_velocity);

        t += (Time.deltaTime);
    }
}

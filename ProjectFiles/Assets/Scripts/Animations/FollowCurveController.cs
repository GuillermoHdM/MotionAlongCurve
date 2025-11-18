using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;

public class FollowCurveController : MonoBehaviour
{
    public string state = "Idle";

    public BezierCurve curve;

    Animator animator;
    int velocity_hash;

    public float t = 0.0f;
    float t_max = 5.0f;

    public float total_distance = 0.0f;

    float max_velocity = 4.0f;
    public float curr_velocity = 1.0f;


    float increment = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        velocity_hash = Animator.StringToHash("Velocity");
    }

    public void SetVelocity(float sliderVal)
    {
        curr_velocity = sliderVal;  
    }


    // Update is called once per frame
    void Update()
    {

        if(curve.GetTotalLength() < total_distance)
        {
            total_distance = 0.0f;
            t = 0.0f;
        }

        if(curr_velocity > 0)
        {
            if(curr_velocity < max_velocity)
            {
                state = "Run";
            }
            else
            {
                state = "Naruto run";
            }
        }
        else
        {
            state = "Idle";
        }

        increment = Mathf.Lerp(0.0f, t_max, t);

        total_distance += (curr_velocity * Time.deltaTime); 

        Vector3 new_position = curve.GetPos(total_distance);
        new_position = new Vector3(new_position.x, transform.position.y, new_position.z);

        Vector3 next = curve.GetPos(total_distance + (Time.deltaTime));
        Vector3 next2 = curve.GetPos(total_distance + (Time.deltaTime) * 2.0f);
        Vector3 next3 = curve.GetPos(total_distance + (Time.deltaTime) * 3.0f);
        Vector3 average = (next + next2 + next3) / 3.0f;
        Vector3 direction = average - new_position;


        Vector3 lookDirection = new Vector3(direction.x, 0, direction.z);
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = targetRotation;
        }


        transform.position = new_position;
         
        animator.SetFloat(velocity_hash, curr_velocity / max_velocity);

        t += (Time.deltaTime);
    }
}

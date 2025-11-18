using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 10.0f;
    public float rotateSensitivity = 0.2f;

    Vector3 anchorPoint;
    Quaternion anchorRot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void LookToMouse()
    {
        if (Input.GetMouseButtonDown(1))
        {
            anchorPoint = new Vector3(Input.mousePosition.y, -Input.mousePosition.x);
            anchorRot = transform.rotation;
        }
        if (Input.GetMouseButton(1))
        {
            Quaternion rot = anchorRot;
            Vector3 dif = anchorPoint - new Vector3(Input.mousePosition.y, -Input.mousePosition.x);
            rot.eulerAngles += dif * rotateSensitivity;
            transform.rotation = rot;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Movement
        if (Input.GetMouseButton(1))
        {
            LookToMouse();

            if (Input.GetKey(KeyCode.W))
            {
                transform.position += transform.forward * speed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.position += transform.right * speed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.A))
            {

                transform.position += -transform.right * speed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.S))
            {

                transform.position += -transform.forward * speed * Time.deltaTime;
            }
        } 
        else
        {
            if (Input.GetKey(KeyCode.W))
            {

                transform.position += transform.up * speed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.position += transform.right * speed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.A))
            {

                transform.position += -transform.right * speed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.S))
            {

                transform.position += -transform.up * speed * Time.deltaTime;
            }
        }

    }
}

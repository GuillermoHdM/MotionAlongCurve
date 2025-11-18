using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;

public class TileSelect : MonoBehaviour
{
    public Material notSelectedMaterial;
    public Material hoverMaterial;
    public Material selectedMaterial;

    private Material currentMaterial;
    Renderer objectRenderer;

    bool enableHovering = true;
    float blockHeightObstacle = 3.0f;
    float blockHeightDefault = 1.0f;

    Vector3 originalPos;

    void Start()
    {
        currentMaterial = notSelectedMaterial;
        originalPos = transform.position;
        objectRenderer = GetComponent<Renderer>();
    }

    void Hovering(RaycastHit hit)
    {
        // Check if the mouse is hovering over this object
        if (hit.collider.gameObject == gameObject)
        {
            // Change material to hover material
            if (enableHovering)
            {
                objectRenderer.material = hoverMaterial;
            }

            // Detect mouse click on the object
            if (Input.GetMouseButtonDown(0))
            {
                objectRenderer.material = selectedMaterial;
                currentMaterial = selectedMaterial;
                enableHovering = false;

                transform.localScale = new Vector3(blockHeightDefault, blockHeightObstacle, blockHeightDefault);

                Vector3 pos = transform.position;
                pos.y = originalPos.y + blockHeightObstacle * 0.33f;
                transform.position = pos;
            }

            // Detect mouse click on the object
            if (Input.GetMouseButtonDown(1))
            {
                objectRenderer.material = notSelectedMaterial;
                currentMaterial = notSelectedMaterial;
                enableHovering = false;

                transform.localScale = new Vector3(blockHeightDefault, blockHeightDefault, blockHeightDefault);
                 
                transform.position = originalPos;

            }
        }

        else
        {
            NotHovering(hit);
        }
    }

    void NotHovering(RaycastHit hit)
    {
        enableHovering = true; 
        objectRenderer.material = currentMaterial;
    }

    // Update is called once per frame
    void Update()
    {

        // Raycasting from the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit; 

        // Check if the ray hits a collider
        if (Physics.Raycast(ray, out hit))
        {
            Hovering(hit);
        }
        else
        {
            NotHovering(hit);
        }

    }
}

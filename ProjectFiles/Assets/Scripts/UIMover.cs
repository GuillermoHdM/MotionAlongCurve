using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMover : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public GameObject rectTransformObject;

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {

        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        rectTransformObject.transform.position = screenPosition;
    }
}

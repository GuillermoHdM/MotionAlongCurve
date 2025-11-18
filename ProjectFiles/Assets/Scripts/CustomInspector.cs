using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierCurve))]
public class CustomInspector : Editor
{
    bool drawPath;

    BezierCurve bezierCurve;
    GameObject CPointsParent;

    void OnEnable()
    {
        bezierCurve = (BezierCurve)target;
        CPointsParent = bezierCurve.GetControlPoints();
    }


    public override void OnInspectorGUI()
    {
        if(GUILayout.Button("Create new curve"))
        {
            Vector3 lastPoint = bezierCurve.GetLastPoint();
            Vector3 lastPointDir = bezierCurve.GetLastPointDir();

            GameObject newPoint_1 = Instantiate(bezierCurve.GetControlPointPrefab(), lastPoint + lastPointDir, Quaternion.identity);
            GameObject newPoint_2 = Instantiate(bezierCurve.GetControlPointPrefab(), lastPoint + lastPointDir * 1.3f, Quaternion.identity);
            GameObject newPoint_3 = Instantiate(bezierCurve.GetControlPointPrefab(), lastPoint + lastPointDir * 1.6f, Quaternion.identity);

            newPoint_1.transform.SetParent(CPointsParent.transform);
            newPoint_2.transform.SetParent(CPointsParent.transform);
            newPoint_3.transform.SetParent(CPointsParent.transform);
        }

        // This draws the default inspector properties
        DrawDefaultInspector();
    }

}

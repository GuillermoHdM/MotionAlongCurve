using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using UnityEngine.UI;

//public class ArcLengthReparametrization : MonoBehaviour
public partial class BezierCurve : MonoBehaviour
{
    int NumberOfCurves = 1;
    public bool DebugDraw = true;
    public float Resolution = 0.0005f;//changes the ammount of entries on the lookup table
    float TotDist;
    List<float> s = new List<float>();
    List<float> U = new List<float>();
    BezierCurve curve;
    bool Called = false;
    bool DrawnDebug = false;
    public GameObject sphereHolder;

    string objectNameToDelete = "Sphere";

    void CleanSpheres()
    {
        GameObject[] allObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject rootObject in allObjects)
        {
            Transform[] children = rootObject.GetComponentsInChildren<Transform>(true);
            for (int i = children.Length - 1; i >= 0; i--)
            {

                if (children[i].gameObject.name == objectNameToDelete)
                {
#if UNITY_EDITOR
                    // Destroy the GameObject immediately
                    DestroyImmediate(children[i].gameObject); // Use DestroyImmediate in the editor for immediate feedback
#else
                    Destroy(children[i].gameObject); // Use Destroy at runtime
#endif
                }
            }
        }
    }
    float GetArclegnth(float u)
    {
        int i1 = (int)Math.Floor(u / Resolution);
        int i2 = (int)Math.Floor(u / Resolution + 0.5);
        float arclenght = s[i1] + (u - U[i1]) / (U[i2] - U[i1]) * (s[i2] - s[i1]);
        return arclenght;
    }
    public float GetTotalLength()
    {
        if (s.Count == 0)
            return 0.0f;
        return s[s.Count - 1];
    }
    public Vector3 GetPos(float Dist)
    {
        if (s.Count == 0)
            return new Vector3();

        float d = Math.Clamp(Dist, 0, s[s.Count - 1]);
        var result = BinarySearchWithRange(s, d);

        //here i get 2 indexes
        if (result != null)
        {
            if (result.Value.Item1 == result.Value.Item2)
            {
                //found value
                //U[result.Value.Item1]
                return Points[result.Value.Item1];
            }
            else
            {
                //U[result.Value.Item1]
                //U[result.Value.Item2]
                //evaluate the curve twice
                //interpolate
                //success
                Vector3 p1 = Points[result.Value.Item1];
                Vector3 p2 = Points[result.Value.Item2];
                float t = (d - s[result.Value.Item1]) / (s[result.Value.Item2] - s[result.Value.Item1]);
                return Vector3.Lerp(p1, p2, t);

            }
        }
        Debug.Log("Something went terribly wrong, the distance evaluated is freater that the total length of the curve");
        return Points[Points.Length - 1];//return last point just in case
    }
    void Generate_S_Table()
    {
        //Called = true;
        //if(curve == null)
        //{
        //    return;
        //}
        //NumberOfCurves = curve.GetCurveNumber();
        NumberOfCurves = GetCurveNumber();
        if (NumberOfCurves == 0)
        {
            //Debug.Log("curve.GetCurveNumber() returned 0 curves");
            NumberOfCurves = 1;
        }
        s.Clear();
        s.Add(0.0f);
        U.Add(0.0f);
        //s[1] = Vector3.Distance(Q1, Q2);
        float CurrT = deltaT;
        for (int i = 1; i < Points.Length; i++, CurrT += deltaT)
        {
            float dist = Vector3.Distance(Points[i], Points[i - 1]);
            float L = s[i - 1] + dist;
            s.Add(L);
            U.Add(CurrT);
        }
    }
    //
    //to sample the s table from the parameter, we have the function: i+1 = floor(input t / Resolution + 0.5) and the prev point i = floor(input t / Resolution)
    //Given those 2 points we interpolate the arc length as s = S[i] + (u - U[i])/(U[i+1] - U[i]) * (S[i+1] - S[i])
    //That is the arclength
    // Start is called before the first frame update


    // Update is called once per frame
    //void Update()
    //{
    //    if (!Called)
    //    {
    //        Invoke("Generate_S_Table", 1f);
    //        TotDist = s[s.Count - 1];//get total dist
    //        return;
    //    }
    //    if (Called)//check it works
    //    {
    //        if (DebugDraw)
    //        {
    //            if (DrawnDebug == true)//this is so its only drawn once
    //            {
    //                return;
    //            }
    //            for (float D = 0; D < TotDist; D += 0.5f)
    //            {
    //                Vector3 Pos = GetPos(D);
    //                GameObject tmp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //                tmp.transform.position = Pos;
    //                tmp.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); //smaller sphere
    //            }
    //            DrawnDebug = true;
    //        }
    //    }
    //}

    // Binary search function that returns the two indeces the target lies between
    (int, int)? BinarySearchWithRange(List<float> list, float value)
    {
        int left = 0;
        int right = list.Count - 1;

        while (left <= right)
        {
            int middle = left + (right - left) / 2; // Find the middle index

            // Check if the value is present at the middle
            if (list[middle] == value)
            {
                // If found, return the index of the value as both indices
                return (middle, middle);
            }

            // If value is greater than the middle, ignore the left half
            if (list[middle] < value)
            {
                left = middle + 1;
            }
            // If value is smaller than the middle, ignore the right half
            else
            {
                right = middle - 1;
            }
        }

        // If we reach here, the value is not found, so return the indices of the two closest values
        if (left == 0)
        {
            return (0, 0);  // The value is less than the smallest element (no range possible)
        }
        else if (left >= list.Count)
        {
            return (list.Count - 1, list.Count - 1);  // The value is greater than the largest element (no range possible)
        }
        else
        {
            // Return the indices of the two closest values (list[right], list[left])
            return (right, left);
        }
    }

    public void RefreshCalculations()
    {
        Called = false;
    }


    void DrawDebugArclength(float step)
    {
        if (Called == false)
        {
            CleanSpheres();
            if (DebugDraw)
            {
                TotDist = GetTotalLength();
                for (float D = 0; D < TotDist; D += step)
                {
                    Vector3 Pos = GetPos(D);
                    GameObject tmp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    tmp.transform.position = Pos;
                    tmp.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); //smaller sphere
                }
                GameObject DT = GameObject.Find("DT");
                if (DT == null)
                    return;

                Transform DTTransform = DT.transform;
                DTTransform = DTTransform.GetChild(0);
                int i = 0;
                for (; i < DTTransform.childCount - 1; i++)
                {
                    Transform child = DTTransform.GetChild(i);
                    GameObject childObject = child.gameObject;
                    if(childObject.name == "Idx")
                    {
                        Text txt = childObject.GetComponent<Text>();
                        txt.text = s[i].ToString();
                    }
                }
                Transform Lastchild = DTTransform.GetChild(i);
                GameObject LastchildObject = Lastchild.gameObject;
                if (LastchildObject.name == "Idx")
                {
                    Text txt = LastchildObject.GetComponent<Text>();
                    txt.text = GetTotalLength().ToString();
                    Debug.Log(txt);
                }
                Called = true;
            }
        }

    }
}
using System.Linq;
using UnityEditor;
using UnityEngine;


[ExecuteInEditMode]
public partial class BezierCurve : MonoBehaviour
{
    public GameObject CPointsParent;
    Vector3[] ControlPoints;

    [Range(0.001f, 0.5f)]
    public float deltaT;
    Vector3[] Points;

    public Material PassingPointMat;
    public Material CtrlPointMat;
    public GameObject C_point;

    Vector3 ColiearLine;
    GameObject SelectedPassing_P = null;

    // Start is called before the first frame update
    void Start()
    {
        SetControlPoints();
    }

    void SetControlPoints()
    {
        if (CPointsParent == null)
            return;

        ControlPoints = new Vector3[CPointsParent.transform.childCount];
        for (int it = 0; it < ControlPoints.Length; it++)
        {
            GameObject child = CPointsParent.transform.GetChild(it).gameObject;
            ControlPoints[it] = child.transform.position;

            if (it % 3 == 0)
            {
                child.GetComponent<MeshRenderer>().material = PassingPointMat;
                child.tag = "Passing_P";
            }
            else
            {
                child.GetComponent<MeshRenderer>().material = CtrlPointMat;
                child.tag = "Control_P";
            }
        }
    }

    public GameObject GetControlPointPrefab()
    {
        return C_point;
    }

    public GameObject GetControlPoints()
    {
        return CPointsParent;
    }
    public Vector3 GetLastPointDir()
    {
        int lastID = ControlPoints.Length - 1;
        return ControlPoints[lastID] - ControlPoints[lastID - 1];
    }
    public Vector3 GetLastPoint()
    {
        int lastID = ControlPoints.Length - 1;
        return ControlPoints[lastID];
    }



    int GetControlPointID(GameObject point)
    {
        for (int it = 0; it < ControlPoints.Length; it++)
        {
            GameObject child = CPointsParent.transform.GetChild(it).gameObject;

            if (child == point)
                return it;
        }

        return -1;
    }

    // Not using factorial to avoid inevitable overflow
    private double Combinatorial(int a, int b)
    {
        double result = 1.0;
        for (int it = 0; b - it > 0; it++)
        {
            double num = (double)(a - it);
            double den = (double)(b - it);
            result *= num / den;
        }

        return result;
    }

    public void ComputePath()
    {
        SetControlPoints();

        Points = new Vector3[0];

        // +=3 because the last point is shared between curves
        for (int it = 0; it < ControlPoints.Length - 1; it += 3)
        {
            Vector3[] result = BezierBerstein(it);

            Points = Points.Concat(result).ToArray();
        }

        // generate the table
        Generate_S_Table();
    }

    // The points used for the algorithm are the first 4 starting from the passed index
    Vector3[] BezierBerstein(int startIdx)
    {
        // Sanity check
        if (ControlPoints != null && ControlPoints.Length == 0)
            return new Vector3[0];

        // Not enough points for this curve
        if (ControlPoints.Length - startIdx < 4)
            return new Vector3[0];


        int size = (int)(1.0f / deltaT);

        Vector3[] BezierPoints = new Vector3[size];
  

        // Compute all the points
        float t = 0;
        int n = 3; // 4 counting the 0

        for (int pointsIt = 0; pointsIt < size; pointsIt++)
        {
            Vector3 point = new Vector3(0,0,0);
        
            // Iterate through the control points
            for (int i = 0; i <= n; i++)
            {
                // Bernstein polynomial formula
                float BernsteinScalar = (float)(Combinatorial(n, i) * (double)(Mathf.Pow((1.0f - t), n - i) * Mathf.Pow(t, i)));
                point += ControlPoints[i + startIdx] * BernsteinScalar;
            }
            BezierPoints[pointsIt] = point;
        
            // Increase t for next point
            t += deltaT;
        }

        // Add last point
        BezierPoints[size - 1] = ControlPoints[startIdx + n];

        return BezierPoints;
    }


    void OnDestroy()
    { 
        CleanSpheres();
    }
    // Update is called once per frame
    void Update()
    {
        // Compute path
        ComputePath();

        if (Points == null || Points.Length < 2)
            return;

        for (int it = 0; it < Points.Length - 1; it++)
        {
            Debug.DrawLine(Points[it], Points[it + 1], UnityEngine.Color.red);
        }
		DrawDebugArclength(1.0f);

        GameObject selectedObj = Selection.activeGameObject;

        // Force the scene to update selection status
        if (selectedObj == null)
        {
            SelectedPassing_P = null;
            return;
        }

        int ID = GetControlPointID(selectedObj);

        // No control point selected
        // First and last two points can't collinearate
        if (ID == -1 || ID <= 1 || ID >= ControlPoints.Length - 2)
        {
            SelectedPassing_P = null;

            if(ID <= 1)
                Debug.DrawLine(ControlPoints[0], ControlPoints[1], UnityEngine.Color.blue);
            else if(ID >= ControlPoints.Length - 2)
            {
                int lenght = ControlPoints.Length - 1;
                Debug.DrawLine(ControlPoints[lenght-1], ControlPoints[lenght], UnityEngine.Color.blue);
            }
            return;
        }

        string selectedTag = selectedObj.tag;

        if (SelectedPassing_P != selectedObj)
            SelectedPassing_P = null;

        if (selectedTag == "Passing_P")
        {
            GameObject NextControl_P = CPointsParent.transform.GetChild(ID + 1).gameObject;
            GameObject PrevControl_P = CPointsParent.transform.GetChild(ID - 1).gameObject;

            if (SelectedPassing_P == null)
            {
                ColiearLine = NextControl_P.transform.position - selectedObj.transform.position;
                SelectedPassing_P = selectedObj;
            }

            NextControl_P.transform.position = selectedObj.transform.position + ColiearLine;
            PrevControl_P.transform.position = selectedObj.transform.position - ColiearLine;

            Debug.DrawLine(PrevControl_P.transform.position, NextControl_P.transform.position, UnityEngine.Color.blue);
        }
        else if (selectedTag == "Control_P")
        {
            // Get point to the other side on the passing P
            // It is either to the left or right
            GameObject Passing_P = CPointsParent.transform.GetChild(ID + 1).gameObject;
            GameObject NextControl_P = CPointsParent.transform.GetChild(ID + 2).gameObject;

            // Get in the other side
            if (Passing_P.tag != "Passing_P")
            {
                Passing_P = CPointsParent.transform.GetChild(ID - 1).gameObject;
                NextControl_P = CPointsParent.transform.GetChild(ID - 2).gameObject;
            }

            Vector3 Ctrl_to_Passing = Passing_P.transform.position - selectedObj.transform.position;

            NextControl_P.transform.position = Passing_P.transform.position + Ctrl_to_Passing;

            Debug.DrawLine(selectedObj.transform.position, NextControl_P.transform.position, UnityEngine.Color.blue);
        }
        
    }

    public int GetCurveNumber()
    {
        return (ControlPoints.Length - 1) % 3;
    }

    public Vector3 GetPointInCurve(float t)
    {
        if(Points == null)
        {
            Debug.Log("Requested a point in the curve before the points were computed");
            return new Vector3();
        }
        int idx = (int)(t / deltaT);

        // Avoid out of bounds
        idx = Mathf.Min(idx, Points.Length - 1);

        return Points[idx];
    }
}

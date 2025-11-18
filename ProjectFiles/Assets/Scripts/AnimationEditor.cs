using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimationEditor : MonoBehaviour
{
    public FollowCurveController player;
    public BezierCurve arcLength;
    public Animator animator;

    public TMP_Text player_state;
    public TMP_Text player_velocity;
    public TMP_Text total_curve_time;
    public TMP_Text player_current_time;
    public TMP_Text total_curve_length;

    public GameObject distTable;
    public Slider speedSlider;

    bool setOnce = true;
    float prevLength = 1f;
    void Start()
    {
        prevLength = arcLength.GetTotalLength();
    }

    // Update is called once per frame
    void Update()
    {
        if (setOnce)
        {
            float value = arcLength.GetTotalLength() / player.curr_velocity;
            if (value != 0)
            {
                float val = player.t + (arcLength.GetTotalLength() - player.total_distance) / player.curr_velocity;
                total_curve_time.text = "Total Curve Time: " + val.ToString("F2") + "s";
                setOnce = false;
            }
        }

        player_state.text = "Current state : " + player.state;
        total_curve_length.text = "Curve length: " + arcLength.GetTotalLength().ToString("F2") + "m";
        player_velocity.text = player.curr_velocity.ToString("F2") + "m/s";
        player_current_time.text = "Time: " + player.t.ToString("F2") + "s";

        if (prevLength != arcLength.GetTotalLength())
        {
            UpdateTimeLeft();
        }

        prevLength = arcLength.GetTotalLength();
    }

    public void UpdateTimeLeft(float timeMal = 0f)
    {
        float val = player.t - timeMal + (arcLength.GetTotalLength() - player.total_distance) / player.curr_velocity;
         
        total_curve_time.text = "Total Curve Time: " + val.ToString("F2") + "s";
        total_curve_length.text = "Curve length: " + arcLength.GetTotalLength().ToString("F2") + "m";
    }

    public void ToggleDistTable()
    { 
        distTable.SetActive(!distTable.activeSelf);
    }
    public void ResetSpeed()
    { 
        speedSlider.value = 1;
    }
    public void ResetTime()
    { 
        player.t = 1;
        player.total_distance = 0;
    }
}

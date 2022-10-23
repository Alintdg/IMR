using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine;
using TMPro;
using System;


public class ThrowTrigger : MonoBehaviour
{
    public float score;
    public TextMeshProUGUI scoreUI;

    void Update()
    {
        scoreUI.text = "score: " + Math.Round(score / 2, 2).ToString();
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "ball")
        {
            float distance = GameObject.Find("Military target").transform.position.y - GameObject.Find("LeftHand Controller").transform.position.y;
            print(distance);
            score = distance;
        }
    }
}
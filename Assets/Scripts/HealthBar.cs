using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider healthBar;

    void Start()
    {
        healthBar = GetComponent<Slider>();
        healthBar.maxValue = 100;
        healthBar.value = 100;
    }

    public float HealthPoints
    {
        get { return healthBar.value; }
        set { healthBar.value = value; }
    }

    public float HealthPointsMax
    {
        get { return healthBar.maxValue; }
        set { healthBar.maxValue = value; }
    }
}

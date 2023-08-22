using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.SetQualityLevel(2);
    }

    public void low()
    {
        QualitySettings.SetQualityLevel(0);
    }

    public void medium()
    {
        QualitySettings.SetQualityLevel(1);
    }

    public void high()
    {
        QualitySettings.SetQualityLevel(2);
    }
}
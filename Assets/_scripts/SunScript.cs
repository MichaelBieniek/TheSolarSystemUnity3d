using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunScript : MonoBehaviour
{
    // thousands of KM in 10^3
    public float DiameterInEarths = 0f;
    // as a mass in 10^20 (e20)
    public float MassInEarths = 0f;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.localScale = new Vector3(DiameterInEarths, DiameterInEarths, DiameterInEarths);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

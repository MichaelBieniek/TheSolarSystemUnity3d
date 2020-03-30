using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FPScript : MonoBehaviour
{
    public float turnSpeed = 4.0f;
    public float moveSpeed = 100.0f;

    public float minTurnAngle = -90.0f;
    public float maxTurnAngle = 90.0f;
    private float rotX;

    public GameObject sun;
    public TextMeshProUGUI distanceToSun;
    public TextMeshProUGUI rayDetect;

    void Update()
    {
        MouseAiming();
        KeyboardMovement();
        CalcDistanceToSun();
        Trace();
    }

    void MouseAiming()
    {
        // get the mouse inputs
        float y = Input.GetAxis("Mouse X") * turnSpeed;
        rotX += Input.GetAxis("Mouse Y") * turnSpeed;

        // clamp the vertical rotation
        rotX = Mathf.Clamp(rotX, minTurnAngle, maxTurnAngle);

        // rotate the camera
        transform.eulerAngles = new Vector3(-rotX, transform.eulerAngles.y + y, 0);
    }

    void KeyboardMovement()
    {
        Vector3 dir = new Vector3(0, 0, 0);

        dir.x = Input.GetAxis("Horizontal");
        dir.z = Input.GetAxis("Vertical");

        transform.Translate(dir * moveSpeed * Time.deltaTime);
    }

    void CalcDistanceToSun()
    {
        var distance = Vector3.Distance(this.transform.position, sun.transform.position) / Universe.SCALE * Universe.EARTH_DIAMETER;
        var distanceInMill = Math.Round(distance / 1000000D, 3).ToString("0.000");
        var distanceInAU = Math.Round(distance / Universe.AU, 3).ToString("0.000");
        if(distanceToSun != null)
        {
            distanceToSun.text = string.Format("Distance to sun: {0} million km ({1} AU)", distanceInMill, distanceInAU);
        }
    }

    void Trace()
    {
        RaycastHit hit;
        // Does the ray intersect any objects
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            rayDetect.text = string.Format("{0} - Distance: {1} AU", hit.transform.name, Math.Round(hit.distance * Universe.EARTH_DIAMETER / Universe.SCALE / Universe.AU, 3).ToString("0.000"));
            var celestialBody = hit.collider.transform.GetComponent<CelestialBody>();
            if(celestialBody != null)
            {
                celestialBody.Dectivate();
            }
            
        }
        else
        {
            rayDetect.text = "";
        }
    }
}


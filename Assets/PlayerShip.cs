﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerShip : MonoBehaviour
{
    public float turnSpeed = 4.0f;
    [Tooltip("Move speed units / s")]
    public float moveSpeed = 100.0f;

    [Tooltip("Amount of thrust force to add")]
    public float thrustForce = 30f;

    public float minTurnAngle = -90.0f;
    public float maxTurnAngle = 90.0f;
    private float rotX;

    public GameObject sun;
    public TextMeshProUGUI distanceToSun;
    public TextMeshProUGUI rayDetect;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MouseAiming();
        //KeyboardMovement();
        CalcDistanceToSun();
        Trace();
    }

    private void FixedUpdate()
    {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        var brake = Input.GetButton("Jump");
        
        if(brake)
        {
            _rb.drag = 10;
            return;
        }
        _rb.drag = 0;
        if (x != 0 || y != 0)
        {
            _rb.AddRelativeForce(x * thrustForce, 0, y * thrustForce, ForceMode.Force);
        }
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
        var text = "";
        if (distanceToSun != null)
        {
            text = string.Format("Distance to sun: {0} million km ({1} AU)", distanceInMill, distanceInAU);
        }
        text += "\n";
        text += string.Format("Current speed: {0} km/s", (_rb.velocity.magnitude * Universe.EARTH_DIAMETER).ToString("###,###"));
        distanceToSun.text = text;
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
            if (celestialBody != null)
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPScript : MonoBehaviour
{
    public float turnSpeed = 4.0f;
    public float moveSpeed = 100.0f;

    public float minTurnAngle = -90.0f;
    public float maxTurnAngle = 90.0f;
    private float rotX;

    public GameObject sun;
    public Text distanceToSun;

    void Update()
    {
        MouseAiming();
        KeyboardMovement();
        CalcDistanceToSun();
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
        var distance = Vector3.Distance(this.transform.position, sun.transform.position);
        if(distanceToSun != null)
        {
            distanceToSun.text = string.Format("Distance to sun: {0} million km", distance / 10f);
        }
    }
}


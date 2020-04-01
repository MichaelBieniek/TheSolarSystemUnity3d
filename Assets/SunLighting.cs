using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunLighting : MonoBehaviour
{
    //todo: big assumption, Sun is [0,0,0] for simplicity's sake

    Vector3 sunPos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.localPosition = Vector3.zero;
        this.transform.localScale = Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
        var direction = this.transform.position - sunPos;
        this.transform.rotation = Quaternion.LookRotation(sunPos);
        this.transform.Rotate(new Vector3(0, 90, 0));
        this.transform.localPosition = -Vector3.Normalize(direction)*2;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public static float EARTH_DIAMETER = 6371 * 2f;
    public static double EARTH_MASS = 5.97237E24;
    public static double AU = 1.496E8;
    public static double SCALE = 0.1D;

    public float DiameterInEarths = 1f;
    public float MassInEarths = 1f;
    public float DistanceFromStarInAu = 1f; 

    private GameObject _gravField = null;
    private bool _isRotating = false;


    private float _distanceNorm = 0;
    private float _diameterNorm = 0;
    private float _massNorm = 0;

    private Rigidbody _rb;
    

    // 1 unit = 1 earth dia
 
    // Start is called before the first frame update
    void Start()
    {
        _distanceNorm = (float)(SCALE * this.DistanceFromStarInAu * AU / EARTH_DIAMETER);
        _diameterNorm = (float)(this.DiameterInEarths * EARTH_DIAMETER / 1000);
        Debug.Log(string.Format("Planet: {0} is {1} earths from the sun", this.name, _distanceNorm ));
        this.transform.localScale = new Vector3(_diameterNorm, _diameterNorm, _diameterNorm);
        this.transform.position = new Vector3(_distanceNorm, 0f, 0f);
        CreateMass();
        //_gravField = this.CreateGravField();
        //StartRotation();
    }

    // Update is called once per frame
    void Update()
    {
        if(_isRotating && _gravField != null)
        {
            _gravField.transform.Rotate(0f, 0.1f, 0f, Space.World);
        }
    }

    GameObject CreateGravField()
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = new Vector3(0f, 0f, 0f);
        sphere.transform.localScale = new Vector3(_distanceNorm, _distanceNorm, _distanceNorm) * 2f;

        sphere.AddComponent(typeof(Rigidbody));
        sphere.transform.GetComponent<Rigidbody>().useGravity = false;
        sphere.transform.GetComponent<Rigidbody>().isKinematic = true;
        sphere.name = string.Format("{0}-GravField", this.name);

        //sphere.AddComponent(typeof(ConstantForce));


        // hide mesh
        Mesh mesh = sphere.GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        this.transform.parent = sphere.transform;
        return sphere;
    }

    void StartRotation()
    {
        _isRotating = true;
    }

    // create a RigidBody and adds mass based on props
    void CreateMass()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if(rb == null)
        {
            rb = (Rigidbody)this.gameObject.AddComponent(typeof(Rigidbody));
        }
        _massNorm = MassInEarths;
        rb.mass = _massNorm;
        rb.useGravity = false;
        _rb = rb;
    }
}

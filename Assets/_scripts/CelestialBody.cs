using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public Material RaySphereMat;
    public float DiameterInEarths = 1f;
    public float MassInEarths = 1f;
    public float DistanceFromStarInAu = 1f;
    public float InitialVelocityZ = 0f;
    public float InitialVelocityY = 0f;
    public float InitialVelocityX = 0f;



    private float _distanceNorm = 0;
    private float _diameterNorm = 0;
    private float _massNorm = 0;

    private GameObject _collisionSphere;

    private Rigidbody _rb;
    public Rigidbody rb
    {
        get
        {
            return this._rb;
        }
    }

    public static List<CelestialBody> bodies;

    // 1 unit = 1 earth dia
 
    // Start is called before the first frame update
    void Start()
    {
        _distanceNorm = (float)(Universe.SCALE * this.DistanceFromStarInAu * Universe.AU / Universe.EARTH_DIAMETER);
        _diameterNorm = (float)(this.DiameterInEarths);
        Debug.Log(string.Format("Planet: {0} is {1} earths from the sun", this.name, _distanceNorm ));
        this.transform.localScale = new Vector3(_diameterNorm, _diameterNorm, _diameterNorm);
        this.transform.localPosition = new Vector3(_distanceNorm, 0f, 0f);
        this.transform.localRotation = Quaternion.identity;
        CreateMass();
        _collisionSphere = BuildCollisionSphere();
        Vector3 additiveVelocity = Vector3.zero;
        if(this.transform.parent != null && this.transform.parent.GetComponent<Rigidbody>() != null)
        {
            
            var rbParent = this.transform.parent.GetComponent<Rigidbody>();
            additiveVelocity = rbParent.velocity;
            Debug.Log("Parent vel " + additiveVelocity);
      
        }
        rb.velocity = new Vector3(InitialVelocityX, InitialVelocityY, InitialVelocityZ) + additiveVelocity;

    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        foreach(CelestialBody cb in bodies)
        {
            if(cb != this)
            {
                Attract(cb);
            }
            
        }

    }

    private void OnEnable()
    {
        if(bodies == null)
        {
            bodies = new List<CelestialBody>();
        }
        bodies.Add(this);
    }

    private void OnDisable()
    {
        bodies.Remove(this);
    }

    GameObject BuildCollisionSphere()
    {
        if (RaySphereMat != null)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.parent = this.transform;
            sphere.transform.localPosition = new Vector3(0f, 0f, 0f);
            sphere.transform.localScale = Vector3.one * 2f;

            //sphere.AddComponent(typeof(Rigidbody));
            //sphere.transform.GetComponent<Rigidbody>().useGravity = false;
            sphere.transform.name = string.Format("RaySphere-{0}", this.name);
        
            Renderer meshRenderer = sphere.GetComponent<Renderer>();
            meshRenderer.material = RaySphereMat;
            sphere.SetActive(false);
            return sphere;
        }
        return null;
        //sphere.transform.GetComponent<Rigidbody>().isKinematic = true;


        // hide mesh
        //Mesh mesh = sphere.GetComponent<MeshFilter>().mesh;
        //mesh.Clear();
        //this.transform.parent = sphere.transform;

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

    void Attract(CelestialBody cb)
    {
        Rigidbody rbOther = cb.rb;
        Vector3 direction = _rb.position - rbOther.position;
        float distance = direction.magnitude;

        float forceMagnitude = (_rb.mass * rbOther.mass) / Mathf.Pow(distance, 2);
        Vector3 force = direction.normalized * forceMagnitude;

        rbOther.AddForce(force);
    }

    public void Dectivate()
    {
        if (_collisionSphere != null)
        {
            _collisionSphere.SetActive(true);
            Destroy(_collisionSphere, 3);
            _collisionSphere = BuildCollisionSphere();
        }
    }

}

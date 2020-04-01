using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TYPE
{
    STAR,
    PLANET,
    MOON
}

public class CelestialBody : MonoBehaviour
{
    public Material RaySphereMat;
    public float DiameterInEarths = 1f;
    public float MassInEarths = 1f;
    public float DistanceFromStarInAu = 1f;
    public float DayInEarthDays;
    public float AxialTiltDeg;

    public float InitialVelocityZ = 0f;
    public float InitialVelocityY = 0f;
    public float InitialVelocityX = 0f;
    public TYPE type = TYPE.PLANET;
    public string Name = "Generic celestial body";

    public GameObject[] orbiters;

    private float _distanceNorm = 0;
    private float _diameterNorm = 0;
    private float _massNorm = 0;
    private bool _seenByPlayer = false;

    private GameObject _collisionSphere;
    private CelestialBody _primaryCb;

    

    private Rigidbody _rb;
    public Rigidbody rb
    {
        get
        {
            return this._rb;
        }
    }

    private List<CelestialBody> orbitingBodies = new List<CelestialBody>();

    public void Awake()
    {
        Debug.Log("Awake: " + this.Name);
        _distanceNorm = this.DistanceFromStarInAu == 0 ? 0 : (float)(Universe.SCALE * this.DistanceFromStarInAu * Universe.AU / Universe.EARTH_DIAMETER);
        _diameterNorm = (float)(this.DiameterInEarths);

        ScaleSize();
        this.transform.localRotation = Quaternion.identity;
        this.transform.Rotate(0, 0, AxialTiltDeg);

        CreateMass();
        _collisionSphere = BuildCollisionSphere();

        
    }

    // 1 unit = 1 earth dia

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("Start: " + this.Name);
        CreateOrbiters();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        foreach(CelestialBody cb in orbitingBodies)
        {
            if(cb != this)
            {
                Attract(cb);
            }
            
        }

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
            sphere.transform.name = string.Format("RaySphere-{0}", this.Name);
        
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

    // scales planets by defined size * PLANET_SCALE universal constant
    void ScaleSize()
    {
        var scaleToUse = _diameterNorm;
        if(this.type != TYPE.STAR)
        {
            scaleToUse *= Universe.PLANET_SCALE;
        }
        this.transform.localScale = new Vector3(scaleToUse, scaleToUse, scaleToUse);
    }

    // Sets the original velocity of body + current velocity of primary passed as parameter
    void SetOriginalVelocity(Vector3 primaryVelocity)
    {
        if (_rb == null)
        {
            Debug.LogError("No rigid body on " + this.Name);
        }
        _rb.velocity = new Vector3(InitialVelocityX, InitialVelocityY, InitialVelocityZ) + primaryVelocity;
    }

    void Attract(CelestialBody cb)
    {
        Rigidbody rbOther = cb.rb;
        if(rbOther == null)
        {
            Debug.Log("Orbiting body has no rigidBody: " + cb.transform.name);
            return;
        }
        //Debug.Log("attracting: " + cb.transform.name);
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

    void CreateOrbiters()
    {
        foreach(GameObject orbiter in orbiters)
        {
            var body = orbiter.GetComponent<CelestialBody>();
            Debug.Log("Creating orbiter: " + body.name);
            if(body == null)
            {
                Debug.LogError("Skipping orbiter");
                // skip any orbiters that are not celestial bodies
                continue;
            }
            var distance = body.DistanceFromStarInAu == 0 ? 0 : (float)(Universe.SCALE * body.DistanceFromStarInAu * Universe.AU / Universe.EARTH_DIAMETER);

            // set position as distance from "primary" in X + the current position of "primary"
            var worldPosition = new Vector3(distance, 0, 0);
            worldPosition += this.transform.position;

            var instance = Instantiate(orbiter, worldPosition, Quaternion.identity);
            var instanceCb = instance.GetComponent<CelestialBody>();
            instanceCb.SetPrimary(this);
            instanceCb.SetOriginalVelocity(this._rb.velocity);
            AddOrbiters(instanceCb);
            //instance.transform.SetParent(this.transform, false);
        }
    }

    void AddOrbiters(CelestialBody body)
    {
        if (orbitingBodies == null)
        {
            orbitingBodies = new List<CelestialBody>();
        }
        orbitingBodies.Add(body);
    }

    void SetPrimary(CelestialBody parent)
    {
        _primaryCb = parent;
    } 

    double GetDistanceToCentre()
    {
        if(this.type == TYPE.STAR || _primaryCb == null)
        {
            return 0D;
        }
        var distance = Vector3.Distance(_primaryCb.transform.position, this.transform.position);
        return distance * Universe.EARTH_DIAMETER / Universe.SCALE / Universe.AU;
    }

    double GetDistanceToCamera()
    {
        var distance = Vector3.Distance(this.transform.position, Camera.main.transform.position);
        return distance * Universe.EARTH_DIAMETER / Universe.SCALE / Universe.AU;
    }

    void OnGUI()
    {
        if(_seenByPlayer)
        {
            var message = this.Name;
            message += "\n";
            message += this.type.ToString();

            if (this.type != TYPE.STAR)
            {
                message += "\n";
                message += string.Format("Distance to primary: {0} AU", Math.Round(GetDistanceToCentre(), 3).ToString("0.00"));
            }
            message += "\n";
            message += string.Format("Distance: {0} AU", Math.Round(GetDistanceToCamera(), 3).ToString("0.00"));
            Vector2 worldPoint = Camera.main.WorldToScreenPoint(transform.position);
            GUI.Label(new Rect(worldPoint.x - 100, (Screen.height - worldPoint.y) - 50, 200, 100), message);
        }
        
    }

    private void OnBecameVisible()
    {
        //Debug.Log(Name + " is visible");
        _seenByPlayer = true;
    }

    private void OnBecameInvisible()
    {
        //Debug.Log(Name + " not visible");
        _seenByPlayer = false;
    }

    

}

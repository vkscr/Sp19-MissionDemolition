using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    static private Slingshot S;

    [Header("Set in Inspector")]                                            // a
    public GameObject prefabProjectile;
    public float velocityMult = 8f;
    // fields set dynamically
    [Header("Set Dynamically")]                                             // a
    public GameObject launchPoint;
    public Vector3 launchPos;                                   // b
    public GameObject projectile;                                  // b
    public bool aimingMode;                                  // b
    private Rigidbody projectileRigidbody;                             // a
    static public Vector3 LAUNCH_POS
    {                                        // b
        get
        {
            if (S == null) return Vector3.zero;
            return S.launchPos;
        }
    }

    void Awake()
    {
        S = this;

        Transform launchPointTrans = transform.Find("LaunchPoint");              // a
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;

    }

    void OnMouseEnter()
    {
        print("Slingshot:OnMouseEnter()");
        launchPoint.SetActive(true);                                           // b

    }
    void OnMouseExit()
    {
        print("Slingshot:OnMouseExit()");
        launchPoint.SetActive(false);                                           // b

    }

    void OnMouseDown()
    {                                                    // d
        // The player has pressed the mouse button while over Slingshot
        aimingMode = true;
        // Instantiate a Projectile
        projectile = Instantiate(prefabProjectile) as GameObject;
        // Start it at the launchPoint
        projectile.transform.position = launchPos;
        // Set it to isKinematic for now
        projectile.GetComponent<Rigidbody>().isKinematic = true;
        projectileRigidbody = projectile.GetComponent<Rigidbody>();                // a
        projectileRigidbody.isKinematic = true;                                    // a

    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!aimingMode) return;                                                   // b
                                                                                   // Get the current mouse position in 2D screen coordinates
        Vector3 mousePos2D = Input.mousePosition;                                  // c
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);
        // Find the delta from the launchPos to the mousePos3D
        Vector3 mouseDelta = mousePos3D - launchPos;
        // Limit mouseDelta to the radius of the Slingshot SphereCollider          // d
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;
        if (Input.GetMouseButtonUp(0))
        {                                         // e
            // The mouse has been released
            aimingMode = false;
            projectileRigidbody.isKinematic = false;
            projectileRigidbody.velocity = -mouseDelta * velocityMult;
            FollowCam.POI = projectile;

            projectile = null;
            MissionDemolition.ShotFired();                             // a
            ProjectileLine.S.poi = projectile;                         // b

        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AdvancedDissolve_Example
{
    public class CylinderDroneController : MonoBehaviour
    {
        public Controller_Mask_Cylinder_Alt maskController;
        public ParticleSystem plasma;
        ParticleSystem.EmissionModule paslamEmissionModule;

        LineRenderer lineRenderer;

        // Use this for initialization
        void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();

            lineRenderer.positionCount = 2;

            paslamEmissionModule = plasma.emission;
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetMouseButton(0))
            {
                maskController.cylinder1.position = transform.position;
                maskController.cylinder1.normal = transform.forward;



                if(lineRenderer.enabled == false)
                    lineRenderer.enabled = true;

                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, LookAtMouse.mouseWorldPosition);


                plasma.gameObject.transform.position = LookAtMouse.mouseWorldPosition;
                paslamEmissionModule.enabled = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                lineRenderer.enabled = false;

                paslamEmissionModule.enabled = false;
            }
        }
    }
}
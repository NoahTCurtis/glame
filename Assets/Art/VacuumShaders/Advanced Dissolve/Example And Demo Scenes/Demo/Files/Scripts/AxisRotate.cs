using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedDissolve_Example
{
    public class AxisRotate : MonoBehaviour
    {
        public float rotateSpeed = 8.0f;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.Q)) RotateUpDown(true);
            if (Input.GetKey(KeyCode.A)) RotateUpDown(false);
            if (Input.GetKey(KeyCode.W)) RotateLeftRight(true);
            if (Input.GetKey(KeyCode.S)) RotateLeftRight(false);
            if (Input.GetKey(KeyCode.E)) RotateForwardBackward(true);
            if (Input.GetKey(KeyCode.D)) RotateForwardBackward(false);
        }

        void RotateUpDown(bool rUP)
        {
            float fOrbitCircumfrance = 2F * rotateSpeed * Mathf.PI;
            float fDistanceRadians = (rotateSpeed / fOrbitCircumfrance) * 2 * Mathf.PI;
            if (rUP)
            {
                transform.RotateAround(transform.position, Vector3.up, -fDistanceRadians);
            }
            else
                transform.RotateAround(transform.position, Vector3.up, fDistanceRadians);
        }

        void RotateLeftRight(bool rLeft)
        {
            float fOrbitCircumfrance = 2F * rotateSpeed * Mathf.PI;
            float fDistanceRadians = (rotateSpeed / fOrbitCircumfrance) * 2 * Mathf.PI;
            if (rLeft)
            {
                transform.RotateAround(transform.position, Vector3.right, -fDistanceRadians);
            }
            else
                transform.RotateAround(transform.position, Vector3.right, fDistanceRadians);
        }

        void RotateForwardBackward(bool rForward)
        {
            float fOrbitCircumfrance = 2F * rotateSpeed * Mathf.PI;
            float fDistanceRadians = (rotateSpeed / fOrbitCircumfrance) * 2 * Mathf.PI;
            if (rForward)
            {
                transform.RotateAround(transform.position, Vector3.forward, -fDistanceRadians);
            }
            else
                transform.RotateAround(transform.position, Vector3.forward, fDistanceRadians);
        }
    }
}
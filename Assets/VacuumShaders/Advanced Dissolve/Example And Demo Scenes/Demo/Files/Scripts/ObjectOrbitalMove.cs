using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AdvancedDissolve_Example
{
    public class ObjectOrbitalMove : MonoBehaviour
    {
        public Transform target;

        float fDistance;
        public float rotateSpeed = 10;

        public float maxDistance = 20;

        // Use this for initialization
        void Start()
        {
            fDistance = Vector3.Distance(transform.position, target.position);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.D)) RotateLeftRight(true);
            if (Input.GetKey(KeyCode.A)) RotateLeftRight(false);
            if (Input.GetKey(KeyCode.W)) RotateUpDown(true);
            if (Input.GetKey(KeyCode.S)) RotateUpDown(false);


            if (Input.GetKey(KeyCode.LeftControl))
                MoveForwardBackward();
        }

        void RotateLeftRight(bool bLeft)
        {
            float fOrbitCircumfrance = 2F * fDistance * Mathf.PI;
            float fDistanceRadians = (rotateSpeed / fOrbitCircumfrance) * 2 * Mathf.PI;
            if (bLeft)
            {
                transform.RotateAround(target.position, Vector3.up, -fDistanceRadians);
            }
            else
                transform.RotateAround(target.position, Vector3.up, fDistanceRadians);
        }

        void RotateUpDown(bool bUp)
        {
            Vector3 forwardV = (transform.position - target.position).normalized;
            Vector3 upV = transform.up;
            Vector3 rotateAxis = Vector3.Cross(forwardV, upV);

            float dot = Vector3.Dot(forwardV, Vector3.up);


            float fOrbitCircumfrance = 2F * fDistance * Mathf.PI;
            float fDistanceRadians = (rotateSpeed / fOrbitCircumfrance) * 2 * Mathf.PI;
            if (bUp)
            {
                if (dot < 0.8f)
                    transform.RotateAround(target.position, rotateAxis, fDistanceRadians);
            }
            else
            {
                if (dot > 0.1f)
                    transform.RotateAround(target.position, rotateAxis, -fDistanceRadians);
            }
        }

        void MoveForwardBackward()
        {
            float t = Vector3.Distance(transform.position, target.position) / maxDistance;

            t -= Input.mouseScrollDelta.y * 0.02f;
            t = Mathf.Clamp(t, 0.35f, 1);


            Vector3 mVector = (transform.position - target.position).normalized;

            transform.position = target.position + mVector * t * maxDistance;
        }

    }
}
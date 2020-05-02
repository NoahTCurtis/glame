using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AdvancedDissolve_Example
{
    public class UIRadiusChange : MonoBehaviour
    {
        static public UIRadiusChange get;

        public delegate void UpdateMyRadius();
        public UpdateMyRadius DelegateMethod;

        public float radius = 5;


        // Use this for initialization
        void Start()
        {
            Shader.SetGlobalFloat("_DissolveMaskRadius_Global", radius);

            get = this;
        }

        // Update is called once per frame
        void Update()
        {
            get = this;

            if (Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.KeypadPlus) || Input.GetKey(KeyCode.Equals))
            {
                radius += Time.deltaTime * 2;
                radius = Mathf.Clamp(radius, 1, 10);
            }

            if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
            {
                radius -= Time.deltaTime * 2;
                radius = Mathf.Clamp(radius, 1, 10);
            }


            //Update mask object (first mask object only)
            //Note, in demo scenes we have only one mask controller
            if (Controller_Mask_Box.get)
                Controller_Mask_Box.get.box1.transform.localScale = Vector3.one * radius;
            if (Controller_Mask_Cone.get)
                Controller_Mask_Cone.get.spotLight1.spotAngle = radius * 4;
            if (Controller_Mask_Cylinder_Alt.get)
                Controller_Mask_Cylinder_Alt.get.cylinder1.radius = radius * 0.5f;
            if (Controller_Mask_Sphere.get)
                Controller_Mask_Sphere.get.sphere1.transform.localScale = Vector3.one * radius;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedDissolve_Example
{
    public class LookAtMouse : MonoBehaviour
    {
        public static Vector3 mouseWorldPosition;

        // Use this for initialization
        void Start()
        {
            mouseWorldPosition = new Vector3(0, 8, -3);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    mouseWorldPosition = hit.point;
                }

            }

            transform.LookAt(mouseWorldPosition);
        }
    }
}
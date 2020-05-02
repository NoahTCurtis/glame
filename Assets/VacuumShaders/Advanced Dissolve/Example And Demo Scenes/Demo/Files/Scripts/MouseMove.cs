using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

namespace AdvancedDissolve_Example
{
    public class MouseMove : MonoBehaviour
    { 
        // Update is called once per frame 
        void Update()
        {
            if (Input.GetKey(KeyCode.LeftShift) == false &&
                Input.GetKey(KeyCode.RightShift) == false &&
                (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
            {
                transform.position -= transform.up * Input.mouseScrollDelta.y;
            }
        }
    } 
} 
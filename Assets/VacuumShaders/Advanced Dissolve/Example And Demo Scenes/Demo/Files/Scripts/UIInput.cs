using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace AdvancedDissolve_Example
{
    public class UIInput : MonoBehaviour
    {
        public Text fpsText;
        float deltaTime = 0.0f;
        bool displayFPS;

        public GameObject menu;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //FPS
            if (Input.GetKeyDown(KeyCode.F))
                displayFPS = !displayFPS;

            //VSync
            if (Input.GetKeyDown(KeyCode.V))
                QualitySettings.vSyncCount = (QualitySettings.vSyncCount == 0) ? 1 : 0;

            //Noise
            if (Input.GetKeyDown(KeyCode.N))
                Controller_Cutout.get.noise = (Controller_Cutout.get.noise > 0.5f) ? 0 : 1;

            //Invert
            if (Input.GetKeyDown(KeyCode.I))
            {
                if(Controller_Mask_Sphere.get)
                    Controller_Mask_Sphere.get.invert = !Controller_Mask_Sphere.get.invert;

                if (Controller_Mask_Cone.get)
                    Controller_Mask_Cone.get.invert = !Controller_Mask_Cone.get.invert;

                if (Controller_Mask_Box.get)
                    Controller_Mask_Box.get.invert = !Controller_Mask_Box.get.invert;

                if (Controller_Mask_Cylinder.get)
                    Controller_Mask_Cylinder.get.invert = !Controller_Mask_Cylinder.get.invert;

                if (Controller_Mask_Cylinder_Alt.get)
                    Controller_Mask_Cylinder_Alt.get.invert = !Controller_Mask_Cylinder_Alt.get.invert;

                if (Controller_Mask_Plane.get)
                    Controller_Mask_Plane.get.invert = !Controller_Mask_Plane.get.invert;
            }
            
            //Show/Hide Menu
            if (Input.GetKeyDown(KeyCode.H))
                menu.SetActive(!menu.activeSelf);


            UpdateFPS();
        }


        void UpdateFPS()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;


            if (displayFPS)
            {
                float fps = 1.0f / deltaTime;
                fpsText.text = (int)fps + " fps";
            }
            else
                fpsText.text = string.Empty;
        }
    }
}
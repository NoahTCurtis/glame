﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedDissolve_Example
{
    public class MinionController_Sphere : MonoBehaviour
    {
        public GameObject minion1;
        public GameObject minion2;
        public GameObject minion3;

        bool minionsAreActive;

        // Use this for initialization
        void Start()
        {
            Hide();
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.M))
            {
                if (minionsAreActive)
                    Hide();
                else
                    Show();
            }
        }

        void Show()
        {
            minion1.SetActive(true);
            minion2.SetActive(true);
            minion3.SetActive(true);

            minionsAreActive = true;

            if (Controller_Mask_Sphere.get)
                Controller_Mask_Sphere.get.UpdateMaskCountKeyword(4);
        }

        void Hide()
        {
            minion1.SetActive(false);
            minion2.SetActive(false);
            minion3.SetActive(false);

            minionsAreActive = false;

            if (Controller_Mask_Sphere.get)
                Controller_Mask_Sphere.get.UpdateMaskCountKeyword(1);

        }
    }
}
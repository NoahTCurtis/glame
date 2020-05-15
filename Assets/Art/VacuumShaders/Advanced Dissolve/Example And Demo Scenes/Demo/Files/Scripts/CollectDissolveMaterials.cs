using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedDissolve_Example
{
    [ExecuteInEditMode]
    public class CollectDissolveMaterials : MonoBehaviour
    {
        public Controller_Cutout controller_Cutout;
        public Controller_Edge controller_Edge;

        public Controller_Mask_Box controller_Mask_Box;
        public Controller_Mask_Cone controller_Mask_cone;
        public Controller_Mask_Cylinder controller_Mask_Cylinder;
        public Controller_Mask_Cylinder_Alt Controller_Mask_Cylinder_Alt;
        public Controller_Mask_Plane controller_Mask_Plane;
        public Controller_Mask_Sphere controller_Mask_Sphere;

        // Use this for initialization
        void Start()
        {
            //Find all materials with Dissolve shader
            List<Material> mats = new List<Material>();

            Renderer[] renderers = (Renderer[])Resources.FindObjectsOfTypeAll(typeof(Renderer));
            if (renderers != null)
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i] == null || renderers[i].sharedMaterials == null)
                        continue;

                    mats.AddRange(renderers[i].sharedMaterials);
                }

                //Remove null items
                mats.RemoveAll(item => item == null);

                //Remove duplicates
                mats.Distinct();

                //Remove not Dissolve shader materials
                mats.RemoveAll(item => item.shader.name.Contains("VacuumShaders/Advanced Dissolve") == false);                
            }


            if (controller_Cutout)
                controller_Cutout.materials = mats.ToArray();

            if (controller_Edge)
                controller_Edge.materials = mats.ToArray();


            if (controller_Mask_Box)
                controller_Mask_Box.materials = mats.ToArray();
            if (controller_Mask_cone)
                controller_Mask_cone.materials = mats.ToArray();
            if (controller_Mask_Cylinder)
                controller_Mask_Cylinder.materials = mats.ToArray();
            if (Controller_Mask_Cylinder_Alt)
                Controller_Mask_Cylinder_Alt.materials = mats.ToArray();
            if (controller_Mask_Plane)
                controller_Mask_Plane.materials = mats.ToArray();
            if (controller_Mask_Sphere)
                controller_Mask_Sphere.materials = mats.ToArray();
        }
    }
}
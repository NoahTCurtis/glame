using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedDissolve_Example
{
    public class OnScreenMove : MonoBehaviour
    {
        [Space(10)]
        public Transform target;

        public float speed = 8.0f;
        public float distanceFromCamera = 5.0f;

        public float minDistance = 5;
        public float maxDistance = 30;
        

        Vector3 initialPosition;

        void Start()
        {
            initialPosition = transform.position;
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                Vector3 mousePosition = Input.mousePosition;
                mousePosition.z = distanceFromCamera + (Vector3.Distance(Camera.main.transform.position, target.position) - 20);

                initialPosition = Camera.main.ScreenToWorldPoint(mousePosition);

                Vector3 position = Vector3.Lerp(transform.position, initialPosition, 1.0f - Mathf.Exp(-speed * Time.deltaTime));

                if (position.y < 8)
                    position.y = 8;

                transform.position = position;

                distanceFromCamera += Input.mouseScrollDelta.y;
                distanceFromCamera = Mathf.Clamp(distanceFromCamera, minDistance, maxDistance);
            }
            else
            {
                Vector3 position = Vector3.Lerp(transform.position, initialPosition, 1.0f - Mathf.Exp(-speed * Time.deltaTime));

                if (position.y < 8)
                    position.y = 8;

                transform.position = position;
            }
        }
    }
}
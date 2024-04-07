using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


namespace Thrash.Lab5
{
    public class RaycastVision : MonoBehaviour
    {
        [SerializeField] private Material lineMaterial;
        [SerializeField] private List<Transform> visibleObjects;
        public List<Transform> VisibleObjects => visibleObjects;

        [SerializeField] private (Transform obj, Vector3 location) nearest;
        public (Transform obj, Vector3 obj_location) Nearest => nearest;

        [SerializeField] private (Transform obj, Vector3 location) second;
        public (Transform obj, Vector3 obj_location) Second => second;

        [SerializeField] private int numberOfRays;

        private LineRenderer lines;

        //field of view
        private float FOV;

        private float distance;
        private List<Vector3> rays;


        public void Initialize(float maxAngle, float maxDistance, Color lineColor)
        {
            FOV = maxAngle;
            distance = maxDistance;

            visibleObjects = new List<Transform>();
            lines = transform.AddComponent<LineRenderer>();
            lines.startWidth = lines.endWidth = 0.05f;
            lines.startColor = lines.endColor = lineColor;
            lines.material = lineMaterial;


            // list of rays used for raycasting and drawing field of vision
            rays = new List<Vector3>();
            for (float i = -FOV; i <= FOV; i += FOV * 2 / numberOfRays)
            {
                rays.Add(Quaternion.AngleAxis(i, Vector3.up) * transform.forward);
            }

            lines.positionCount = rays.Count * 2;

        }


        // Update is called once per frame
        void Update()
        {
            rays = new List<Vector3>();
            for (float i = -FOV; i <= FOV; i += FOV * 2 / numberOfRays)
            {
                rays.Add(Quaternion.AngleAxis(i, Vector3.up) * transform.forward);
            }


            //draw rays
            int index = 0;
            foreach (Vector3 ray in rays)
            {
                lines.SetPosition(index, transform.position);
                lines.SetPosition(index + 1, transform.position + ray * distance);
                index += 2;
            }

            //cast rays to detect objects, updating nearest and second nearest objects

            RaycastHit hit;

            List<Transform> currentlyVisible = new List<Transform>();

            float nearestDistance = int.MaxValue;
            float secondNearestDistance = int.MaxValue;
            nearest = (null, Vector3.zero);
            second = (null, Vector3.zero);
            foreach (Vector3 ray in rays)
            {
                Ray raycast = new Ray(transform.position, ray);

                if (Physics.Raycast(raycast, out hit, distance))
                {
                    Transform obj = hit.transform;
                    float distance = hit.distance;
                    if (!currentlyVisible.Contains(obj))
                    {
                        currentlyVisible.Add(obj);
                    }

                    if (!visibleObjects.Contains(obj))
                    {
                        visibleObjects.Add(obj);
                    }

                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearest = (obj, hit.point);
                    }
                    else if (distance < secondNearestDistance && obj != nearest.obj)
                    {
                        secondNearestDistance = distance;
                        second = (obj, hit.point);
                    }

                }
            }
            visibleObjects.RemoveAll(item => !currentlyVisible.Contains(item));

        }

        //check if any objects with "tag" are in view
        public bool InSight(string tag)
        {
            foreach(Transform t in visibleObjects)
            {
                if (t.CompareTag(tag))
                    return true;
            }
            return false;
            //return nearest.obj != null && nearest.obj.tag == tag
            //    || second.obj != null && second.obj.tag == tag;
        }

        //check if given line of sight is blocked
        public bool PathBlocked(Vector3 origin, Vector3 direction)
        {
            Ray ray = new Ray(origin, direction);
            return Physics.Raycast(ray, distance);
        }

    }
}
using System;
using UnityEngine;

namespace Runtime
{
    public class OceHandle : MonoBehaviour
    {
        public Transform controllerTransform;
        public GameObject model;
        public LineRenderer lineRenderer;

        public Vector3 planeNormal;
        public Vector3 planeOrigin;


        public bool drawGizmos = false;

        [HideInInspector]
        public Vector3 closestPointOnPlane = Vector3.zero;

        [HideInInspector]
        public Vector2 projectedPoint = Vector2.zero;

        private Plane _plane;
        private Matrix4x4 _projectionMatrix;


        // Start is called before the first frame update
        void Start()
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });

            _plane = new Plane(planeNormal, planeOrigin);
        }

        private void Update()
        {
            UpdateTranslatedInput();
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, closestPointOnPlane);
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmos) return;

            Quaternion rotation = Quaternion.LookRotation(_plane.normal);
            Matrix4x4 trs = Matrix4x4.TRS(planeOrigin, rotation, Vector3.one);
            Gizmos.matrix = trs;
            Color32 color = Color.blue;
            color.a = 125;
            Gizmos.color = color;
            Gizmos.DrawCube(Vector3.zero, new Vector3(1.0f, 1.0f, 0.0001f));
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.white;

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(closestPointOnPlane, 0.01f);
        }

        private void UpdateTranslatedInput()
        {
            //the translated input is the lineRenderers direction projected onto a plane projected
            // https://discussions.unity.com/t/project-a-vector3-onto-a-plane-orthographically/152982
            //https://www.youtube.com/watch?v=TyAlFihknfQ
            //https://forum.unity.com/threads/get-x-and-y-coordinates-of-point-in-plane.964301/
            closestPointOnPlane = _plane.ClosestPointOnPlane(controllerTransform.position);
            projectedPoint = ProjectPointOntoPlane(closestPointOnPlane, planeOrigin, planeNormal);
            Debug.Log($"x:{projectedPoint.x} y:{projectedPoint.y}");
        }

        public static Vector2 ProjectPointOntoPlane(Vector3 point, Vector3 planePoint, Vector3 planeNormal)
        {
            // Calculate the vector from the plane point to the point to be projected
            Vector3 pointToPlanePoint = point - planePoint;

            // Calculate the distance from the point to the plane along the normal vector
            float distance = Vector3.Dot(pointToPlanePoint, planeNormal);

            // Calculate the projected point in 3D
            Vector3 projectedPoint3D = point - distance * planeNormal;

            // Define two orthogonal vectors on the plane
            Vector3 right = Vector3.Cross(Vector3.up, planeNormal).normalized;
            Vector3 up = Vector3.Cross(planeNormal, right).normalized;

            // Convert the 3D projected point to 2D coordinates
            Vector2 projectedPoint2D = new Vector2(
                Vector3.Dot(projectedPoint3D - planePoint, right),
                Vector3.Dot(projectedPoint3D - planePoint, up)
            );

            return projectedPoint2D;
        }

        public void SetDiameter(float d)
        {
            model.transform.localScale = Vector3.one * d;
            lineRenderer.startWidth = d / 2f;
        }
    }
}
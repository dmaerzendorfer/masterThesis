using UnityEngine;

namespace Runtime
{
    public class OceHandle : MonoBehaviour
    {
        public Transform lineTarget;
        public GameObject model;
        public LineRenderer lineRenderer;

        public Transform planeOrigin;
        public Transform planeLookingTowards;
        public Transform objectInCenter;

        [HideInInspector]
        public Vector3 closestPointOnPlane = Vector3.zero;
        [HideInInspector]
        public Vector2 projectedPoint = Vector2.zero;


        // Start is called before the first frame update
        void Start()
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });
        }

        private void Update()
        {
            UpdateTranslatedInput();
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, closestPointOnPlane);
        }

        private void UpdateTranslatedInput()
        {
            //the translated input is the lineRenderers direction projected onto a plane projected
            // https://discussions.unity.com/t/project-a-vector3-onto-a-plane-orthographically/152982
            //https://www.youtube.com/watch?v=TyAlFihknfQ
            //https://forum.unity.com/threads/get-x-and-y-coordinates-of-point-in-plane.964301/
            
            var planeNormal = planeLookingTowards.position - objectInCenter.position;
            Plane plane = new Plane(planeNormal, planeOrigin.position);
            
            closestPointOnPlane = plane.ClosestPointOnPlane(lineTarget.position);
            projectedPoint = ProjectVector3ToPlane(closestPointOnPlane, planeNormal, planeOrigin.position);
            Debug.Log($"x:{projectedPoint.x} y:{projectedPoint.y}");
        }
        // Function to project a Vector3 onto an arbitrary plane and convert it to a Vector2
        public static Vector2 ProjectVector3ToPlane(Vector3 vector, Vector3 planeNormal, Vector3 planePoint)
        {
            // Step 1: Project the vector onto the plane
            Vector3 projectedVector = vector - Vector3.Dot(vector - planePoint, planeNormal) * planeNormal;

            // Step 2: Define a local coordinate system on the plane
            // Choose an arbitrary vector that is not parallel to the plane normal
            Vector3 arbitraryVector = (planeNormal != Vector3.up) ? Vector3.up : Vector3.forward;
            Vector3 tangent1 = Vector3.Cross(planeNormal, arbitraryVector).normalized;
            Vector3 tangent2 = Vector3.Cross(planeNormal, tangent1).normalized;

            // Step 3: Convert the projected vector to 2D coordinates
            Vector2 result = new Vector2(Vector3.Dot(projectedVector, tangent1), Vector3.Dot(projectedVector, tangent2));

            return result;
        }

        public void SetDiameter(float d)
        {
            model.transform.localScale = Vector3.one * d;
            lineRenderer.startWidth = d / 2f;
        }
    }
}
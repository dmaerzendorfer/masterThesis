using System.Collections.Generic;
using System.Linq;
using _Generics.Scripts.Runtime;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace Runtime.View
{
    public class ViewData
    {
        public XRGrabInteractable interactable;
        public ViewPanel viewPanel;
        public bool inHud = false;
        public Vector3 originalScale; //so we can scale it back to its original once it leaves the hud
        public Image backgroundImage;

        public Sequence popSequence = DOTween.Sequence();
    }

    public class ViewManager : SingletonMonoBehaviour<ViewManager>
    {
        public GameObject viewParent;

        public float hudDistanceFromCamera = 1f;

        [BoxGroup("ColorSettings")]
        public Color worldColor = Color.black;

        [BoxGroup("ColorSettings")]
        public Color hudColor = Color.gray;

        private List<ViewData> _views = new List<ViewData>();
        private Camera _mainCam;

        public override void Awake()
        {
            base.Awake();
            _mainCam = Camera.main;
        }

        private void OnDestroy()
        {
            _views.ForEach(x => x.popSequence.Kill());
        }

        public void OnViewWindowActivate(ActivateEventArgs args)
        {
            var view = _views.Where(x => x.interactable == args.interactableObject).FirstOrDefault();
            if (view == null)
            {
                //its a new view -> add it to the list
                view = new ViewData();
                _views.Add(view);
                view.viewPanel = args.interactableObject.transform.GetComponent<ViewPanel>();

                view.interactable = (XRGrabInteractable)args.interactableObject;
                view.inHud = true; //will move it into the hud now
                view.viewPanel.IsInHud = true;
                view.originalScale = args.interactableObject.transform.localScale;
                //move into the hud
                args.interactableObject.transform.parent = viewParent.transform;

                //change color
                view.backgroundImage = view.interactable.GetComponentInChildren<Image>();
                view.backgroundImage.color = hudColor;
            }
            else
            {
                //view already in list

                //move out of hud
                if (view.inHud)
                {
                    //move it out of hud
                    view.interactable.transform.parent = null;
                    view.inHud = false;
                    view.viewPanel.IsInHud = false;

                    //set to original scale again
                    view.interactable.transform.localScale = view.originalScale;

                    //change color
                    view.backgroundImage.color = worldColor;
                }
                //move into the hud
                else
                {
                    view.inHud = true; //will move it into the hud now
                    view.viewPanel.IsInHud = true;

                    view.originalScale = args.interactableObject.transform.localScale;
                    //move into the hud
                    args.interactableObject.transform.parent = viewParent.transform;
                    //change color
                    view.backgroundImage.color = hudColor;
                }
            }
        }

        public void OnViewWindowSelectionExit(SelectExitEventArgs args)
        {
            // //check if view is in list
            var view = _views.Where(x => x.interactable == args.interactableObject).FirstOrDefault();
            if (view == null) return;
            if (view.inHud)
            {
                //if so make sure to set the parent if also in hud (since the grab interactible reverts its parent once its let go)
                //(since it keeps track of the transform parent, which is fine but sometimes isnt) 
                view.interactable.transform.parent = viewParent.transform;
            }
            else
            {
                view.interactable.transform.parent = null;
            }
            // MoveViewIntoHudWithScaling(view);
        }

        /// <summary>
        /// Moves the given view into the hud and makes sure perceived scale stays the same.
        /// </summary>
        /// <param name="v"></param>
        private void MoveViewIntoHudWithScaling(ViewData v)
        {
            //todo: fix this, its not working
            var newPosition = _mainCam.transform.position +
                              (v.interactable.gameObject.transform.position - _mainCam.transform.position) *
                              hudDistanceFromCamera;

            // Calculate the initial distance from the camera to the object
            float initialDistance =
                Vector3.Distance(_mainCam.transform.position, v.interactable.gameObject.transform.position);

            // Calculate the new distance from the camera to the new position
            float newDistance = Vector3.Distance(_mainCam.transform.position, newPosition);

            // Calculate the scale factor
            // float scaleFactor = initialDistance / newDistance; //wrong?
            float scaleFactor = newDistance / initialDistance;

            // Move the object to the new position
            v.interactable.transform.position = newPosition;

            // Apply the scale factor to the object's scale
            v.interactable.transform.transform.localScale = v.interactable.transform.localScale * scaleFactor;
        }
    }
}
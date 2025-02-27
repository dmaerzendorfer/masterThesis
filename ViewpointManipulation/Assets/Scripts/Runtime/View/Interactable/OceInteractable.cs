﻿using NaughtyAttributes;
using Runtime.View.Manager;
using Runtime.View.ViewPair;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Runtime.View.Interactable
{
    /// <summary>
    /// An interactable object that can be selected to spawn an OCE-Cam focusing on it.
    /// </summary>
    public class OceInteractable : XRGrabInteractable
    {
        [HorizontalLine(color: EColor.Red)]
        public Transform lookAtTarget;

        private ViewManager _viewManager;
        private OrbitViewPair _mostRecentOrbitViewPair = null;

        private void Start()
        {
            _viewManager = ViewManager.Instance;
        }


        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);

            //spawn orbit cam and set lookat target
            if (_viewManager.ViewMode != ViewMode.OCE) return;
            _mostRecentOrbitViewPair = (OrbitViewPair)_viewManager.SpawnViewPair();
            if (_mostRecentOrbitViewPair == null)
                return;
            _mostRecentOrbitViewPair.orbitCamController.SetTarget(lookAtTarget);
            var camInteractable = _mostRecentOrbitViewPair.orbitCamController.GetComponent<OceCamInteractable>();
            args.interactableObject = camInteractable;
            camInteractable.CallOnSelectEntered(args);

            //move the view panel to an opportune position
            _viewManager.AdjustNewViewPanelPosition(_mostRecentOrbitViewPair);
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            if (_mostRecentOrbitViewPair == null) return;
            var camInteractable = _mostRecentOrbitViewPair.orbitCamController.GetComponent<OceCamInteractable>();
            args.interactableObject = camInteractable;
            camInteractable.CallOnSelectExited(args);
        }
    }
}
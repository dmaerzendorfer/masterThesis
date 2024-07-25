using NaughtyAttributes;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Runtime.View
{
    /// <summary>
    /// The object in center
    /// </summary>
    public class OceInteractable : XRGrabInteractable
    {
        [HorizontalLine(color: EColor.Red)]
        public OceActions actionsLeft;

        public OceActions actionsRight;

        [HorizontalLine(color: EColor.Red)]
        public Transform lookAtTarget;

        private ViewManager _viewManager;
        private OceViewPair _mostRecentOceViewPair = null;
        
        private void Start()
        {
            _viewManager = ViewManager.Instance;
        }


        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);

            //spawn orbit cam and set lookat target
            _mostRecentOceViewPair = _viewManager.SpawnOce();
            if (_mostRecentOceViewPair == null)
                return;
            _mostRecentOceViewPair.orbitCamController.SetTarget(lookAtTarget);
            var camInteractable = _mostRecentOceViewPair.orbitCamController.GetComponent<OceCamInteractable>();
            args.interactableObject = camInteractable;
            camInteractable.CallOnSelectEntered(args);

            //move the view panel to an opportune position
            _viewManager.AdjustNewViewPanelPosition(_mostRecentOceViewPair);
        }
        
        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            if (_mostRecentOceViewPair == null) return;
            var camInteractable = _mostRecentOceViewPair.orbitCamController.GetComponent<OceCamInteractable>();
            args.interactableObject = camInteractable;
            camInteractable.CallOnSelectExited(args);
        }
       
    }
}
using NaughtyAttributes;
using Runtime.CameraControl;
using UnityEngine.XR.Interaction.Toolkit;

namespace Runtime.View
{
    /// <summary>
    /// The object in center
    /// </summary>
    public class DroneInteractable : XRGrabInteractable
    {
        [HorizontalLine(color: EColor.Red)]
        public DroneCamController droneCamController;

        private ViewManager _viewManager;

        private void Start()
        {
            _viewManager = ViewManager.Instance;
        }


        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);

            if (droneCamController.IsSelected)
            {
                droneCamController.IsSelected = false;
                return;
            }

            _viewManager.droneViewConfigs.ForEach(x =>
            {
                if (x.instance != null)
                    x.instance.droneCamController.IsSelected = false;
            });
            droneCamController.IsSelected = true;
        }
    }
}
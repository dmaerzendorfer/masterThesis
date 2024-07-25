using Runtime.CameraControl;

namespace Runtime.View
{
    public class DroneViewPair : BaseViewPair
    {
        public DroneCamController droneCamController;

        public override void ReceiveSelect()
        {
            droneCamController.IsSelected = !droneCamController.IsSelected;
        }
    }
}
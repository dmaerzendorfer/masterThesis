using Runtime.CameraControl;

namespace Runtime.View.ViewPair
{
    public class HoverViewPair : BaseViewPair
    {
        public HoverCamController hoverCamController;

        public override void ReceiveSelect()
        {
            hoverCamController.IsSelected = !hoverCamController.IsSelected;
        }

        public override void DeleteViewPair()
        {
            hoverCamController.IsSelected = false;
            base.DeleteViewPair();
        }
    }
}
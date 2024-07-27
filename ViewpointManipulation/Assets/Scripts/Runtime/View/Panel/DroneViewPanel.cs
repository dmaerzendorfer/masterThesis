using Runtime.View.ViewPair;
using TMPro;

namespace Runtime.View.Panel
{
    public class DroneViewPanel : BaseViewPanel
    {
        public TextMeshProUGUI movementToggleText;
        private DroneViewPair _droneViewPair;

        public override void Start()
        {
            base.Start();
            _droneViewPair = (DroneViewPair)myViewPair;
        }

        public void OnToggleMovementMode()
        {
            if (myViewPair)
            {
                movementToggleText.text = _droneViewPair.droneCamController.ToggleMovementMode();
            }
        }
    }
}
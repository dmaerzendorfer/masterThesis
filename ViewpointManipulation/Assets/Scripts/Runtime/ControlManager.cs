using Runtime.View.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime
{
    public class ControlManager : MonoBehaviour
    {
        public ViewManager viewManager;

        public Text camCountText;
        public Text viewModeText;
        public Text viewModeTextButton;

        private void Start()
        {
            UpdateActiveCamCountDisplay();
            viewManager.onAnyCamDestroyed.AddListener(UpdateActiveCamCountDisplay);
        }

        public void ToggleMode()
        {
            var currentMode = viewManager.ViewMode;
            viewManager.ViewMode = currentMode == ViewMode.Drone ? ViewMode.OCE : ViewMode.Drone;

            string mode = viewManager.ViewMode == ViewMode.Drone ? "Drone" : "OCE";
            viewModeText.text = $"Current Mode: {mode}";
            viewModeTextButton.text = mode;
        }

        public void DeleteAllCams()
        {
            viewManager.DeleteAllActiveViews();
        }

        public void StartStudy()
        {
            //not yet implemented
        }

        private void UpdateActiveCamCountDisplay()
        {
            camCountText.text = $"Active Cams: {viewManager.CurrentActiveViewCount}/5";
        }
    }
}
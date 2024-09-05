using System.Linq;
using Runtime.View.ViewPair;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.View.Manager.ViewModeHandler
{
    public class HoverViewModeHandler : BaseViewModeHandler<HoverViewPair>
    {
        public HoverCamSpawner hoverCamSpawner;
        public InputActionReference hoverCamUnselectAction;

        private Camera _mainCam;
        private ViewManager _viewManager;
        private bool _isActivated = false;

        private void Start()
        {
            _mainCam = Camera.main;
            _viewManager = ViewManager.Instance;
            hoverCamUnselectAction.action.performed += OnHoverCamUnselect;
        }

        private void OnDestroy()
        {
            hoverCamUnselectAction.action.performed -= OnHoverCamUnselect;
        }

        public override HoverViewPair SpawnViewPair()
        {
            if (!_isActivated) return null;

            //first check if there is a currently selected one
            var selected = viewConfigs.FirstOrDefault(x =>
            {
                if (x.instance != null)
                    return x.instance.hoverCamController.IsSelected;
                return false;
            });
            if (selected != null) return selected.instance;

            //if not create a new one if we are not yet at the limit
            var config = viewConfigs.FirstOrDefault(x => x.instance == null);
            if (config == null) return null;
            config.instance = Instantiate(config.prefab);
            config.instance.basePanel.panelText.text = config.panelTitle;
            config.instance.hoverCamController.IsSelected = true;
            _viewManager.AdjustNewViewPanelPosition(config.instance);
            return config.instance;
        }

        public override void Activate()
        {
            _isActivated = true;
            hoverCamSpawner.isSpawningEnabled = true;
            hoverCamUnselectAction.action.Enable();
        }

        public override void Deactivate()
        {
            base.Deactivate();
            _isActivated = false;
            hoverCamSpawner.isSpawningEnabled = false;
            hoverCamUnselectAction.action.Disable();
        }

        private void OnHoverCamUnselect(InputAction.CallbackContext callbackContext)
        {
            foreach (var hoverCamViewConfig in viewConfigs)
            {
                if (hoverCamViewConfig.instance != null)
                {
                    hoverCamViewConfig.instance.hoverCamController.IsSelected = false;
                }
            }
        }
    }
}
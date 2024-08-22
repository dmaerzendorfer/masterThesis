using System.Linq;
using Runtime.View.ViewPair;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.View.Manager.ViewModeHandler
{
    public class DroneViewModeHandler : BaseViewModeHandler<DroneViewPair>
    {
        public InputActionReference droneSpawnAction;

        public InputActionReference droneUnselectAction;
        public Transform droneSpawnLocation;
        public float droneSpawningDistance = 1f;

        private Camera _mainCam;
        private ViewManager _viewManager;
        private bool _isActivated = false;

        private void Start()
        {
            _mainCam = Camera.main;
            _viewManager = ViewManager.Instance;
            droneSpawnAction.action.performed += OnDroneSpawn;
            droneUnselectAction.action.performed += OnDroneUnselect;
        }

        private void OnDestroy()
        {
            droneSpawnAction.action.performed -= OnDroneSpawn;
            droneUnselectAction.action.performed -= OnDroneUnselect;
        }

        public override DroneViewPair SpawnViewPair()
        {
            if (!_isActivated) return null;

            var config = viewConfigs.FirstOrDefault(x => x.instance == null);
            if (config == null) return null;
            config.instance = Instantiate(config.prefab);
            config.instance.basePanel.panelText.text = config.panelTitle;
            return config.instance;
        }

        public override void Activate()
        {
            _isActivated = true;
            droneSpawnAction.action.Enable();
        }

        public override void Deactivate()
        {
            base.Deactivate();
            _isActivated = false;
            droneSpawnAction.action.Disable();
        }

        private void OnDroneUnselect(InputAction.CallbackContext callbackContext)
        {
            foreach (var droneViewConfig in viewConfigs)
            {
                if (droneViewConfig.instance != null)
                {
                    droneViewConfig.instance.droneCamController.IsSelected = false;
                }
            }
        }

        public void AdjustNewDronePosition(DroneViewPair viewPair)
        {
            //set panel pos
            RaycastHit hit;
            var transf = viewPair.droneCamController.transform;

            if (Physics.Raycast(transf.position, _mainCam.transform.forward, out hit, droneSpawningDistance))
            {
                transf.position = hit.point;
            }
            else
            {
                transf.position += _mainCam.transform.forward * droneSpawningDistance;
            }
        }

        private void OnDroneSpawn(InputAction.CallbackContext callbackContext)
        {
            OnDroneUnselect(callbackContext);

            var dronePair = SpawnViewPair();
            if (dronePair == null) return;
            //set drone pos
            var transf = dronePair.droneCamController.transform;
            transf.position = droneSpawnLocation.position;
            transf.forward = _mainCam.transform.forward;
            //then adjust pos
            AdjustNewDronePosition(dronePair);
            //then adjust view panel pos
            _viewManager.AdjustNewViewPanelPosition(dronePair);

            //activateDrone
            dronePair.droneCamController.IsSelected = true;
        }
    }
}
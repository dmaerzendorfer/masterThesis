using System;
using System.Collections.Generic;
using System.Linq;
using _Generics.Scripts.Runtime;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace Runtime.View
{
    public class ViewPanelData
    {
        public XRGrabInteractable interactable;
        public ViewPanel viewPanel;
        public bool inHud = false;
        public Vector3 originalScale; //so we can scale it back to its original once it leaves the hud
        public Image backgroundImage;
    }

    public enum ViewMode
    {
        OCE = 0,
        Drone = 1 << 0
    }

    [Serializable]
    public class OceViewConfig
    {
        public OceViewPair prefab;

        [HideInInspector]
        public OceViewPair instance = null;
    }

    [Serializable]
    public class DroneViewConfig
    {
        public DroneViewPair prefab;

        [HideInInspector]
        public DroneViewPair instance = null;
    }


    public class ViewManager : SingletonMonoBehaviour<ViewManager>
    {
        [Foldout("ViewSpawning")]
        [SerializeField]
        private ViewMode _viewMode = ViewMode.OCE;

        public ViewMode ViewMode
        {
            get { return _viewMode; }
            set
            {
                _viewMode = value;
                //make sure to delete any views of the incorrect mode on changing
                switch (_viewMode)
                {
                    case ViewMode.OCE:
                        droneViewConfigs.ForEach(x =>
                        {
                            Destroy(x.instance.gameObject);
                            x.instance = null;
                        });
                        droneSpawnAction.Disable();
                        break;
                    case ViewMode.Drone:
                        oceViewConfigs.ForEach(x =>
                        {
                            Destroy(x.instance.gameObject);
                            x.instance = null;
                        });
                        droneSpawnAction.Enable();
                        break;
                    default:
                        return;
                }
            }
        }

        [Foldout("ViewSpawning")]
        public List<OceViewConfig> oceViewConfigs;

        [Foldout("ViewSpawning")]
        public List<DroneViewConfig> droneViewConfigs;

        [Foldout("ViewSpawning")]
        public InputAction droneSpawnAction;

        [Foldout("ViewSpawning")]
        public Transform droneSpawnLocation;

        [Foldout("ViewSpawning")]
        public float viewPanelDistance = 3.5f;

        [Foldout("ViewPanels")]
        public GameObject viewParent;

        [Foldout("ViewPanels")]
        public float hudDistanceFromCamera = 1f;

        [Foldout("ViewPanels")]
        public Color worldColor = Color.black;

        [Foldout("ViewPanels")]
        public Color hudColor = Color.gray;

        private List<ViewPanelData> _viewPanels = new List<ViewPanelData>();
        private Camera _mainCam;

        public override void Awake()
        {
            base.Awake();
            _mainCam = Camera.main;
            //to apply the viewmode which is selected in the inspector
            ViewMode = _viewMode;
            droneSpawnAction.performed += OnDroneSpawn;
        }

        private void OnDestroy()
        {
            droneSpawnAction.performed -= OnDroneSpawn;
        }

        public BaseViewPair SpawnViewPair()
        {
            if (ViewMode == ViewMode.Drone)
            {
                return SpawnDrone();
            }
            else
            {
                return SpawnOce();
            }
        }

        /// <summary>
        /// Spawns a oce viewpair if possible. otherwise returns null.
        /// </summary>
        /// <returns></returns>
        public OceViewPair SpawnOce()
        {
            var config = oceViewConfigs.FirstOrDefault(x => x.instance == null);
            if (config == null) return null;
            config.instance = Instantiate(config.prefab);
            return config.instance;
        }

        /// <summary>
        /// Spawns a drone viewpair if possible. otherwise returns null.
        /// </summary>
        /// <returns></returns>
        public DroneViewPair SpawnDrone()
        {
            var config = droneViewConfigs.FirstOrDefault(x => x.instance == null);
            if (config == null) return null;
            config.instance = Instantiate(config.prefab);
            return config.instance;
        }


        public void OnViewWindowActivate(ActivateEventArgs args)
        {
            var view = _viewPanels.Where(x => x.interactable == args.interactableObject).FirstOrDefault();
            if (view == null)
            {
                //its a new view -> add it to the list
                view = new ViewPanelData();
                _viewPanels.Add(view);
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
            var view = _viewPanels.Where(x => x.interactable == args.interactableObject).FirstOrDefault();
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

        public void AdjustNewViewPanelPosition(BaseViewPair viewPair)
        {
            //set panel pos
            RaycastHit hit;
            if (Physics.Raycast(_mainCam.transform.position, _mainCam.transform.forward, out hit, viewPanelDistance))
            {
                viewPair.viewPanel.transform.position = hit.point;
            }
            else
            {
                viewPair.viewPanel.transform.position =
                    _mainCam.transform.position + _mainCam.transform.forward * viewPanelDistance;
            }
        }

        private void OnDroneSpawn(InputAction.CallbackContext callbackContext)
        {
            var dronePair = SpawnDrone();
            if (dronePair == null) return;
            //set drone pos
            var transf = dronePair.droneCamController.transform;
            transf.position = droneSpawnLocation.position;
            transf.forward = _mainCam.transform.forward;

            AdjustNewViewPanelPosition(dronePair);
        }

        /// <summary>
        /// Moves the given view into the hud and makes sure perceived scale stays the same.
        /// </summary>
        /// <param name="v"></param>
        private void MoveViewIntoHudWithScaling(ViewPanelData v)
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
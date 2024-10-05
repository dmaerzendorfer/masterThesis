using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Runtime.View.Manager.ViewModeHandler;
using Runtime.View.Panel;
using Runtime.View.ViewPair;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace Runtime.View.Manager
{
    public class ViewPanelData
    {
        public XRGrabInteractable interactable;
        public BaseViewPanel BasePanel;
        public Vector3 originalScale; //so we can scale it back to its original once it leaves the hud
        public Image backgroundImage;
    }

    public enum ViewMode
    {
        OCE = 0,
        Drone = 1 << 0,
        Hover = 1 << 1
    }


    /// <summary>
    /// Manages the spawning of OCE ViewPairs and Drone ViewPairs. In addition manages the moving of view panels in and out of the 'HUD'.
    /// </summary>
    public class ViewManager : SingletonMonoBehaviour<ViewManager>
    {
        #region ViewSpawningSettings

        [Foldout("ViewSpawning")]
        [SerializeField]
        private ViewMode _viewMode = ViewMode.Hover;

        public ViewMode ViewMode
        {
            get { return _viewMode; }
            set
            {
                _viewMode = value;
                switch (_viewMode)
                {
                    case ViewMode.OCE:
                        droneViewModeHandler.Deactivate();
                        hoverViewModeHandler.Deactivate();

                        oceViewModeHandler.Activate();
                        break;
                    case ViewMode.Drone:
                        hoverViewModeHandler.Deactivate();
                        oceViewModeHandler.Deactivate();

                        droneViewModeHandler.Activate();
                        break;
                    case ViewMode.Hover:
                        oceViewModeHandler.Deactivate();
                        droneViewModeHandler.Deactivate();

                        hoverViewModeHandler.Activate();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        [Foldout("ViewSpawning")]
        public DroneViewModeHandler droneViewModeHandler;

        [Foldout("ViewSpawning")]
        public OceViewModeHandler oceViewModeHandler;

        [Foldout("ViewSpawning")]
        public HoverViewModeHandler hoverViewModeHandler;

        #endregion

        #region ViewPanel Settings

        [Foldout("ViewPanels")]
        public float viewPanelDistance = 2f;

        [Foldout("ViewPanels")]
        public GameObject viewParent;

        [Foldout("ViewPanels")]
        public Color worldColor = Color.black;

        [Foldout("ViewPanels")]
        public Color hudColor = Color.gray;

        #endregion

        [Foldout("Events")]
        public UnityEvent onAnyCamDestroyed = new UnityEvent();

        [Foldout("Events")]
        public UnityEvent onAnyCamSpawned = new UnityEvent();

        [Foldout("Events")]
        public UnityEvent<DroneViewPair> onDroneCamSpawned = new UnityEvent<DroneViewPair>();

        [Foldout("Events")]
        public UnityEvent onViewPanelDocked = new UnityEvent();

        [Foldout("Events")]
        public UnityEvent onViewPanelUndocked = new UnityEvent();

        public int CurrentActiveViewCount
        {
            get
            {
                switch (_viewMode)
                {
                    case ViewMode.OCE:
                        return oceViewModeHandler.CurrentActiveViewCount;
                    case ViewMode.Drone:
                        return droneViewModeHandler.CurrentActiveViewCount;
                    case ViewMode.Hover:
                        return hoverViewModeHandler.CurrentActiveViewCount;
                    default:
                        return 0;
                }
            }
        }

        private List<ViewPanelData> _viewPanels = new List<ViewPanelData>();
        private Camera _mainCam;

        public override void Awake()
        {
            base.Awake();
            _mainCam = Camera.main;
            //to apply the viewmode which is selected in the inspector
            ViewMode = _viewMode;
        }

        public BaseViewPair SpawnViewPair()
        {
            switch (ViewMode)
            {
                case ViewMode.OCE:
                    var ocePair = oceViewModeHandler.SpawnViewPair();
                    if (ocePair != null)
                    {
                        ocePair.onViewPairDeleted.AddListener(() => onAnyCamDestroyed.Invoke());
                        onAnyCamSpawned.Invoke();
                    }

                    return ocePair;
                    break;
                case ViewMode.Drone:
                    var dronePair = droneViewModeHandler.SpawnViewPair();
                    if (dronePair != null)
                    {
                        dronePair.onViewPairDeleted.AddListener(() => onAnyCamDestroyed.Invoke());
                        onAnyCamSpawned.Invoke();
                        onDroneCamSpawned.Invoke(dronePair);
                    }

                    return dronePair;
                    break;
                case ViewMode.Hover:
                    var hoverPair = hoverViewModeHandler.SpawnViewPair();
                    if (hoverPair != null)
                    {
                        hoverPair.onViewPairDeleted.AddListener(() => onAnyCamDestroyed.Invoke());
                        onAnyCamSpawned.Invoke();
                    }

                    return hoverPair;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// For when a viewPanel is activated. Handles pinning the panel into the "hud" (just makes it a child of the main so it moves with the FoV)
        /// </summary>
        /// <param name="args"></param>
        public void OnViewPanelActivate(ActivateEventArgs args)
        {
            var view = _viewPanels.Where(x => x.interactable == args.interactableObject).FirstOrDefault();
            if (view == null)
            {
                //its a new view -> add it to the list
                view = new ViewPanelData();
                _viewPanels.Add(view);
                view.BasePanel = args.interactableObject.transform.GetComponent<BaseViewPanel>();
                view.interactable = (XRGrabInteractable)args.interactableObject;
                view.backgroundImage = view.interactable.GetComponentInChildren<Image>();
            }

            //view already in list
            //move out of hud if currently in hud
            if (view.BasePanel.IsInHud)
            {
                //move it out of hud
                view.interactable.transform.parent = null;
                view.BasePanel.IsInHud = false;

                //set to original scale again
                view.interactable.transform.localScale = view.originalScale;

                //change color
                view.backgroundImage.color = worldColor;

                onViewPanelUndocked.Invoke();
            }
            //otherwise move into the hud
            else
            {
                //will move it into the hud now
                view.BasePanel.IsInHud = true;

                view.originalScale = args.interactableObject.transform.localScale;
                //move into the hud
                args.interactableObject.transform.parent = viewParent.transform;
                //change color
                view.backgroundImage.color = hudColor;
                onViewPanelDocked.Invoke();
            }
        }

        public void OnViewWindowSelectionExit(SelectExitEventArgs args)
        {
            // //check if view is in list
            var view = _viewPanels.Where(x => x.interactable == args.interactableObject).FirstOrDefault();
            if (view == null) return;
            if (view.BasePanel.IsInHud)
            {
                //if so make sure to set the parent if also in hud (since the grab interactible reverts its parent once its let go)
                //(since it keeps track of the transform parent, which is fine but sometimes isnt) 
                view.interactable.transform.parent = viewParent.transform;
            }
            else
            {
                view.interactable.transform.parent = null;
            }
        }

        public void AdjustNewViewPanelPosition(BaseViewPair viewPair)
        {
            //set panel pos
            RaycastHit hit;
            if (Physics.Raycast(_mainCam.transform.position, _mainCam.transform.forward, out hit, viewPanelDistance))
            {
                viewPair.basePanel.transform.position = hit.point;
            }
            else
            {
                viewPair.basePanel.transform.position =
                    _mainCam.transform.position + _mainCam.transform.forward * viewPanelDistance;
            }
        }

        public void DeleteAllActiveViews()
        {
            var count = 0;
            switch (_viewMode)
            {
                case ViewMode.OCE:
                    count = oceViewModeHandler.DeleteAllActiveViews();
                    break;
                case ViewMode.Drone:
                    count = droneViewModeHandler.DeleteAllActiveViews();
                    break;
                case ViewMode.Hover:
                    count = hoverViewModeHandler.DeleteAllActiveViews();
                    break;
                default:
                    return;
            }

            if (count > 0)
                onAnyCamDestroyed.Invoke();
        }
    }
}
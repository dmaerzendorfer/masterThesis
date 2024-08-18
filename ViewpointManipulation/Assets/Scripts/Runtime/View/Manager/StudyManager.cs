using DG.Tweening;
using FastFileLog;
using Runtime.CameraControl;
using Runtime.View.Interactable;
using Runtime.View.ViewPair;
using TMPro;
using UnityEngine;

namespace Runtime.View.Manager
{
    public class StudyManager : MonoBehaviour
    {
        public TextMeshProUGUI poiCounterTmp;
        public TextMeshProUGUI instructionTmp;
        public GameObject separationWall;
        public GameObject diorama;
        public PointOfInterest[] pointOfInterests;
        public OceInteractable[] oceInteractables;
        public ControlManager controlManager;

        private StudyDataRecord _studyDataRecord;
        private bool _isInTaskOne = false;
        private bool _isStudyRunning = false;
        private ViewManager _viewManager;
        private float _timer = 0f;
        private float _droneRelativeTimer = 0f;
        private bool _droneRelativeActive = false;
        private float _userRelativeTimer = 0f;
        private bool _userRelativeActive = false;
        private int _foundPoisCount = 0;

        private void Start()
        {
            _viewManager = ViewManager.Instance;
        }


        private void FixedUpdate()
        {
            if (_isStudyRunning)
            {
                _timer += Time.deltaTime;

                if (_droneRelativeActive)
                {
                    _droneRelativeTimer += Time.deltaTime;
                }
                else if (_userRelativeActive)
                {
                    _userRelativeTimer += Time.deltaTime;
                }
            }

            if (_isInTaskOne)
            {
                poiCounterTmp.text = $"Found POIs: {_foundPoisCount}/{pointOfInterests.Length}";
                if (_foundPoisCount == pointOfInterests.Length)
                {
                    AllPoisFound();
                }
            }
        }

        private void OnDestroy()
        {
            DOTween.KillAll();
        }

        public void StartStudy()
        {
            _viewManager.DeleteAllActiveViews();

            _studyDataRecord = new StudyDataRecord();
            LogManager.Log(gameObject, "Start of Study - Task One: POIs");
            _isInTaskOne = true;
            _isStudyRunning = true;
            controlManager.canSwapMode = false;

            SetupTaskOneTracking();

            //show instructions
            instructionTmp.transform.parent.gameObject.SetActive(true);
            instructionTmp.text =
                $"Find the {pointOfInterests.Length} hidden POIs in the diorama opposite of the control table!";
            DOVirtual.DelayedCall(3, () => { instructionTmp.transform.parent.gameObject.SetActive(false); });
            poiCounterTmp.transform.parent.gameObject.SetActive(true);

            // enable diorama, disable wall
            diorama.SetActive(true);
            separationWall.SetActive(false);
        }

        public void AllPoisFound()
        {
            _isInTaskOne = false;
            _studyDataRecord.TaskOne.Duration = _timer;

            LogManager.Log(gameObject, _studyDataRecord.TaskOne.GetHeader());
            LogManager.Log(gameObject, _studyDataRecord.TaskOne.ToString());

            LogManager.Log(gameObject, "All POIs found, swapping to open exploration part");

            CleanupTaskOneTracking();

            controlManager.ToggleMode(true);

            SetupTaskTwoTracking();
        }

        public void EndStudy()
        {
            _isStudyRunning = false;
            controlManager.canSwapMode = true;

            //make sure to save duration if not done yet
            if (_isInTaskOne)
            {
                _studyDataRecord.TaskOne.Duration = _timer;
                _studyDataRecord.TaskOne.TimeInDroneRelativeMode = _droneRelativeTimer;
                _studyDataRecord.TaskOne.TimeInUserRelativeMode = _userRelativeTimer;

                LogManager.Log(gameObject, _studyDataRecord.TaskOne.GetHeader());
                LogManager.Log(gameObject, _studyDataRecord.TaskOne.ToString());

                CleanupTaskOneTracking();
            }
            else
            {
                _studyDataRecord.TaskTwo.Duration = _timer;
                _studyDataRecord.TaskTwo.TimeInDroneRelativeMode = _droneRelativeTimer;
                _studyDataRecord.TaskTwo.TimeInUserRelativeMode = _userRelativeTimer;

                LogManager.Log(gameObject, _studyDataRecord.TaskTwo.GetHeader());
                LogManager.Log(gameObject, _studyDataRecord.TaskTwo.ToString());

                CleanupTaskTwoTracking();
            }


            LogManager.Log(gameObject, (_isInTaskOne ? "End of Study whilst task one not complete!" : "End of Study"));
            //disable diorama, show wall again
            diorama.SetActive(false);
            separationWall.SetActive(true);
            _viewManager.DeleteAllActiveViews();
        }

        private void SetupTaskOneTracking()
        {
            _foundPoisCount = 0;
            _timer = 0f;
            _droneRelativeTimer = 0f;
            _userRelativeTimer = 0f;
            _droneRelativeActive = false;
            _userRelativeActive = false;

            poiCounterTmp.gameObject.SetActive(true);
            poiCounterTmp.text = $"Found POIs: {_foundPoisCount}/{pointOfInterests.Length}";

            //set oce mode in record
            _studyDataRecord.TaskOne.Mode = _viewManager.ViewMode;
            //hook up all the events
            _viewManager.onViewPanelDocked.AddListener(() => _studyDataRecord.TaskOne.DockCount++);
            _viewManager.onViewPanelUndocked.AddListener(() => _studyDataRecord.TaskOne.UnDockCount++);
            _viewManager.onAnyCamSpawned.AddListener(() => _studyDataRecord.TaskOne.SpawnedCamCount++);
            _viewManager.onAnyCamDestroyed.AddListener(() => _studyDataRecord.TaskOne.DeletedCamCount++);
            _viewManager.onDroneCamSpawned.AddListener(DroneSpawned);

            for (int i = 0; i < pointOfInterests.Length; i++)
            {
                var poi = pointOfInterests[i];
                //quite the ugly code duplication but i just want to have it work for now
                switch (i)
                {
                    case 0:
                        poi.OnIsNowInView.AddListener(() =>
                        {
                            if (_studyDataRecord.TaskOne.TimeForPoiOne == 0)
                            {
                                _studyDataRecord.TaskOne.TimeForPoiOne = _timer;
                            }

                            _foundPoisCount++;
                        });
                        poi.OnIsNoLongerInView.AddListener(() =>
                        {
                            _studyDataRecord.TaskOne.LostTrackOfPoiOne++;
                            _foundPoisCount--;
                        });
                        break;
                    case 1:
                        poi.OnIsNowInView.AddListener(() =>
                        {
                            if (_studyDataRecord.TaskOne.TimeForPoiTwo == 0)
                            {
                                _studyDataRecord.TaskOne.TimeForPoiTwo = _timer;
                            }

                            _foundPoisCount++;
                        });
                        poi.OnIsNoLongerInView.AddListener(() =>
                        {
                            _studyDataRecord.TaskOne.LostTrackOfPoiTwo++;
                            _foundPoisCount--;
                        });
                        break;
                    case 2:
                        poi.OnIsNowInView.AddListener(() =>
                        {
                            if (_studyDataRecord.TaskOne.TimeForPoiThree == 0)
                            {
                                _studyDataRecord.TaskOne.TimeForPoiThree = _timer;
                            }

                            _foundPoisCount++;
                        });
                        poi.OnIsNoLongerInView.AddListener(() =>
                        {
                            _studyDataRecord.TaskOne.TimeForPoiThree++;
                            _foundPoisCount--;
                        });
                        break;
                    case 3:
                        poi.OnIsNowInView.AddListener(() =>
                        {
                            if (_studyDataRecord.TaskOne.TimeForPoiFour == 0)
                            {
                                _studyDataRecord.TaskOne.TimeForPoiFour = _timer;
                            }

                            _foundPoisCount++;
                        });
                        poi.OnIsNoLongerInView.AddListener(() =>
                        {
                            _studyDataRecord.TaskOne.TimeForPoiFour++;
                            _foundPoisCount--;
                        });
                        break;
                    case 4:
                        poi.OnIsNowInView.AddListener(() =>
                        {
                            if (_studyDataRecord.TaskOne.TimeForPoiFive == 0)
                            {
                                _studyDataRecord.TaskOne.TimeForPoiFive = _timer;
                            }

                            _foundPoisCount++;
                        });
                        poi.OnIsNoLongerInView.AddListener(() =>
                        {
                            _studyDataRecord.TaskOne.TimeForPoiFive++;
                            _foundPoisCount--;
                        });
                        break;
                    case 5:
                        poi.OnIsNowInView.AddListener(() =>
                        {
                            if (_studyDataRecord.TaskOne.TimeForPoiSix == 0)
                            {
                                _studyDataRecord.TaskOne.TimeForPoiSix = _timer;
                            }

                            _foundPoisCount++;
                        });
                        poi.OnIsNoLongerInView.AddListener(() =>
                        {
                            _studyDataRecord.TaskOne.TimeForPoiSix++;
                            _foundPoisCount--;
                        });
                        break;
                    default:
                        break;
                }
            }

            foreach (var oceObj in oceInteractables)
            {
                oceObj.enabled = _studyDataRecord.TaskOne.Mode == ViewMode.OCE;
            }
        }

        private void CleanupTaskOneTracking()
        {
            poiCounterTmp.transform.parent.gameObject.SetActive(false);

            //unhook events
            _viewManager.onViewPanelDocked.RemoveAllListeners();
            _viewManager.onViewPanelUndocked.RemoveAllListeners();
            _viewManager.onAnyCamSpawned.RemoveAllListeners();
            _viewManager.onAnyCamDestroyed.RemoveAllListeners();
            _viewManager.onDroneCamSpawned.RemoveAllListeners();

            foreach (var poi in pointOfInterests)
            {
                poi.OnIsNowInView.RemoveAllListeners();
                poi.OnIsNoLongerInView.RemoveAllListeners();
            }

            //since we remove all listeners we have to hook the control table up again
            controlManager.HookUpCamCount();
        }

        private void SetupTaskTwoTracking()
        {
            instructionTmp.transform.parent.gameObject.SetActive(true);
            instructionTmp.text =
                $"Now explore the diorama with the other control method as long as you want. \n In order to exit press the 'Exit' button on the control table.";
            DOVirtual.DelayedCall(10, () => { instructionTmp.transform.parent.gameObject.SetActive(false); });

            _timer = 0f;
            _droneRelativeTimer = 0f;
            _userRelativeTimer = 0f;
            _droneRelativeActive = false;
            _userRelativeActive = false;

            //set control mode in record
            _studyDataRecord.TaskTwo.Mode = _viewManager.ViewMode;
            //hook up all the events
            _viewManager.onViewPanelDocked.AddListener(() => _studyDataRecord.TaskTwo.DockCount++);
            _viewManager.onViewPanelUndocked.AddListener(() => _studyDataRecord.TaskTwo.UnDockCount++);
            _viewManager.onAnyCamSpawned.AddListener(() => _studyDataRecord.TaskTwo.SpawnedCamCount++);
            _viewManager.onAnyCamDestroyed.AddListener(() => _studyDataRecord.TaskTwo.DeletedCamCount++);

            _viewManager.onDroneCamSpawned.AddListener(DroneSpawned);

            foreach (var oceObj in oceInteractables)
            {
                oceObj.enabled = _studyDataRecord.TaskTwo.Mode == ViewMode.OCE;
            }
        }

        private void CleanupTaskTwoTracking()
        {
            //unhook up all the events
            _viewManager.onViewPanelDocked.RemoveAllListeners();
            _viewManager.onViewPanelUndocked.RemoveAllListeners();
            _viewManager.onAnyCamSpawned.RemoveAllListeners();
            _viewManager.onAnyCamDestroyed.RemoveAllListeners();
            _viewManager.onDroneCamSpawned.RemoveAllListeners();

            //since we remove all listeners we have to hook the control table up again
            controlManager.HookUpCamCount();
        }

        private void DroneSpawned(DroneViewPair droneViewPair)
        {
            droneViewPair.droneCamController.onSelected.AddListener(DroneSelected);
            droneViewPair.droneCamController.onUnselected.AddListener(DroneUnselected);
            droneViewPair.droneCamController.onMovementModeChange.AddListener(DroneModeChanged);
        }

        private void DroneSelected(DroneCamController droneCamController)
        {
            if (droneCamController.movementMode == DroneMovementMode.DroneRelative)
            {
                _droneRelativeActive = true;
                _userRelativeActive = false;
            }
            else
            {
                _droneRelativeActive = false;
                _userRelativeActive = true;
            }
        }

        private void DroneUnselected(DroneCamController droneCamController)
        {
            _droneRelativeActive = false;
            _userRelativeActive = false;
        }

        private void DroneModeChanged(DroneCamController droneCamController)
        {
            if (droneCamController.IsSelected)
            {
                if (droneCamController.movementMode == DroneMovementMode.DroneRelative)
                {
                    _droneRelativeActive = true;
                    _userRelativeActive = false;
                }
                else
                {
                    _droneRelativeActive = false;
                    _userRelativeActive = true;
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FastFileLog;
using Runtime.CameraControl;
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

        public PointOfInterest[] pois;
        public int howManyPois = 3;
        public ControlManager controlManager;

        public float instructionDisplayTime = 4.5f;

        public AudioSource successSound;

        private StudyDataRecord _studyDataRecord;
        private int _currentTask = -1;
        private bool _isStudyRunning = false;
        private ViewManager _viewManager;
        private float _timer = 0f;
        private float _droneRelativeTimer = 0f;
        private bool _droneRelativeActive = false;
        private float _userRelativeTimer = 0f;
        private bool _userRelativeActive = false;

        private int _foundPoisCount = 0;

        private List<PointOfInterest> _currentPointOfInterests;
        private List<PointOfInterest> _previousPointOfInterests = new List<PointOfInterest>();

        private void Start()
        {
            _viewManager = ViewManager.Instance;

            _studyDataRecord = new StudyDataRecord();
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

            if (_currentTask != -1 && _currentPointOfInterests != null)
            {
                poiCounterTmp.text = $"Found POIs: {_foundPoisCount}/{_currentPointOfInterests.Count}";
                if (_foundPoisCount == _currentPointOfInterests.Count)
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

            LogManager.Log(gameObject, "Start of Study");
            //increase currenTask, if its at -1 we start with 0, after the first task(0) users need to press start again for the second one (1)
            _currentTask++;
            _isStudyRunning = true;
            controlManager.canSwapMode = false;

            DecideOnPoiSet();

            SetupTaskTracking();

            //show instructions
            instructionTmp.transform.parent.gameObject.SetActive(true);
            instructionTmp.text =
                $"Find the {_currentPointOfInterests.Count} hidden POIs in the diorama opposite of the control table!";
            DOVirtual.DelayedCall(instructionDisplayTime,
                () => { instructionTmp.transform.parent.gameObject.SetActive(false); });
            poiCounterTmp.transform.parent.gameObject.SetActive(true);

            // enable diorama, disable wall
            diorama.SetActive(true);
            separationWall.SetActive(false);
        }

        public void AllPoisFound()
        {
            //save current task
            _studyDataRecord.Tasks[_currentTask].Duration = _timer;
            _studyDataRecord.Tasks[_currentTask].TimeInDroneRelativeMode = _droneRelativeTimer;
            _studyDataRecord.Tasks[_currentTask].TimeInUserRelativeMode = _userRelativeTimer;

            LogManager.Log(gameObject, _studyDataRecord.Tasks[_currentTask].GetHeader());
            LogManager.Log(gameObject, _studyDataRecord.Tasks[_currentTask].ToString());

            CleanupTaskTracking();
            //either stop test or start second one

            diorama.SetActive(false);
            separationWall.SetActive(true);
            _viewManager.DeleteAllActiveViews();

            if (_currentTask == 1)
            {
                //we are done
                _isStudyRunning = false;
                controlManager.canSwapMode = true;

                _currentPointOfInterests = null;
                _currentTask = -1;

                LogManager.Log(gameObject, "End of Study");


                instructionTmp.transform.parent.gameObject.SetActive(true);
                instructionTmp.text =
                    $"Study over, well done :)";
                DOVirtual.DelayedCall(instructionDisplayTime,
                    () => { instructionTmp.transform.parent.gameObject.SetActive(false); });


                return;
            }
            //there is a second part for the study

            //show info and let user know to press start again
            controlManager.ToggleMode(true);
            //set currentPointOfInterests to null so we dont enter here again...
            _currentPointOfInterests = null;

            instructionTmp.transform.parent.gameObject.SetActive(true);
            instructionTmp.text =
                $"Press start study again for part 2 :)";
            DOVirtual.DelayedCall(instructionDisplayTime,
                () => { instructionTmp.transform.parent.gameObject.SetActive(false); });
        }

        public void EndStudy()
        {
            if (!_isStudyRunning) return;

            //save task if one is active
            _studyDataRecord.Tasks[_currentTask].Duration = _timer;
            _studyDataRecord.Tasks[_currentTask].TimeInDroneRelativeMode = _droneRelativeTimer;
            _studyDataRecord.Tasks[_currentTask].TimeInUserRelativeMode = _userRelativeTimer;

            LogManager.Log(gameObject, "saving task in case its not saved yet");
            LogManager.Log(gameObject, _studyDataRecord.Tasks[_currentTask].GetHeader());
            LogManager.Log(gameObject, _studyDataRecord.Tasks[_currentTask].ToString());

            _isStudyRunning = false;
            controlManager.canSwapMode = true;

            _currentTask = -1;

            CleanupTaskTracking();
            _currentPointOfInterests = null;

            LogManager.Log(gameObject, "Early end of Study via button press");
            diorama.SetActive(false);
            separationWall.SetActive(true);
            _viewManager.DeleteAllActiveViews();
        }

        private void SetupTaskTracking()
        {
            _foundPoisCount = 0;
            _timer = 0f;
            _droneRelativeTimer = 0f;
            _userRelativeTimer = 0f;
            _droneRelativeActive = false;
            _userRelativeActive = false;

            poiCounterTmp.gameObject.SetActive(true);
            poiCounterTmp.text = $"Found POIs: {_foundPoisCount}/{_currentPointOfInterests.Count}";

            var task = _studyDataRecord.Tasks[_currentTask];

            //set oce mode in record
            task.Mode = _viewManager.ViewMode;


            //enable the correct pois
            foreach (var poi in _currentPointOfInterests)
            {
                poi.gameObject.SetActive(true);
            }

            //hook up all the events
            _viewManager.onViewPanelDocked.AddListener(() => task.DockCount++);
            _viewManager.onViewPanelUndocked.AddListener(() => task.UnDockCount++);
            _viewManager.onAnyCamSpawned.AddListener(() => task.SpawnedCamCount++);
            _viewManager.onAnyCamDestroyed.AddListener(() => task.DeletedCamCount++);
            _viewManager.onDroneCamSpawned.AddListener(DroneSpawned);

            for (int i = 0; i < _currentPointOfInterests.Count; i++)
            {
                var poi = _currentPointOfInterests[i];
                //quite the ugly code duplication but i just want to have it work for now
                switch (i)
                {
                    case 0:
                        poi.OnIsNowInView.AddListener(() =>
                        {
                            if (task.TimeForPoiOne == 0)
                            {
                                task.TimeForPoiOne = _timer;
                                task.PoiNameOne = poi.poiName;
                                successSound.Play();
                            }

                            _foundPoisCount++;
                        });
                        poi.OnIsNoLongerInView.AddListener(() =>
                        {
                            task.LostTrackOfPoiOne++;
                            _foundPoisCount--;
                        });
                        break;
                    case 1:
                        poi.OnIsNowInView.AddListener(() =>
                        {
                            if (task.TimeForPoiTwo == 0)
                            {
                                task.TimeForPoiTwo = _timer;
                                task.PoiNameTwo = poi.poiName;
                                successSound.Play();
                            }

                            _foundPoisCount++;
                        });
                        poi.OnIsNoLongerInView.AddListener(() =>
                        {
                            task.LostTrackOfPoiTwo++;
                            _foundPoisCount--;
                        });
                        break;
                    case 2:
                        poi.OnIsNowInView.AddListener(() =>
                        {
                            if (task.TimeForPoiThree == 0)
                            {
                                task.TimeForPoiThree = _timer;
                                task.PoiNameThree = poi.poiName;
                                successSound.Play();
                            }

                            _foundPoisCount++;
                        });
                        poi.OnIsNoLongerInView.AddListener(() =>
                        {
                            task.LostTrackOfPoiThree++;
                            _foundPoisCount--;
                        });
                        break;
                    case 3:
                        poi.OnIsNowInView.AddListener(() =>
                        {
                            if (task.TimeForPoiFour == 0)
                            {
                                task.TimeForPoiFour = _timer;
                                task.PoiNameFour = poi.poiName;
                                successSound.Play();
                            }

                            _foundPoisCount++;
                        });
                        poi.OnIsNoLongerInView.AddListener(() =>
                        {
                            task.LostTrackOfPoiFour++;
                            _foundPoisCount--;
                        });
                        break;
                    case 4:
                        poi.OnIsNowInView.AddListener(() =>
                        {
                            if (task.TimeForPoiFive == 0)
                            {
                                task.TimeForPoiFive = _timer;
                                task.PoiNameFive = poi.poiName;
                                successSound.Play();
                            }

                            _foundPoisCount++;
                        });
                        poi.OnIsNoLongerInView.AddListener(() =>
                        {
                            task.LostTrackOfPoiFive++;
                            _foundPoisCount--;
                        });
                        break;
                    case 5:
                        poi.OnIsNowInView.AddListener(() =>
                        {
                            if (task.TimeForPoiSix == 0)
                            {
                                task.TimeForPoiSix = _timer;
                                task.PoiNameSix = poi.poiName;
                                successSound.Play();
                            }

                            _foundPoisCount++;
                        });
                        poi.OnIsNoLongerInView.AddListener(() =>
                        {
                            task.LostTrackOfPoiSix++;
                            _foundPoisCount--;
                        });
                        break;
                    default:
                        break;
                }
            }
        }

        private void DecideOnPoiSet()
        {
            //select random pois from possible ones
            var possiblePois = pois.Except(_previousPointOfInterests).ToList();
            _currentPointOfInterests = possiblePois.OrderBy(x => Guid.NewGuid()).Take(howManyPois).ToList();
            _previousPointOfInterests = _currentPointOfInterests;
        }

        private void CleanupTaskTracking()
        {
            poiCounterTmp.transform.parent.gameObject.SetActive(false);

            _timer = 0f;
            _droneRelativeTimer = 0f;
            _userRelativeTimer = 0f;
            _droneRelativeActive = false;
            _userRelativeActive = false;

            //unhook events
            _viewManager.onViewPanelDocked.RemoveAllListeners();
            _viewManager.onViewPanelUndocked.RemoveAllListeners();
            _viewManager.onAnyCamSpawned.RemoveAllListeners();
            _viewManager.onAnyCamDestroyed.RemoveAllListeners();
            _viewManager.onDroneCamSpawned.RemoveAllListeners();

            if (_currentPointOfInterests != null)
            {
                foreach (var poi in _currentPointOfInterests)
                {
                    poi.OnIsNowInView.RemoveAllListeners();
                    poi.OnIsNoLongerInView.RemoveAllListeners();
                    poi.gameObject.SetActive(false);
                }
            }

            //since we remove all listeners we have to hook the control table up again
            controlManager.HookUpCamCount();
        }

        private void DroneSpawned(DroneViewPair droneViewPair)
        {
            // DroneSelected(droneViewPair.droneCamController);

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
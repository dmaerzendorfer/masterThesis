using System;
using DG.Tweening;
using FastFileLog;
using Runtime.CameraControl;
using Runtime.View.Interactable;
using Runtime.View.ViewPair;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.View.Manager
{
    [Serializable]
    public struct PoiSet
    {
        public PointOfInterest[] pois;
    }

    public class StudyManager : MonoBehaviour
    {
        public TextMeshProUGUI poiCounterTmp;
        public TextMeshProUGUI instructionTmp;
        public GameObject separationWall;
        public GameObject diorama;
        public PoiSet[] poiSets;
        public ControlManager controlManager;

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
        private int _mostRecentlyUsedSet = -1;
        private int _currentSet = -1;
        private PointOfInterest[] _currentPointOfInterests;

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

            if (_currentTask != -1)
            {
                poiCounterTmp.text = $"Found POIs: {_foundPoisCount}/{_currentPointOfInterests.Length}";
                if (_foundPoisCount == _currentPointOfInterests.Length)
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
            LogManager.Log(gameObject, "Start of Study");
            _currentTask = 0;
            _isStudyRunning = true;
            controlManager.canSwapMode = false;

            if (_mostRecentlyUsedSet == -1)
            {
                _currentSet = Random.Range(0, poiSets.Length);
            }
            else
            {
                do
                {
                    _currentSet = Random.Range(0, poiSets.Length);
                } while (_currentSet == _mostRecentlyUsedSet);
            }

            _currentPointOfInterests = poiSets[_currentSet].pois;

            SetupTaskTracking();

            //show instructions
            instructionTmp.transform.parent.gameObject.SetActive(true);
            instructionTmp.text =
                $"Find the {_currentPointOfInterests.Length} hidden POIs in the diorama opposite of the control table!";
            DOVirtual.DelayedCall(3, () => { instructionTmp.transform.parent.gameObject.SetActive(false); });
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

            if (_currentTask == 1)
            {
                //we are done
                LogManager.Log(gameObject, "End of Study");
                diorama.SetActive(false);
                separationWall.SetActive(true);
                _viewManager.DeleteAllActiveViews();

                instructionTmp.transform.parent.gameObject.SetActive(true);
                instructionTmp.text =
                    $"Study over, well done :)";
                DOVirtual.DelayedCall(3, () => { instructionTmp.transform.parent.gameObject.SetActive(false); });


                return;
            }

            //set the correct pois
            if (_mostRecentlyUsedSet == -1)
            {
                _currentSet = Random.Range(0, poiSets.Length);
            }
            else
            {
                do
                {
                    _currentSet = Random.Range(0, poiSets.Length);
                } while (_currentSet == _mostRecentlyUsedSet);
            }

            //setup tracking again
            _mostRecentlyUsedSet = _currentSet;
            _currentPointOfInterests = null;
            _currentTask++;

            controlManager.ToggleMode(true);
            SetupTaskTracking();
        }

        public void EndStudy()
        {
            _isStudyRunning = false;
            controlManager.canSwapMode = true;

            _currentPointOfInterests = null;
            _mostRecentlyUsedSet = _currentSet;
            _currentSet = -1;

            CleanupTaskTracking();

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
            poiCounterTmp.text = $"Found POIs: {_foundPoisCount}/{_currentPointOfInterests.Length}";

            var task = _studyDataRecord.Tasks[_currentTask];

            //set oce mode in record
            task.Mode = _viewManager.ViewMode;
            task.SetIndex = _currentSet;

            //enable the correct pois
            for (int i = 0; i < poiSets.Length; i++)
            {
                bool shouldBeActive = i == _currentSet;

                foreach (var pointOfInterest in poiSets[i].pois)
                {
                    pointOfInterest.gameObject.SetActive(shouldBeActive);
                }
            }


            //hook up all the events
            _viewManager.onViewPanelDocked.AddListener(() => task.DockCount++);
            _viewManager.onViewPanelUndocked.AddListener(() => task.UnDockCount++);
            _viewManager.onAnyCamSpawned.AddListener(() => task.SpawnedCamCount++);
            _viewManager.onAnyCamDestroyed.AddListener(() => task.DeletedCamCount++);
            _viewManager.onDroneCamSpawned.AddListener(DroneSpawned);

            for (int i = 0; i < _currentPointOfInterests.Length; i++)
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
                            }

                            _foundPoisCount++;
                        });
                        poi.OnIsNoLongerInView.AddListener(() =>
                        {
                            task.TimeForPoiThree++;
                            _foundPoisCount--;
                        });
                        break;
                    case 3:
                        poi.OnIsNowInView.AddListener(() =>
                        {
                            if (task.TimeForPoiFour == 0)
                            {
                                task.TimeForPoiFour = _timer;
                            }

                            _foundPoisCount++;
                        });
                        poi.OnIsNoLongerInView.AddListener(() =>
                        {
                            task.TimeForPoiFour++;
                            _foundPoisCount--;
                        });
                        break;
                    case 4:
                        poi.OnIsNowInView.AddListener(() =>
                        {
                            if (task.TimeForPoiFive == 0)
                            {
                                task.TimeForPoiFive = _timer;
                            }

                            _foundPoisCount++;
                        });
                        poi.OnIsNoLongerInView.AddListener(() =>
                        {
                            task.TimeForPoiFive++;
                            _foundPoisCount--;
                        });
                        break;
                    case 5:
                        poi.OnIsNowInView.AddListener(() =>
                        {
                            if (task.TimeForPoiSix == 0)
                            {
                                task.TimeForPoiSix = _timer;
                            }

                            _foundPoisCount++;
                        });
                        poi.OnIsNoLongerInView.AddListener(() =>
                        {
                            task.TimeForPoiSix++;
                            _foundPoisCount--;
                        });
                        break;
                    default:
                        break;
                }
            }
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

            foreach (var poi in _currentPointOfInterests)
            {
                poi.OnIsNowInView.RemoveAllListeners();
                poi.OnIsNoLongerInView.RemoveAllListeners();
            }

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
using System;
using DG.Tweening;
using FastFileLog;
using Runtime.View.Interactable;
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
        private float _userRelativeTimer = 0f;
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
            }

            if (_isInTaskOne)
            {
                poiCounterTmp.text = $"Found POIs: {_foundPoisCount}/{pointOfInterests.Length}";
                if (_foundPoisCount == pointOfInterests.Length)
                {
                    AllPoisFound();
                }
            }
            //todo: setup the drone mode timer thingy
        }

        private void OnDestroy()
        {
            DOTween.KillAll();
        }

        public void StartStudy()
        {
            _studyDataRecord = new StudyDataRecord();
            LogManager.Log(gameObject, "Start of Study - Task One: POIs");
            _isInTaskOne = true;
            _isStudyRunning = true;
            controlManager.canSwapMode = false;

            SetupTaskOneTracking();

            //show instructions
            instructionTmp.gameObject.SetActive(true);
            instructionTmp.text =
                $"Find the {pointOfInterests.Length} hidden POIs in the diorama opposite of the control table!";
            DOVirtual.DelayedCall(3, () => { instructionTmp.gameObject.SetActive(false); });

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
            
            instructionTmp.gameObject.SetActive(true);
            instructionTmp.text =
                $"Now explore the diorama with the other control method as long as you want. \n In order to exit press the 'Exit' button on the control table.";
            DOVirtual.DelayedCall(3, () => { instructionTmp.gameObject.SetActive(false); });
        }

        public void EndStudy()
        {
            _isStudyRunning = false;
            controlManager.canSwapMode = true;

            //make sure to save duration if not done yet
            if (_isInTaskOne)
            {
                if (_studyDataRecord.TaskOne.Duration == 0)
                {
                    _studyDataRecord.TaskOne.Duration = _timer;
                }

                CleanupTaskOneTracking();
            }
            else
            {
                if (_studyDataRecord.TaskTwo.Duration == 0)
                {
                    _studyDataRecord.TaskTwo.Duration = _timer;
                }

                CleanupTaskTwoTracking();
            }


            LogManager.Log(gameObject, _studyDataRecord.TaskTwo.GetHeader());
            LogManager.Log(gameObject, _studyDataRecord.TaskTwo.ToString());
            LogManager.Log(gameObject, (_isInTaskOne ? "End of Study whilst task one not complete!" : "End of Study"));
            //disable diorama, show wall again
            diorama.SetActive(false);
            separationWall.SetActive(true);
        }

        private void SetupTaskOneTracking()
        {
            _foundPoisCount = 0;
            _timer = 0f;

            poiCounterTmp.gameObject.SetActive(true);
            poiCounterTmp.text = $"Found POIs: {_foundPoisCount}/{pointOfInterests.Length}";

            //set oce mode in record
            _studyDataRecord.TaskOne.Mode = _viewManager.ViewMode;
            //hook up all the events
            _viewManager.onViewPanelDocked.AddListener(() => _studyDataRecord.TaskOne.DockCount++);
            _viewManager.onViewPanelUndocked.AddListener(() => _studyDataRecord.TaskOne.UnDockCount++);
            _viewManager.onAnyCamSpawned.AddListener(() => _studyDataRecord.TaskOne.SpawnedCamCount++);
            _viewManager.onAnyCamDestroyed.AddListener(() => _studyDataRecord.TaskOne.DeletedCamCount++);

            pointOfInterests[0].OnIsNowInView.AddListener(() =>
            {
                if (_studyDataRecord.TaskOne.TimeForPoiOne == 0)
                {
                    _studyDataRecord.TaskOne.TimeForPoiOne = _timer;
                }

                _foundPoisCount++;
            });
            pointOfInterests[0].OnIsNoLongerInView.AddListener(() =>
            {
                _studyDataRecord.TaskOne.LostTrackOfPoiOne++;
                _foundPoisCount--;
            });

            pointOfInterests[1].OnIsNowInView.AddListener(() =>
            {
                if (_studyDataRecord.TaskOne.TimeForPoiTwo == 0)
                {
                    _studyDataRecord.TaskOne.TimeForPoiTwo = _timer;
                }

                _foundPoisCount++;
            });
            pointOfInterests[1].OnIsNoLongerInView.AddListener(() =>
                {
                    _studyDataRecord.TaskOne.LostTrackOfPoiTwo++;
                    _foundPoisCount--;
                }
            );

            pointOfInterests[2].OnIsNowInView.AddListener(() =>
            {
                if (_studyDataRecord.TaskOne.TimeForPoiThree == 0)
                {
                    _studyDataRecord.TaskOne.TimeForPoiThree = _timer;
                }

                _foundPoisCount++;
            });
            pointOfInterests[2].OnIsNoLongerInView
                .AddListener(() =>
                {
                    _studyDataRecord.TaskOne.LostTrackOfPoiThree++;
                    _foundPoisCount--;
                });

            pointOfInterests[3].OnIsNowInView.AddListener(() =>
            {
                if (_studyDataRecord.TaskOne.TimeForPoiFour == 0)
                {
                    _studyDataRecord.TaskOne.TimeForPoiFour = _timer;
                }

                _foundPoisCount++;
            });
            pointOfInterests[3].OnIsNoLongerInView.AddListener(() =>
            {
                _studyDataRecord.TaskOne.LostTrackOfPoiFour++;
                _foundPoisCount--;
            });

            pointOfInterests[4].OnIsNowInView.AddListener(() =>
            {
                if (_studyDataRecord.TaskOne.TimeForPoiFive == 0)
                {
                    _studyDataRecord.TaskOne.TimeForPoiFive = _timer;
                }

                _foundPoisCount++;
            });
            pointOfInterests[4].OnIsNoLongerInView.AddListener(() =>
            {
                _studyDataRecord.TaskOne.LostTrackOfPoiFive++;
                _foundPoisCount--;
            });

            pointOfInterests[5].OnIsNowInView.AddListener(() =>
            {
                if (_studyDataRecord.TaskOne.TimeForPoiSix == 0)
                {
                    _studyDataRecord.TaskOne.TimeForPoiSix = _timer;
                }

                _foundPoisCount++;
            });
            pointOfInterests[5].OnIsNoLongerInView.AddListener(() =>
            {
                _studyDataRecord.TaskOne.LostTrackOfPoiSix++;
                _foundPoisCount--;
            });

            foreach (var oceObj in oceInteractables)
            {
                oceObj.enabled = _studyDataRecord.TaskOne.Mode == ViewMode.OCE;
            }
        }

        private void CleanupTaskOneTracking()
        {
            poiCounterTmp.gameObject.SetActive(false);

            //unhook events
            _viewManager.onViewPanelDocked.RemoveAllListeners();
            _viewManager.onViewPanelUndocked.RemoveAllListeners();
            _viewManager.onAnyCamSpawned.RemoveAllListeners();
            _viewManager.onAnyCamDestroyed.RemoveAllListeners();

            foreach (var poi in pointOfInterests)
            {
                poi.OnIsNowInView.RemoveAllListeners();
                poi.OnIsNoLongerInView.RemoveAllListeners();
            }
        }

        private void SetupTaskTwoTracking()
        {
            _timer = 0f;

            //set control mode in record
            _studyDataRecord.TaskOne.Mode = _viewManager.ViewMode;
            //hook up all the events
            _viewManager.onViewPanelDocked.AddListener(() => _studyDataRecord.TaskTwo.DockCount++);
            _viewManager.onViewPanelUndocked.AddListener(() => _studyDataRecord.TaskTwo.UnDockCount++);
            _viewManager.onAnyCamSpawned.AddListener(() => _studyDataRecord.TaskTwo.SpawnedCamCount++);
            _viewManager.onAnyCamDestroyed.AddListener(() => _studyDataRecord.TaskTwo.DeletedCamCount++);

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
        }
    }
}
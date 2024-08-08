using System;
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


        private StudyDataRecord _studyDataRecord;
        private bool _isInTaskOne = false;
        private ViewManager _viewManager;

        private void Start()
        {
            _viewManager = ViewManager.Instance;
        }

        public void StartStudy()
        {
            _studyDataRecord = new StudyDataRecord();
            LogManager.Log(gameObject, "Start of Study");
            LogManager.Log(gameObject, _studyDataRecord.GetHeader());
            _isInTaskOne = true;
            //todo: setup task -> show instruction and poi counter
            // enable diorama, disable wall, enable oce-objects(depending on if in drone or oce)
        }

        public void AllPoisFound()
        {
            _isInTaskOne = false;
            LogManager.Log(gameObject, "All POIs found, swapping to open exploration part");
            //todo: swap to other method -> disable/enable oce obj if need be
        }

        public void EndStudy()
        {
            LogManager.Log(gameObject, _studyDataRecord.ToString());
            LogManager.Log(gameObject, (_isInTaskOne ? "End of Study whilst task one not complete!" : "End of Study"));
            //disable diorama, show wall again
        }
    }
}
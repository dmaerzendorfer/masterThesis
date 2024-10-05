using Runtime.CameraControl;
using UnityEngine;

namespace Runtime.View.ViewPair
{
    public class OrbitViewPair : BaseViewPair
    {
        public OrbitCamController orbitCamController;

        public override void Awake()
        {
            base.Awake();
            orbitCamController.viewPair = this;
        }

        public override void ReceiveSelect()
        {
            Debug.Log("Oce view panel does not support selection of cam via panel!");
        }
    }
}
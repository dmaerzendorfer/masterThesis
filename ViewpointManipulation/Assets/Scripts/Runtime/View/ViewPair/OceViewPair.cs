using Runtime.CameraControl;
using UnityEngine;

namespace Runtime.View.ViewPair
{
    public class OceViewPair : BaseViewPair
    {
        public OrbitCamController orbitCamController;

        public override void ReceiveSelect()
        {
            Debug.Log("Oce view panel does not support selection of cam via panel!");
        }
    }
}
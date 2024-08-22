using System.Linq;
using Runtime.View.ViewPair;
using UnityEngine;

namespace Runtime.View.Manager.ViewModeHandler
{
    public class OceViewModeHandler : BaseViewModeHandler<OceViewPair>
    {
        private Camera _mainCam;
        private ViewManager _viewManager;
        private bool _isActivated = false;

        private void Start()
        {
            _mainCam = Camera.main;
            _viewManager = ViewManager.Instance;
        }

        public override OceViewPair SpawnViewPair()
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
        }

        public override void Deactivate()
        {
            base.Deactivate();
            _isActivated = false;
        }
    }
}
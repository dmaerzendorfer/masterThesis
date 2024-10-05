using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.View.ViewPair;
using UnityEngine;

namespace Runtime.View.Manager.ViewModeHandler
{
    [Serializable]
    public class ViewConfig<T> where T : BaseViewPair
    {
        public String panelTitle;

        public T prefab;

        [HideInInspector]
        public T instance = null;
    }

    public abstract class BaseViewModeHandler<T> : MonoBehaviour where T : BaseViewPair
    {
        public List<ViewConfig<T>> viewConfigs;

        public int CurrentActiveViewCount
        {
            get
            {
                return viewConfigs.Count(x => x.instance != null);
            }
        }
        
        /// <summary>
        ///  Deletes all active views pairs.
        /// </summary>
        /// <returns>how many view pairs where deleted</returns>
        public int DeleteAllActiveViews()
        {
            var count = 0;
            viewConfigs.ForEach(x =>
            {
                if (x.instance != null)
                {
                    x.instance.DeleteViewPair();
                    x.instance = null;
                    count++;
                }
            });
            return count;
        }

        public abstract T SpawnViewPair();
        public abstract void Activate();

        public virtual void Deactivate()
        {
            DeleteAllActiveViews();
        }
    }
}
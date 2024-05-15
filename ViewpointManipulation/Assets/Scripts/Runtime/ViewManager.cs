using System.Collections.Generic;
using System.Linq;
using _Generics.Scripts.Runtime;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class ViewData
{
    public XRGrabInteractable interactable;
    public bool inHud = false;
    public GameObject linkedCamera;
    public Vector3 originalScale; //so we can scale it back to its original once it leaves the hud
    public Image backgroundImage;

    public Sequence popSequence = DOTween.Sequence();
}

public class ViewManager : SingletonMonoBehaviour<ViewManager>
{
    public GameObject viewParent;

    [BoxGroup("ColorSettings")]
    public Color worldColor = Color.black;

    [BoxGroup("ColorSettings")]
    public Color hudColor = Color.gray;
    
    private List<ViewData> _views = new List<ViewData>();

    private void OnDestroy()
    {
        _views.ForEach(x => x.popSequence.Kill());
    }

    public void OnViewWindowActivate(ActivateEventArgs args)
    {
        var view = _views.Where(x => x.interactable == args.interactableObject).FirstOrDefault();
        if (view == null)
        {
            //its a new view -> add it to the list
            view = new ViewData();
            _views.Add(view);

            view.interactable = (XRGrabInteractable)args.interactableObject;
            view.inHud = true; //will move it into the hud now
            view.originalScale = args.interactableObject.transform.localScale;
            //move into the hud
            args.interactableObject.transform.parent = viewParent.transform;

            //change color
            view.backgroundImage = view.interactable.GetComponentInChildren<Image>();
            view.backgroundImage.color = hudColor;

            //todo: figure out how to make sure its somewhat close to the camera (and keeps its scale)
        }
        else
        {
            //view already in list

            //move out of hud
            if (view.inHud)
            {
                //move it out of hud
                view.interactable.transform.parent = null;
                view.inHud = false;
                //set to original scale again
                view.interactable.transform.localScale = view.originalScale;

                //change color
                view.backgroundImage.color = worldColor;
            }
            //move into the hud
            else
            {
                view.inHud = true; //will move it into the hud now
                view.originalScale = args.interactableObject.transform.localScale;
                //move into the hud
                args.interactableObject.transform.parent = viewParent.transform;
                //change color
                view.backgroundImage.color = hudColor;
            }
        }

    }

    public void OnViewWindowSelectionExit(SelectExitEventArgs args)
    {
        //check if view is in list
        var view = _views.Where(x => x.interactable == args.interactableObject).FirstOrDefault();
        if (view == null) return;
        if (view.inHud)
        {
            //if so make sure to set the parent if also in hud (since the grab interactible reverts its parent once its let go)
            view.interactable.transform.parent = viewParent.transform;
        }
        else
        {
            view.interactable.transform.parent = null;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using Core.InputSystem;
using Core.Shared.Utilities;

namespace UI
{
    public class MenuEventSystemHandler : MonoBehaviour
    {
        [Header("Selectable UI Elements")]
        [Tooltip("List of all the selectable UI elements in this menu")]
        [SerializeField] protected List<Selectable> selectables = new();
        [Tooltip("The first selected UI element when the menu is opened")]
        [SerializeField] protected Selectable firstSelected;

        [Header("Animations")]
        [Tooltip("Scale multiplier for the selected UI element")]
        [SerializeField] protected float selectedAnimationScale = 1.2f;
        [Tooltip("Duration of the scale animation")]
        [SerializeField] protected float scaleDuration = 0.25f;
        [Tooltip("List of UI elements to exclude from the selection animation")]
        [SerializeField] protected List<GameObject> excludeFromAnimation = new();

        [Header("Game Inputs")]
        [SerializeField][Required] protected InputHandler inputHandler;

        protected Dictionary<Selectable, Vector3> scales = new();
        protected Selectable lastSelected;
        protected Tween scaleUpTween;
        protected Tween scaleDownTween;

        public virtual void Awake()
        {
            foreach (var _selectable in selectables)
            {
                AddSelectionListner(_selectable);
                scales.Add(_selectable, _selectable.transform.localScale);
            }
        }

        public virtual void OnEnable()
        {
            inputHandler.OnNavigateEvent += OnNavigate;
            for (int i = 0; i < selectables.Count; i++)
            {
                selectables[i].transform.localScale = scales[selectables[i]];
            }

            StartCoroutine(SelectAfterDelay());
        }

        public virtual void OnDisable()
        {
            inputHandler.OnNavigateEvent -= OnNavigate;
            scaleUpTween.Kill(true);
            scaleDownTween.Kill(true);
        }

        protected virtual IEnumerator SelectAfterDelay()
        {
            yield return null;
            EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
        }

        protected virtual void AddSelectionListner(Selectable selectable)
        {
            // Add listener
            EventTrigger trigger = selectable.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = selectable.gameObject.AddComponent<EventTrigger>();

            // Add SELECT event
            EventTrigger.Entry selectEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Select
            };
            selectEntry.callback.AddListener(OnSelect);
            trigger.triggers.Add(selectEntry);

            // Add DESELECT event
            EventTrigger.Entry deselectEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Deselect
            };
            deselectEntry.callback.AddListener(OnDeselect);
            trigger.triggers.Add(deselectEntry);

            // Add ONPOINTERENTER event
            EventTrigger.Entry pointerEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            pointerEntry.callback.AddListener(OnPointerEnter);
            trigger.triggers.Add(pointerEntry);

            // Add ONPOINTEREXIT event
            EventTrigger.Entry pointerExit = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            pointerExit.callback.AddListener(OnPointerExit);
            trigger.triggers.Add(pointerExit);
        }

        /// <summary>
        /// Called when a selectable is selected
        /// </summary>
        /// <param name="eventData">Data of the selectable</param>
        public void OnSelect(BaseEventData eventData)
        {
            // TODO: Play audio on select
            
            lastSelected = eventData.selectedObject.GetComponent<Selectable>();

            if (excludeFromAnimation.Contains(eventData.selectedObject))
                return;

            Vector3 newScale = eventData.selectedObject.transform.localScale * selectedAnimationScale;
            scaleUpTween = eventData.selectedObject.transform.DOScale(newScale, scaleDuration);
        }
        /// <summary>
        /// Called when a selectable is deselected
        /// </summary>
        /// <param name="eventData">Data of the selectable</param>
        public void OnDeselect(BaseEventData eventData)
        {
            if (excludeFromAnimation.Contains(eventData.selectedObject))
                return;
            
            Selectable sel = eventData.selectedObject.GetComponent<Selectable>();
            scaleDownTween = eventData.selectedObject.transform.DOScale(scales[sel], scaleDuration);
        }
        /// <summary>
        /// Called when pointer enters a selectable
        /// </summary>
        /// <param name="eventData">Data of the selectable</param>
        public void OnPointerEnter(BaseEventData eventData)
        {
            PointerEventData pointerEventData = eventData as PointerEventData;
            if (pointerEventData != null)
            {
                Selectable sel = pointerEventData.pointerEnter.GetComponentInParent<Selectable>();
                if (sel == null)
                    sel = pointerEventData.pointerEnter.GetComponentInChildren<Selectable>();

                pointerEventData.selectedObject = sel.gameObject;
            }
        }
        /// <summary>
        /// Called when pointer exits a selectable
        /// </summary>
        /// <param name="eventData">Data of the selectable</param>
        public void OnPointerExit(BaseEventData eventData)
        {
            PointerEventData pointerEventData = eventData as PointerEventData;
            if (pointerEventData != null)
            {
                pointerEventData.selectedObject = null;
            }
        }
        
        protected virtual void OnNavigate(Vector2 direction)
        {
            if (EventSystem.current.currentSelectedGameObject == null && lastSelected != null)
                EventSystem.current.SetSelectedGameObject(lastSelected.gameObject);
        }
    }
}

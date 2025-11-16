using System;
using Core.Shared.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Shared
{
    /// <summary>
    /// Tool that can be used by character(weapon, whistle, etc.).
    /// </summary>
    public abstract class PlayerTool : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField, Tooltip("UI of this tool."), Required]
        protected GameObject toolUI;

        [Header("Sound")]
        [SerializeField, Tooltip("This sound plays when this tool is equipped."), Required]
        private AudioClip toolEquipSound;

        [Header("Animation Points")]
        [SerializeField, Tooltip("Defines the key points in the player's animation for this specific tool.")]
        protected ToolAnimationKeyPoints keyPoints;

        [Space]
        [Header("Events")]
        [Tooltip("Invokes when player uses main action(LMB) of the tool.")] 
        public UnityEvent OnMainUse;
        [Tooltip("Invokes when player uses reload(R) of the tool.")]
        public UnityEvent OnReload;
        [Tooltip("Invokes when player uses secondary action(RMB) of the tool.")]
        public UnityEvent OnSecondaryUse;


        /// <summary>
        /// Audio clip which has to be played, when player equips tool.
        /// </summary>
        public AudioClip EquipSound => toolEquipSound;


        /// <summary>
        /// Called when LMB pressed.
        /// </summary>
        public abstract void MainUsageStarted(Observable<Vector3> cursorWorldPosition);
        /// <summary>
        /// Called when LMB released.
        /// </summary>
        public abstract void MainUsageFinished();

        /// <summary>
        /// Logic of tool reload.
        /// </summary>
        public abstract void Reload();

        /// <summary>
        /// Called when RMB pressed.
        /// </summary>
        public abstract void SecondaryUsageStarted(Observable<Vector3> cursorWorldPosition);
        /// <summary>
        /// Called when RMB released.
        /// </summary>
        public abstract void SecondaryUsageFinished();

        
        /// <summary>
        /// Invokes, when player removes tool.
        /// </summary>
        public virtual void HideTool()
        {
            HideUI();
        }
        /// <summary>
        /// Invokes, when player equips tool.
        /// </summary>
        public virtual void ShowTool()
        {
            ShowUI();
        }


        protected virtual void HideUI()
        {
            toolUI.SetActive(false);
        }
        protected virtual void ShowUI()
        {
            toolUI.SetActive(true);
        }
    }
}

[Serializable]
public struct ToolAnimationKeyPoints
{
    [field: Space, Header("Right hand")]
    [field: SerializeField,Tooltip("Used for dynamic animations of right hand.")]
    public Transform RightHandTarget { get; private set; }
    [field: SerializeField, Tooltip("Used to support dynamic animations of right hand.")]
    public Transform RightHandHint { get; private set; }

    [field: Space, Header("Left hand")]
    [field: SerializeField, Tooltip("Used for dynamic animations of left hand.")]
    public Transform LeftHandTarget { get; private set; }
    [field: SerializeField, Tooltip("Used to support dynamic animations of left hand.")]
    public Transform LeftHandHint { get; private set; }

    [field: Space]
    [field: SerializeField, Tooltip("Used for dynamic chest rotation.")]
    public Transform ChestTarget { get; private set; }
}
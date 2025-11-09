using System;
using Core.Shared.Utilities;
using UnityEngine;

namespace Core.Shared
{
    /// <summary>
    /// Interface for every tool that can be used by character(weapon, whistle, etc.).
    /// </summary>
    public abstract class PlayerTool : MonoBehaviour
    {
        [SerializeField, Tooltip("UI of this tool."), Required]
        private GameObject toolUI;
        [SerializeField, Tooltip("This sound plays when this tool is equipped."), Required]
        private AudioClip toolEquipSound;


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


        public virtual void HideTool()
        {
            HideUI();
        }
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
    [Header("Right hand")]
    public Transform RightHandTarget;
    public Transform RightHandHint;

    [Space]
    [Header("Left hand")]
    public Transform LeftHandTarget;
    public Transform LeftHandHint;

    [Space]
    public Transform ShouldersTarget;
}
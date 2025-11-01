using UnityEngine;

namespace Core.Shared
{
    /// <summary>
    /// Interface for every tool that can be used by character(weapon, whistle, etc.).
    /// </summary>
    public interface IPlayerTool
    {
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
    }
}
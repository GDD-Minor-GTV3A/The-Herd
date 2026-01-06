using System.Collections.Generic;
using Core.Events;
using Core.Shared;
using UnityEngine;

namespace Gameplay
{
    public class PauseManager : MonoBehaviour
    {
        private List<IPausable> pausableobjects = new List<IPausable>();
        private bool isGamePaused = false;


        public void Initialize()
        {
            EventManager.AddListener<RegisterNewPausableEvent>(AddNewPausable);
            isGamePaused = false;
        }


        private void AddNewPausable(RegisterNewPausableEvent evt)
        {
            pausableobjects.Add(evt.NewPausable);
        }


        public void PauseGame()
        {
            isGamePaused = true;
            foreach (IPausable p in pausableobjects)
                p.Pause();
        }

        public void ResumeGame()
        {
            isGamePaused = false;
            foreach (IPausable p in pausableobjects)
                p.Resume();
        }


        /// <summary>
        /// test
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (isGamePaused)
                    ResumeGame();
                else
                    PauseGame();
            }
        }
    }
}
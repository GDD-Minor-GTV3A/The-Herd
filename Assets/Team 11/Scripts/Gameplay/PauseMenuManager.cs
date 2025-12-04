using UnityEngine;
using Core.InputSystem;

namespace Gameplay
{
    public class PauseMenuManager : MonoBehaviour
    {
        [Header("Inputs")]
        [SerializeField] private InputHandler inputHandler;

        [Header("Canvas Menu")]
        [SerializeField] private Canvas menuCanvas;
        private GameObject pauseMenu;

        [Header("Manu pages")]
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private GameObject settingsMenu;

        private bool isMenuOpen = true;

        void Start()
        {
            pauseMenu = menuCanvas.gameObject;

            OpenPauseMenu();
        }
        
        void OnEnable()
        {
            inputHandler.PauseEvent += OpenMenu;
        }

        void OnDisable()
        {
            inputHandler.PauseEvent -= OpenMenu;
        }

        private void OpenMenu()
        {
            if (!isMenuOpen)
            {
                OpenPauseMenu();
                isMenuOpen = true;
            }
        }

        /// <summary>
        /// Open the pause menu
        /// </summary>
        public void OpenPauseMenu()
        {
            // Set canvas to be visible
            pauseMenu.SetActive(true);

            // Set the correct menu to show
            mainMenu.SetActive(true);
            settingsMenu.SetActive(false);
        }

        /// <summary>
        /// Close the pause menu
        /// </summary>
        public void ClosePauseMenu()
        {
            pauseMenu.SetActive(false);

            isMenuOpen = false;
        }

        /// <summary>
        /// Open settings menu
        /// </summary>
        public void OpenSettingsMenu()
        {
            settingsMenu.SetActive(true);
            mainMenu.SetActive(false);
        }

        /// <summary>
        /// Close settings menu
        /// </summary>
        public void CloseSettingsMenu()
        {
            mainMenu.SetActive(true);
            settingsMenu.SetActive(false);
        }
    }
}

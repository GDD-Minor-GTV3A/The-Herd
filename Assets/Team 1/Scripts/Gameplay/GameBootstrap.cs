using Gameplay.Collectables;
using Gameplay.FogOfWar;

using UnityEngine;

namespace Gameplay 
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField]
        private Gameplay.Player.Player player;

        [SerializeField]
        private Gameplay.Dog.Dog dog;

        [SerializeField]
        private PauseManager pauseManager;

        [SerializeField]
        private FogOfWarManager fogOfWar;

        [SerializeField]
        private CollectablesManager collectablesManager;


        private void Start()
        {
            if (pauseManager != null)
                pauseManager.Initialize();

            if (player != null)
                player.Initialize();

            if (dog != null)
                dog.Initialize();

            if (fogOfWar != null)
                fogOfWar.Initialize();

            if (collectablesManager != null)
                collectablesManager.Initialize();
        }
    }
}
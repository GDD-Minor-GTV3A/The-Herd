using Core.Economy;

using UnityEngine;
using UnityEngine.AI;

namespace AI.Shaman
{
    [RequireComponent(typeof(ShamanStateManager), typeof(NavMeshAgent))]
    public class Shaman : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerWallet _player;

        [Header("Shaman Currencies")]
        [SerializeField] private CurrencyData _currency;


        private Wallet _wallet;
        private ShamanStateManager _stateManager;

        // for test, needs to be moved to bootstrap
        void Start()
        {
            Initialize();
        }

        /// <summary> 
        /// initialization method.
        /// </summary>
        public void Initialize()
        {
            _wallet = new Wallet();
            _stateManager = GetComponent<ShamanStateManager>();
            _stateManager.Initialize(this);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                Trade();
            }
        }

        public void Trade()
        {
            const int amount = 25;

            bool success = Trading.Transfer(
                _player.GetWallet(),
                _wallet,
                _currency,
                amount
            );

            if (success)
            {
                Debug.Log($"Trade successful ! {amount} {_currency.DisplayName}");
                Debug.Log($"Shaman now has: {_wallet.GetAmount(_currency)} {_currency.DisplayName}");
                Debug.Log($"Player now has: {_player.GetWallet().GetAmount(_currency)} {_currency.DisplayName}");
            }
            else
            {
                Debug.Log("Trade failed - not enough " + _currency.DisplayName);
                Debug.Log($"Shaman now has: {_wallet.GetAmount(_currency)} {_currency.DisplayName}");
                Debug.Log($"Player now has: {_player.GetWallet().GetAmount(_currency)} {_currency.DisplayName}");
            }
        }
    }
}
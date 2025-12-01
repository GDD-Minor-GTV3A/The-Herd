using Core.Shared.Utilities;
using TMPro;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Handles updating amount of ammo text.
    /// </summary>
    public class AmmoAmountUI : MonoBehaviour
    {
        [SerializeField, Tooltip("TMP Text component for ammo amount."), Required] 
        private TextMeshProUGUI ammoText;


        public void UpdateAmmoText(int ammoAmount)
        {
            if (ammoText != null)
            {
                ammoText.text = $"{ammoAmount}";
            }
        }
    }
}
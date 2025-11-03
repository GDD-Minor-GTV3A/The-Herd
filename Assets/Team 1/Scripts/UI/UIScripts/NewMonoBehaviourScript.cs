using UnityEngine;
using TMPro;

public class AmmoDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText; // drag your TMP text here in the Inspector
    [SerializeField] private Rifle rifle;              // reference to the script that holds the ammo value

    private void Start()
    {
        UpdateAmmoText(); // set initial value
    }

    private void Update()
    {
        UpdateAmmoText(); // update every frame (simple)
    }

    private void UpdateAmmoText()
    {
        if (ammoText != null && rifle != null)
        {
            ammoText.text = $"{rifle.CurrentAmmo}";
        }
    }
}

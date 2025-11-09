using UnityEngine;
using TMPro;

public class AmmoDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText; 
    [SerializeField] private Rifle rifle;              

    private void Start()
    {
        UpdateAmmoText(); 
    }

    private void Update()
    {
        UpdateAmmoText(); 
    }

    private void UpdateAmmoText()
    {
        if (ammoText != null && rifle != null)
        {
            ammoText.text = $"{rifle.CurrentAmmo}";
        }
    }
}

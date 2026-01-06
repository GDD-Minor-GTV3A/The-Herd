using System;

using Core.AI.Sheep.Event;
using Core.Events;

using Gameplay.SanityEffects;

using UnityEngine;
using UnityEngine.UI;

public class SanityBarUI : MonoBehaviour
{
    [SerializeField] private Image sanityBarSprite;

    private void Start()
    {
        EventManager.AddListener<SanityChangeEvent>(OnSanityChanged);
    }

    private void OnSanityChanged(SanityChangeEvent evt)
    {
        UpdateSanity(100, evt.Percentage);

    }

    public void UpdateSanity(float maxSanity, float currentSanity)
    {
        sanityBarSprite.fillAmount = currentSanity / maxSanity; 
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<SanityChangeEvent>(OnSanityChanged);
    }
}

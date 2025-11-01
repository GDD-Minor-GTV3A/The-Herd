using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomEditor.Attributes;
using UnityEngine;

namespace Gameplay.Effects 
{
    public class DamageEffect : MonoBehaviour
    {
        [Serializable]
        private class ShowableList<T> 
        { 
            public List<T> Items;
        }




        [SerializeField, Tooltip("Color of object, when it takes damage.")] 
        private Color damageColor = Color.red;
        [SerializeField, Tooltip("Takes all renderers on object and it's childs. To make it work add this component on root object.")] 
        private bool autoFindMeshRenderers = false;
        [SerializeField, ShowIf("autoFindMeshRenderers"), Tooltip("Include inactive renderers in list.")] 
        private bool includeInactive = false;
        [SerializeField, ShowIf("autoFindMeshRenderers", true), Tooltip("All MeshRenderers which will change color.")]
        private ShowableList<Renderer> meshRenderers;


        private List<Material> materialsToChangeColor;
        private Coroutine blinkCoroutine;


        public void Initialize()
        {
            if (autoFindMeshRenderers)
            {
                meshRenderers.Items = new List<Renderer>();
                meshRenderers.Items = GetComponentsInChildren<Renderer>(includeInactive).ToList();
            }

            materialsToChangeColor = new List<Material>();


            foreach (Renderer renderer in meshRenderers.Items)
            {
                foreach (Material material in renderer.materials)
                    materialsToChangeColor.Add(material);
            }
        }


        public void Blink(float duration)
        {
            if (blinkCoroutine != null)
                StopCoroutine(blinkCoroutine);

            blinkCoroutine = StartCoroutine(BlinkRoutine(duration));
        }

        private IEnumerator BlinkRoutine(float duration)
        {
            List<Color> _baseColors = new List<Color>();
            foreach (Material material in materialsToChangeColor)
                _baseColors.Add(material.color);

            float _currentTime = 0f;
            float _redDuration = duration / 3;
            float _fadeDuration = (duration - _redDuration) / 2;


            while (_currentTime < _fadeDuration)
            {
                _currentTime += Time.deltaTime;

                for (int i = 0; i < materialsToChangeColor.Count; i++)
                {
                    materialsToChangeColor[i].color = Color.Lerp(_baseColors[i], damageColor, _currentTime / _fadeDuration);
                }
                yield return null;
            }

            yield return new WaitForSeconds(_redDuration);

            _currentTime = 0f;

            while (_currentTime < _fadeDuration)
            {
                _currentTime += Time.deltaTime;

                for (int i = 0; i < materialsToChangeColor.Count; i++)
                {
                    materialsToChangeColor[i].color = Color.Lerp(damageColor, _baseColors[i], _currentTime / _fadeDuration);
                }
                yield return null;
            }
        }
    }
}
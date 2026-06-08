using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NINESOFT.COMPLETEGAMES.CARJAM3D
{
    public class ShopVisualItem : MonoBehaviour
    {
        [SerializeField] private GameObject[] VisualItems;

        private void Awake()
        {
            UpdateVisualItem();
            ShopManager.Instance.OnItemChanged += UpdateVisualItem;
            GameManager.Instance.OnGameStart += () =>
            {
                UpdateVisualItem(ShopManager.Instance.SelectedItem);
            };
        }

        private void UpdateVisualItem(int index = 0)
        {
            for (int i = 0; i < VisualItems.Length; i++)
            {
                VisualItems[i].SetActive(false);
            }

            int idx = index;
            VisualItems[idx].SetActive(true);
        }
    }
}
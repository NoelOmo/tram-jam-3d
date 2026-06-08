using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NINESOFT.COMPLETEGAMES.CARJAM3D
{
    public class ShopManager : Manager<ShopManager>
    {
        [System.Serializable]
        public class ShopItem
        {
            public int Index;
            public string Name;
            public Sprite Icon;
            public int Price;

            public bool IsPurchased { get => Price == 0 || PlayerPrefs.GetInt("shopItem_purchased_" + Index, 0) == 1; set => PlayerPrefs.SetInt("shopItem_purchased_" + Index, value ? 1 : 0); }
        }
        public int SelectedItem { get => PlayerPrefs.GetInt("selectedShopItem", 0); private set => PlayerPrefs.SetInt("selectedShopItem", value); }

        [Header("ITEMS")]
        [SerializeField] private List<ShopItem> ShopItems;

        [Header("UI")]
        [SerializeField] private Transform ShopItemUIParent;
        [SerializeField] private ShopItemUI ShopItemUIPrefab;
        private List<ShopItemUI> CreatedShopItemUIs = new List<ShopItemUI>();

        public IntAction OnItemChanged;

        private void Start()
        {
            CreateShopItemUIs();
            UpdateShopItemUIs();
        }

        private void CreateShopItemUIs()
        {
            for (int i = 0; i < ShopItems.Count; i++)
            {
                var createdUI = Instantiate(ShopItemUIPrefab, ShopItemUIParent);
                CreatedShopItemUIs.Add(createdUI);
            }
        }
        private void UpdateShopItemUIs()
        {
            for (int i = 0; i < CreatedShopItemUIs.Count; i++)
            {
                CreatedShopItemUIs[i].Init(ShopItems[i]);
                if (i == SelectedItem)
                    CreatedShopItemUIs[i].Selected();
            }
        }

        public void PreviewItem(int idx)
        {
            OnItemChanged?.Invoke(idx);

            AudioManager.Instance.PlaySound(AudioManager.Instance.popSound);
        }
        public void BuyItem(int idx)
        {
            var curItem = ShopItems[idx];
            if (DataManager.Instance.CheckAndSpendMoney(curItem.Price))
            {
                curItem.IsPurchased = true;
                SelectItem(idx);

                AudioManager.Instance.PlaySound(AudioManager.Instance.purchaseSound);
            }
            else
            {
                UIManager.Instance.ShowMessageBox("Money is not enough!");
            }
        }
        public void SelectItem(int idx)
        {
            SelectedItem = idx;
            OnItemChanged?.Invoke(SelectedItem);
            UpdateShopItemUIs();

            AudioManager.Instance.PlaySound(AudioManager.Instance.popSound);
        }
    }
}
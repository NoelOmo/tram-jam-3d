using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace NINESOFT.COMPLETEGAMES.CARJAM3D
{
    public class ShopItemUI : MonoBehaviour
    {
        [SerializeField] private Image IconImage;
        [SerializeField] private TextMeshProUGUI priceText;

        [SerializeField] private Button SelectButton;
        [SerializeField] private TextMeshProUGUI SelectButtonText;
        [SerializeField] private GameObject BuyButton;
        [SerializeField] private GameObject SelectedTick;

        [SerializeField] private Color32 ChangeBtnColor;
        [SerializeField] private Color32 SelectedBtnColor;


        private int ItemIdx;

        public void Init(ShopManager.ShopItem shopItem)
        {
            ItemIdx = shopItem.Index;
            IconImage.sprite = shopItem.Icon;
            priceText.SetText(shopItem.Price.ToString());

            if (shopItem.IsPurchased)
            {
                BuyButton.SetActive(false);
                SelectButton.gameObject.SetActive(true);
            }
            else
            {
                BuyButton.SetActive(true);
                SelectButton.gameObject.SetActive(false);
            }

            SelectedTick.SetActive(false);
            SelectButtonText.SetText("CHANGE");
            SelectButton.image.color = ChangeBtnColor;
        }

        public void Selected()
        {
            SelectedTick.SetActive(true);
            SelectButtonText.SetText("SELECTED");
            SelectButton.image.color = SelectedBtnColor;
        }

        public void PreviewItem()
        {
            ShopManager.Instance.PreviewItem(ItemIdx);
        }

        public void BuyItem()
        {
            ShopManager.Instance.BuyItem(ItemIdx);
        }

        public void SelectItem()
        {
            ShopManager.Instance.SelectItem(ItemIdx);
        }
    }
}
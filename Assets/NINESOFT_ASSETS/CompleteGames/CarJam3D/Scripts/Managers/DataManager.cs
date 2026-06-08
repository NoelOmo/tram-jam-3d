using System.Collections;
using UnityEngine;
namespace NINESOFT.COMPLETEGAMES.CARJAM3D
{
    public enum MoneyType { Gem }
    public class DataManager : Manager<DataManager>
    {

        public int Gem { get { return PlayerPrefs.GetInt("gem"); } set { PlayerPrefs.SetInt("gem", value); } }
        public Action OnEarnedGem, OnSpentGem;

        private void Start()
        {
            AddGem(0);
        }

        public void AddGem(int amount)
        {
            Gem += amount;
            UIManager.Instance.UpdateGemText(Gem);

            if (amount > 0) OnEarnedGem?.Invoke();
            else if (amount < 0) OnSpentGem?.Invoke();
        }

        public bool CheckAndSpendMoney(int price, MoneyType moneyType = MoneyType.Gem)
        {
            bool canPurchased = MoneyIsEnough(price, moneyType);

            if (canPurchased)
                switch (moneyType)
                {
                    case MoneyType.Gem:
                        if (canPurchased) AddGem(-price);
                        break;
                }
            return canPurchased;
        }


        public bool MoneyIsEnough(int price, MoneyType moneyType = MoneyType.Gem)
        {
            bool enough = false;
            switch (moneyType)
            {
                case MoneyType.Gem:
                    enough = Gem >= price;
                    break;
            }
            return enough;
        }

        public void WatchAdsAndAddGem()
        {
            CompleteGameADManager.Instance.PlayRw(() =>
             {
                 AddGem(20);
                 UIManager.Instance.ShowMessageBox("YOU EARNED 20 GEM");
             });
        }

    }
}
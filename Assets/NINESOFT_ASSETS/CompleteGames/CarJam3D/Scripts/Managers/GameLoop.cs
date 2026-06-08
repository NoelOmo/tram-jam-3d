using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NINESOFT.COMPLETEGAMES.CARJAM3D
{
    public class GameLoop : Manager<GameLoop>
    {
        public List<PathLine> _paths;

        public LevelProperties GetCurrentLevel
        {
            get
            {
                if (curLevel == null) curLevel = FindObjectOfType<LevelProperties>();
                return curLevel;
            }
        }
        private LevelProperties curLevel;

        private void Start()
        {
            SortBoxes();
            GameManager.Instance.OnGameWin += () => { StartCoroutine(GiveLevelPrize()); };
        }

        private IEnumerator GiveLevelPrize()
        {
            int earnedGems = GameLoop.Instance.GetCurrentLevel.LevelPrize;
            DataManager.Instance.AddGem(earnedGems);
            UIManager.Instance.WinActivate(true, earnedGems);

            yield return new WaitForSeconds(.5f);
            int flyingGems = earnedGems;
            if (flyingGems > 10) flyingGems = 10;
            for (int i = 0; i < flyingGems; i++)
            {
                UIManager.Instance.SpawnCoinUI(Vector3.zero, false);
                yield return new WaitForSeconds(0.05f);
            }

        }

        public void SortBoxes()
        {

            for (int i = 0; i < GetCurrentLevel.Boxes.Count; i++)
            {
                Vector3 pos = GetCurrentLevel.Boxes[i].transform.position;
                pos.x = i * -5.2f;

                int ii = i;
                GetCurrentLevel.Boxes[ii].transform.DOMove(pos, .35f).OnComplete(() =>
                {
                    if (ii == 0)
                    {
                        SlotManager.Instance.CheckSlots();
                    }
                });
            }

            GetCurrentLevel.Boxes[0].transform.DOScale(Vector3.one * 1.25f, .25f);
        }

        public void NextBox()
        {

            if (GetCurrentLevel.Boxes.Count == 0)
            {
                //win
                GameManager.Instance.Win();
                return;
            }

            Box lastBox = GetCurrentLevel.Boxes[0];
            GetCurrentLevel.Boxes.RemoveAt(0);
            lastBox.transform.DOMove(new Vector3(10f, 0, -10), .35f);
            lastBox.ShowParticle();
            lastBox.transform.DOScale(Vector3.one * .001f, .35f).SetDelay(.35f);


            if (GetCurrentLevel.Boxes.Count == 0)
            {
                //win
                GameManager.Instance.Win();
                return;
            }


            SortBoxes();
        }

        public void ControlAllPathWays()
        {
            for (int i = 0; i < _paths.Count; i++)
            {
                _paths[i].CheckConnectionStatus();
            }

        }

    }
}
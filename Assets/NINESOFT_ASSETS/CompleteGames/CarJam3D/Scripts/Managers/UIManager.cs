using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
namespace NINESOFT.COMPLETEGAMES.CARJAM3D
{
    public class UIManager : Manager<UIManager>
    {
        [Header("Panels")]
        [SerializeField] private GameObject GamePanel;
        [SerializeField] private GameObject MenuPanel;
        [SerializeField] private GameObject WinPanel;
        [SerializeField] private GameObject LosePanel;
        [SerializeField] private GameObject SlotUnlockPanel;

        private SlotUnlockUI _slotUnlockUI;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI LevelText;
        [SerializeField] private TextMeshProUGUI[] LevelTextInMenu;
        [SerializeField] private TextMeshProUGUI GemText;
        [SerializeField] private TextMeshProUGUI earnedGemText;

        [Header("Images")]
        [SerializeField] private Transform Tap;
        [SerializeField] private Transform TapClick;
        [SerializeField] private Transform GemTarget;

        [Header("MENU")]
        [SerializeField] private GameObject[] TabButtons;
        [SerializeField] private GameObject[] TabPanels;

        [Header("LOADING")]
        [SerializeField] private GameObject LoadingPanel;

        [Header("MessageBox")]
        [SerializeField] private GameObject MessageBox;
        [SerializeField] private TextMeshProUGUI MessageText;



#if UNITY_EDITOR

        private void Update()
        {
            /*
            if (Tap != null && TapClick != null)
            {

                Tap.position = Input.mousePosition;
                TapClick.position = Input.mousePosition;
                if (Input.GetMouseButtonDown(0))
                {
                    Tap.gameObject.SetActive(false);
                    TapClick.gameObject.SetActive(true);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    TapClick.gameObject.SetActive(false);
                    Tap.gameObject.SetActive(true);
                }
            }
            */
        }
#endif

        protected override void Awake()
        {
            base.Awake();
            EnsureSlotUnlockPanel();
        }

        private void EnsureSlotUnlockPanel()
        {
            if (SlotUnlockPanel != null)
            {
                _slotUnlockUI = SlotUnlockPanel.GetComponent<SlotUnlockUI>();
                if (_slotUnlockUI == null)
                    _slotUnlockUI = SlotUnlockPanel.AddComponent<SlotUnlockUI>();
                return;
            }

            if (WinPanel == null) return;

            _slotUnlockUI = SlotUnlockUI.CreateFromWinUI(WinPanel);
            SlotUnlockPanel = _slotUnlockUI.gameObject;
        }

        private void Start()
        {
            int curLevel = (LevelManager.loadLastIndex());
            LevelText.text = "Level " + curLevel;
            for (int i = 0; i < LevelTextInMenu.Length; i++)
            {
                LevelTextInMenu[i].SetText((curLevel + i).ToString());
            }
        }

        public void MenuActivate(bool activate) { if (MenuPanel == null) return; MenuPanel.SetActive(activate); }
        public void GameActivate(bool activate) { if (GamePanel == null) return; GamePanel.SetActive(activate); }
        public void LoseActivate(bool activate) { if (LosePanel == null) return; LosePanel.SetActive(activate); }
        public void WinActivate(bool activate, int earnedGems) { if (WinPanel == null) return; WinPanel.SetActive(activate); earnedGemText.SetText("+" + FormatCash(earnedGems)); }
        public void SlotUnlockActivate(bool activate)
        {
            if (_slotUnlockUI == null) EnsureSlotUnlockPanel();
            if (_slotUnlockUI == null) return;

            if (activate) _slotUnlockUI.Show();
            else _slotUnlockUI.Hide();
        }

        public bool BlocksSlotUnlockInput()
        {
            return _slotUnlockUI != null && _slotUnlockUI.BlocksSlotInput;
        }
        public void UpdateGemText(float value) { if (GemText != null) GemText.text = FormatCash(value); }

        public void ShowLoading(bool loading) { LoadingPanel.SetActive(loading); }
        public void Play() => GameManager.Instance.Play();
        public void ShowMessageBox(string content)
        {
            MessageText.SetText(content);
            MessageBox.SetActive(true);
        }
        public void ChangemenuTab(int idx)
        {
            for (int i = 0; i < TabButtons.Length; i++)

            {
                var btn = TabButtons[i].transform;
                if (idx == i)
                {
                    btn.DOScale(Vector3.one * 1.4f, .1f).OnComplete(() =>
                    {
                        btn.DOScale(Vector3.one * 1.2f, .1f);
                    });
                    TabPanels[i]?.SetActive(true);
                }
                else
                {
                    btn.DOScale(Vector3.one * .9f, .1f);
                    TabPanels[i]?.SetActive(false);
                }
            }

            try
            {
                if (AudioManager.Instance != null && AudioManager.Instance.popSound != null) AudioManager.Instance.PlaySound(AudioManager.Instance.popSound);
            }
            catch { }
        }

        public void SpawnCoinUI(Vector3 startPos, bool worldPos = false)
        {
            if (!worldPos) startPos = earnedGemText.transform.position;

            Transform target = GemTarget;
            FlyingBlockUI flayingUI = PoolManager.Instance.GetObjectFromPool<FlyingBlockUI>("FlayingBlockUI", startPos, Quaternion.identity, 5);
            flayingUI.transform.SetParent(target);
            flayingUI.Initialize(startPos, target.position, worldPos);
        }

        public static string FormatCash(double Value)
        {
            if (Value >= 10000000000000000)
            {
                return (Value / 1000000000000000d).ToString("0.#AA");
            }
            if (Value >= 1000000000000000)
            {
                return (Value / 1000000000000000d).ToString("0.##AA");
            }
            if (Value >= 10000000000000)
            {
                return (Value / 1000000000000d).ToString("0.#BB");
            }
            if (Value >= 1000000000000)
            {
                return (Value / 1000000000000d).ToString("0.##BB");
            }
            if (Value >= 10000000000)
            {
                return (Value / 1000000000d).ToString("0.#B");
            }
            if (Value >= 1000000000)
            {
                return (Value / 1000000000d).ToString("0.##B");
            }
            if (Value >= 100000000)
            {
                return (Value / 1000000d).ToString("0.#M");
            }
            if (Value >= 1000000)
            {
                return (Value / 1000000d).ToString("0.##M");
            }
            if (Value >= 100000)
            {
                return (Value / 1000d).ToString("0.#K");
            }
            if (Value >= 1000)
            {
                return (Value / 1000d).ToString("0.##K");
            }

            return Value.ToString("0.#");
        }
    }
}
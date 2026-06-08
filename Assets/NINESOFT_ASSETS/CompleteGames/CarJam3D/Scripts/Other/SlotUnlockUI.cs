using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace NINESOFT.COMPLETEGAMES.CARJAM3D
{
    public class SlotUnlockUI : MonoBehaviour
    {
        [SerializeField] private string titleMessage = "PARKING UNLOCKED!";
        [SerializeField] private string subtitleMessage = "New parking slot available!";
        [SerializeField] private string buttonMessage = "CONTINUE";

        private TextMeshProUGUI _titleText;
        private TextMeshProUGUI _subtitleText;
        private TextMeshProUGUI _buttonText;
        private GameObject _gemIcon;
        private Button _continueButton;
        private Animator _animator;
        private bool _initialized;
        private float _dismissedAt = -1f;
        private const float InputBlockGraceSeconds = 0.35f;

        public bool BlocksSlotInput =>
            gameObject.activeSelf || Time.unscaledTime - _dismissedAt < InputBlockGraceSeconds;

        private void Awake()
        {
            Initialize();
        }

        public static SlotUnlockUI CreateFromWinUI(GameObject winPanel)
        {
            var clone = Instantiate(winPanel, winPanel.transform.parent);
            clone.name = "SlotUnlockUI";
            clone.SetActive(false);

            var ui = clone.GetComponent<SlotUnlockUI>();
            if (ui == null)
                ui = clone.AddComponent<SlotUnlockUI>();

            return ui;
        }

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
            _animator = GetComponent<Animator>();

            foreach (var text in GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                if (text.text == "COMPLETED")
                    _titleText = text;
                else if (text.text == "NEXT")
                    _buttonText = text;
                else if (text.name == "gemText")
                    _subtitleText = text;
            }

            var gemIconTransform = transform.Find("Mid/GemIcon");
            if (gemIconTransform != null)
                _gemIcon = gemIconTransform.gameObject;

            _continueButton = GetComponentInChildren<Button>(true);
            if (_continueButton != null)
            {
                _continueButton.onClick = new Button.ButtonClickedEvent();
                _continueButton.onClick.AddListener(Hide);
            }

            EnsureInputBlocker();
            ApplyCopy();
        }

        private void EnsureInputBlocker()
        {
            var blocker = GetComponent<Image>();
            if (blocker == null)
                blocker = gameObject.AddComponent<Image>();

            blocker.color = new Color(0f, 0f, 0f, 0.78431374f);
            blocker.raycastTarget = true;
        }

        private void ApplyCopy()
        {
            if (_titleText != null)
                _titleText.SetText(titleMessage);

            if (_subtitleText != null)
                _subtitleText.SetText(subtitleMessage);

            if (_buttonText != null)
                _buttonText.SetText(buttonMessage);

            if (_gemIcon != null)
                _gemIcon.SetActive(false);
        }

        public void Show()
        {
            ApplyCopy();
            gameObject.SetActive(true);
            _animator?.Rebind();
            _animator?.Update(0f);

            if (AudioManager.Instance != null && AudioManager.Instance.winSound != null)
                AudioManager.Instance.PlaySound(AudioManager.Instance.winSound);
        }

        public void Hide()
        {
            _dismissedAt = Time.unscaledTime;
            gameObject.SetActive(false);
        }
    }
}

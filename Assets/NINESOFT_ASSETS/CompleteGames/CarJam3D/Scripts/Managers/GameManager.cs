using UnityEngine;
namespace NINESOFT.COMPLETEGAMES.CARJAM3D
{
    public enum GameStatus { Null, Playing, Win, Finish, GameOver, Pause }
    public delegate void Action();
    public delegate void IntAction(int i);

    public class GameManager : Manager<GameManager>
    {
        public Action OnGameStart, OnGameFail, OnGameWin;

        [Header("Game Status")]
        public GameStatus GameStatus;
        [SerializeField] private bool PlayModeInStart;

        public bool IsFirstLogin { get => PlayerPrefs.GetInt("first") == 0 ? true : false; set => PlayerPrefs.SetInt("first", value ? 0 : 1); }

        [Header("Player")]
        private Transform _player;
        public Transform Player
        {
            get
            {
                if (_player == null)
                {
                    _player = GameObject.FindWithTag("Player").transform;
                }
                return _player;
            }
        }

        private void Start()
        {
            Application.targetFrameRate = 300;
            GameStatus = GameStatus.Null;
            if (PlayModeInStart) Play();


            if (IsFirstLogin)
            {
                LevelManager.Instance.OpenMenu = true;
                IsFirstLogin = false;

                /*
                //for demo apk
                DataManager.Instance.AddGem(10000);
                */

            }

            if (!LevelManager.Instance.OpenMenu) Play();
            else UIManager.Instance.MenuActivate(true);

            UIManager.Instance.ChangemenuTab(1);
        }

        public bool IsPlaying() { return GameStatus == GameStatus.Playing; }

        public void Play()
        {
            GameStatus = GameStatus.Playing;

            UIManager.Instance.MenuActivate(false);
            UIManager.Instance.GameActivate(true);

            OnGameStart?.Invoke();
        }


        public void Fail()
        {
            if (GameStatus == GameStatus.GameOver || GameStatus == GameStatus.Win) return;

            GameStatus = GameStatus.GameOver;
            UIManager.Instance.LoseActivate(true);

            OnGameFail?.Invoke();
            AudioManager.Instance.PlaySound(AudioManager.Instance.failSound);

            ShowInter();
        }

        public void Win()
        {
            if (GameStatus == GameStatus.Win || GameStatus == GameStatus.GameOver) return;

            GameStatus = GameStatus.Win;

            OnGameWin?.Invoke();
            AudioManager.Instance.PlaySound(AudioManager.Instance.winSound);

            ShowInter();
        }

        public void NextButtonClicked()
        {
            // ADManager.Instance.ShowInterstitialAd();
        }

        public void TryAgainButtonClicked()
        {
            // ADManager.Instance.ShowInterstitialAd();
        }
        private void ShowInter()
        {
            CompleteGameADManager.Instance.Invoke(nameof(CompleteGameADManager.Instance.ShowInterstitialAd), .05f);
        }
    }
}
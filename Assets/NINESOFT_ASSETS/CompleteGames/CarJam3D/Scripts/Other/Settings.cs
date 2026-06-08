using UnityEngine;
using UnityEngine.UI;
namespace NINESOFT.COMPLETEGAMES.CARJAM3D
{
    public class Settings : MonoBehaviour
    {
        [Header("- SETTINGS -")]
        [SerializeField] private Image soundImage;
        [SerializeField] private Sprite[] soundSprites;


        private void Start()
        {
            GetData();
        }
        private void GetData()
        {
            int i = PlayerPrefs.GetInt("sound");
            soundImage.sprite = soundSprites[i];

            AudioListener.pause = (i != 0);
        }

        public void SoundOnOff()
        {
            int i = PlayerPrefs.GetInt("sound");
            i = i == 0 ? 1 : 0;
            PlayerPrefs.SetInt("sound", i);
            GetData();
        }

        public void ShowPrivacyPolicy()
        {
            var config = CompleteGameADManager.Instance._config;
            if (config != null)
                Application.OpenURL(config.PrivacyLink);
        }

    }
}
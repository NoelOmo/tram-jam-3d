using System;
using UnityEngine;
using UnityEditor;
using NINESOFT.CORE;
using NINESOFT.CORE.EDITOR;
//using GoogleMobileAds.Editor;

namespace NINESOFT.COMPLETEGAMES.CARJAM3D/*.ASSET NAME*/ //<<---- change
{

    public class GameSettingsWindow : EditorWindow
    {
        const string PackageName = "Car Jam 3D";// <<--------- change ASSET NAME       

        CompleteGameConfigration Configration;
        //  GoogleMobileAdsSettings googleSettings;

        int toolbarInt = 0;
        bool admobSdkFounded;


        [MenuItem("NINESOFT/" + PackageName)]

        public static void ShowWindow()
        {
            NSPackageManager.InitPackageInfos();
            EditorWindow.GetWindow<GameSettingsWindow>(utility: true, title: NSPackageManager.GetPackageInfo(PackageName).PackageName);
        }

        private void OnEnable()
        {
            FindAdmob();
        }

        private void OnGUI()
        {
            this.titleContent = new GUIContent(PackageName);
            this.minSize = new Vector2(350, 400);
            this.maxSize = new Vector2(350, 400);

            InitData();
            DrawLayout();
        }
        void InitData()
        {
            if (Configration == null) Configration = Resources.Load<CompleteGameConfigration>("Game Config");
            //  if (googleSettings == null) googleSettings = Resources.Load<GoogleMobileAdsSettings>("GoogleMobileAdsSettings");
        }

        private void FindAdmob()
        {
            if (Resources.Load("GoogleMobileAdsSettings") != null)
            {
                admobSdkFounded = true;
            }
        }

        void DrawLayout()
        {
            GUI.backgroundColor = NSEditorData.TabButtonColor;

            GUIContent[] toolbarStrings = {
                new GUIContent("Asset Info", NSEditorData.GetIcon("e_doc")),
                new GUIContent("Settings", NSEditorData.GetIcon("e_settings")),
            };
            toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings, GUILayout.Height(40));

            switch (toolbarInt)
            {
                case 0:
                    DrawAssetInfo();
                    break;

                case 1:
                    DrawAdmobSettings();
                    break;

                default:
                    break;
            }
        }

        void DrawAssetInfo()
        {
            GUILayout.Space(50f);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(new GUIContent(NSPackageManager.GetPackageInfo(PackageName).PackageName + " v" + NSPackageManager.GetPackageInfo(PackageName).Version), NSEditorData.CenteredBoldStyle);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            EditorGUILayout.LabelField(NSPackageManager.GetPackageInfo(PackageName).ID + " | v" + NSPackageManager.GetPackageInfo(PackageName).Version + " | Last Update: " + NSPackageManager.GetPackageInfo(PackageName).LastUpdateDate, EditorStyles.centeredGreyMiniLabel);

            GUILayout.Space(20f);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("DOCUMENTATION", NSEditorData.GetIcon("e_external_link")), GUILayout.Height(30f), GUILayout.Width(180)))
            {
                Application.OpenURL(NSPackageManager.GetPackageInfo(PackageName).DocLink);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            GUILayout.Space(50f);

            NSEditorData.DrawUILine();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("THANKS FOR PURCHASING!", EditorStyles.whiteLargeLabel, GUILayout.Height(20f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            NSEditorData.DrawUILine();
            GUILayout.Space(10f);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(new GUIContent(" NINESOFT", NSEditorData.GetIcon("e_ns_logo")), EditorStyles.centeredGreyMiniLabel);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        void DrawAdmobSettings()
        {

            GUILayout.Label("Privacy", EditorStyles.boldLabel);

            GUILayout.Space(3);
            Configration.PrivacyLink = EditorGUILayout.TextField("Privacy Link", Configration.PrivacyLink);

            GUILayout.Space(20);
            if (admobSdkFounded)
            {
                GUILayout.Label("Admob Settings (Android)", EditorStyles.boldLabel);

                GUI.backgroundColor = NSEditorData.Green;
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                EditorGUILayout.LabelField("Admob Sdk Found!");
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("APP ID:   Set from Assets/GoogleMobileAds/Resources");

                GUILayout.Space(12);
                Configration.Admob_Android_Banner = EditorGUILayout.TextField("BANNER ID", Configration.Admob_Android_Banner);

                GUILayout.Space(3);
                Configration.Admob_Android_Interstitial = EditorGUILayout.TextField("INTERSTITIALS ID", Configration.Admob_Android_Interstitial);

                GUILayout.Space(3);
                Configration.Admob_Android_RewardedVideo = EditorGUILayout.TextField("REWARDED ID", Configration.Admob_Android_RewardedVideo);

                GUILayout.Space(15);
                EditorGUILayout.HelpBox(new GUIContent("Android Test IDs\nApp Id: ca-app-pub-3940256099942544~3347511713" +
                    "\nBanner: ca-app-pub-3940256099942544/6300978111\nInter: ca-app-pub-3940256099942544/1033173712\nRewarded: ca-app-pub-3940256099942544/5224354917"));


            }
            else
            {
                GUI.backgroundColor = NSEditorData.Red;
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                EditorGUILayout.LabelField("Admob Sdk Not Found!");
                if (GUILayout.Button("Refresh"))
                {
                    FindAdmob();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.HelpBox(new GUIContent("Make sure you have installed the google ads package first.\n\nCheck out:\nMenu -> Assets > Google Mobile Ads > Settings"));
            }

            GUILayout.Space(15);
            Color curCol = GUI.color;
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("SAVE & CLOSE", GUILayout.Height(50)))
            {
                EditorUtility.SetDirty(Configration);
                //  googleSettings.adMobAndroidAppId = Configration.Admob_Android_APPID;
                //  EditorUtility.SetDirty(googleSettings);

                this.Close();
            }
            GUI.backgroundColor = curCol;
            GUILayout.Space(15);

        }


    }
}
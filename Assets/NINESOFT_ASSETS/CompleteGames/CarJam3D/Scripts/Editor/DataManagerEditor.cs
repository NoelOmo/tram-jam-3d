using UnityEngine;
using UnityEditor;

namespace NINESOFT.COMPLETEGAMES.CARJAM3D
{
    [CustomEditor(typeof(DataManager))]
    public class DataManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DataManager myTarget = (DataManager)target;


            if (GUILayout.Button("Add 1000 Gems"))
            {
                myTarget.AddGem(1000);
                Debug.Log("Added 1000 Gems");
            }

            if (GUILayout.Button("# RESET ALL DATA #"))
            {
                PlayerPrefs.DeleteAll();
                Debug.Log("All data has been reset");
            }
        }


    }
}
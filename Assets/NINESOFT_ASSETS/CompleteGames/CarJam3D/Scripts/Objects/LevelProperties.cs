using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace NINESOFT.COMPLETEGAMES.CARJAM3D
{
    public class LevelProperties : MonoBehaviour
{
    [Header("Level Prize (Gem(s))")]public int LevelPrize = 3;
    [HideInInspector] public List<Box> Boxes = new List<Box>();
    void Awake()
    {
        Boxes = GetComponentsInChildren<Box>().OrderByDescending(b => b.transform.position.x).ToList();
    }

#if UNITY_EDITOR
    BoxCollider firstArea = null;
    private void OnDrawGizmos()
    {
        if (firstArea == null)
        {
            foreach (Transform item in transform)
            {
                if (item.CompareTag("Area/First"))
                {
                    firstArea = item.GetComponent<BoxCollider>();
                }
            }
        }

        if (firstArea != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(firstArea.transform.position + firstArea.center, firstArea.size);
        }
        else { Debug.LogError("Please add a 'FirstArea' prefab in this level from Prefabs folder"); }

    }
#endif
}
}
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NINESOFT.COMPLETEGAMES.CARJAM3D
{
    public class Item : MonoBehaviour
    {
        public ItemColor Color;
        [HideInInspector] public List<PathLine> MyPaths = new List<PathLine>();
        [HideInInspector] public bool InBox;
        private bool used;

        [Space(20f)]
        [SerializeField] private MeshRenderer[] meshes;
        [SerializeField] private GameObject particle;
        private void Start()
        {
            Material mat = VisualManager.Instance.GetMaterialByItemColor(Color);
            foreach (var mesh in meshes)
            {
                mesh.material = mat;
            }
            transform.localScale = Vector3.one * .7f;
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            Material mat = VisualManager.Instance.GetMaterialByItemColor(Color);
            foreach (var mesh in meshes)
            {
                mesh.material = mat;
            }
#endif
        }

        private void OnMouseDown()
        {
            Click();
        }

        public void Click()
        {
            if (!GameManager.Instance.IsPlaying()) return;
            if (used) return;
            if (MyPaths != null)
            {
                for (int i = 0; i < MyPaths.Count; i++)
                {
                    if (!MyPaths[i].ConnectedToFirstPoint) continue;
                    MyPaths[i].PathCleared();
                    SlotManager.Instance.SetToSlot(this, MyPaths[i].GetPath);
                    used = true;
                    break;
                }
            }

            if (!used)
            {

                Transform child = transform.GetChild(0);
                child.DOShakePosition(.15f, new Vector3(.1f, 0, .1f)).OnComplete(() =>
                {
                    child.DOLocalMove(Vector3.zero, .1f);
                });
                AudioManager.Instance.PlaySound(AudioManager.Instance.noWaySound);
            }
            else
            {
                AudioManager.Instance.PlaySound(AudioManager.Instance.wayFindedSound);
                particle.SetActive(true);
            }

        }

    }
}
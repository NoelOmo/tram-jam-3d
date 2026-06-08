using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
namespace NINESOFT.COMPLETEGAMES.CARJAM3D
{
    public class Box : MonoBehaviour
    {
        [System.Serializable]
        public class BoxSlot
        {
            public Transform SlotPoint;
            public Item MyItem;
            public bool IsBusy;
            public bool IsFull => MyItem != null;
        }

        public bool Completed
        {
            get
            {
                bool c = true;
                for (int i = 0; i < Slots.Count; i++)
                {
                    if (!Slots[i].IsFull)
                    {
                        c = false;
                        break;
                    }
                }
                return c;
            }
        }

        public ItemColor Color;

        [Space(20f)]
        [SerializeField] private List<BoxSlot> Slots = new List<BoxSlot>();
        [SerializeField] private MeshRenderer[] meshes;
        [SerializeField] private GameObject particle;
        private void Start()
        {
            foreach (var mesh in meshes)
            {
                mesh.material = VisualManager.Instance.GetMaterialByItemColor(Color);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            foreach (var mesh in meshes)
            {
                mesh.material = VisualManager.Instance.GetMaterialByItemColor(Color);
            }
        }
#endif

        public BoxSlot GetEmptySlot()
        {
            BoxSlot emptySlot = Slots.FirstOrDefault(s => !s.IsFull && !s.IsBusy);
            return emptySlot;
        }

        public void ShowParticle()
        {
            particle.SetActive(true);
            AudioManager.Instance.PlaySound(AudioManager.Instance.boxSound);
        }

    }
}
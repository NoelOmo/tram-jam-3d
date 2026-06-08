using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NINESOFT.COMPLETEGAMES.CARJAM3D
{
    public class VisualManager : Manager<VisualManager>
    {
        [SerializeField] private Material[] ColorMaterials;
        public Material PositiveMat;
        public Material NegativeMat;
        public Material GetMaterialByItemColor(ItemColor itemColor)
        {
            return ColorMaterials[(int)itemColor];
        }
    }

}
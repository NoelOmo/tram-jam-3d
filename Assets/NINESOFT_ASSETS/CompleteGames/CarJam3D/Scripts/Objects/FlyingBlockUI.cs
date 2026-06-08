using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace NINESOFT.COMPLETEGAMES.CARJAM3D
{
    public class FlyingBlockUI : MonoBehaviour
{

    public void Initialize(Vector3 starPos, Vector3 targetPos, bool worldPos = false )
    {
        Vector3 screenPos = starPos;
       if(worldPos) screenPos= Camera.main.WorldToScreenPoint(starPos);
        transform.position = screenPos;
        transform.localScale = Vector3.one;

        transform.DOScale(Vector3.one, .15f);
        transform.DOMove(targetPos, .35f).SetEase(Ease.Linear).OnComplete(() =>
        {
           transform.DOScale(Vector3.one * 1.5f, .15f).OnComplete(() =>
           {
               transform.DOScale(Vector3.one * 1f, .15f).OnComplete(() =>
               {
                   gameObject.SetActive(false);
               });
           });
        });
    }
}
}
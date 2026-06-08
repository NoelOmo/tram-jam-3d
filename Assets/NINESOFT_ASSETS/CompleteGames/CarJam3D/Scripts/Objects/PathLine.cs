using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
namespace NINESOFT.COMPLETEGAMES.CARJAM3D
{
    public class PathLine : MonoBehaviour
    {
        private const float CHECK_RADIUS = .2f;

        [SerializeField] public Transform FirstPoint;
        [SerializeField] public Transform EndPoint;

        public List<PathLine> Parents = new List<PathLine>();
        public List<PathLine> Childs = new List<PathLine>();
        public bool ConnectedToFirstPoint;

        [SerializeField] LineRenderer myLineRenderer;

        private void Awake()
        {
            GameLoop.Instance._paths.Add(this);
        }

        public List<PathLine> GetPath
        {
            get
            {
                if (!ConnectedToFirstPoint) return null;
                var lastPath = new List<PathLine>();
                lastPath.Add(this);

                PathLine curParent = null;
                var parents = Parents;
                while (parents.Count > 0)
                {
                    curParent = parents.FirstOrDefault(p => p.ConnectedToFirstPoint);
                    if (curParent == null) break;
                    lastPath.Add(curParent);
                    parents = curParent.Parents;
                }

                return lastPath;
            }
        }


        public bool PathIsClosed => MyItem != null;
        private Item MyItem;

        private void Start()
        {
            CheckEnd();
            CheckParent();
        }

        //Check First Point
        private void CheckParent()
        {
            Collider[] colls = Physics.OverlapSphere(FirstPoint.position, CHECK_RADIUS);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].CompareTag("Line/End"))
                {
                    Parents.Add(colls[i].GetComponentInParent<PathLine>());
                    Parents[Parents.Count - 1].Childs.Add(this);
                    CheckConnectionStatus();
                }
                else if (colls[i].CompareTag("Area/First"))
                {
                    Invoke(nameof(OpenThePath), .25f);
                }
            }
        }

        public void CheckConnectionStatus()
        {
            StartCoroutine(CheckConnectionStatusEnum());
        }
        private IEnumerator CheckConnectionStatusEnum()
        {
            yield return new WaitForSeconds(.05f);
            bool connected = false;
            for (int i = 0; i < Parents.Count; i++)
            {
                if (Parents[i].ConnectedToFirstPoint && !Parents[i].PathIsClosed)
                {
                    connected = true;
                    break;
                }
            }

            if (!connected)
                for (int i = 0; i < Childs.Count; i++)
                {
                    if (Childs[i].ConnectedToFirstPoint && !Childs[i].PathIsClosed)
                    {
                        connected = true;
                        break;
                    }
                }

            if (connected) { OpenThePath(); }
        }

        private void OpenThePath()
        {
            if (ConnectedToFirstPoint) return;

            myLineRenderer.gameObject.SetActive(true);

            GameObject _follower = new GameObject("follower");
            _follower.transform.parent = transform;
            _follower.transform.localPosition = FirstPoint.transform.localPosition;
            _follower.transform.DOLocalMove(EndPoint.transform.localPosition, 0.2f).SetEase(Ease.Linear).OnUpdate(() =>
            {
                myLineRenderer.SetPosition(0, FirstPoint.transform.localPosition);
                myLineRenderer.SetPosition(1, _follower.transform.localPosition);
            }).OnComplete(() =>
            {
                GameLoop.Instance.ControlAllPathWays();
                Destroy(_follower, .05f);
            });

            ConnectedToFirstPoint = true;

            if (MyItem != null)
            {
                MyItem.transform.DOScale(Vector3.one * 1.25f, .2f).OnComplete(() =>
                {
                    MyItem.transform.DOScale(Vector3.one * 1f, .1f);
                });
            }
        }

        public void PathCleared()
        {
            MyItem = null;

            for (int i = 0; i < Childs.Count; i++)
            {
                Childs[i].CheckConnectionStatus();
                var child = Childs[i];
                while (child.Childs.Count > 0 && !child.PathIsClosed && child.ConnectedToFirstPoint)
                {
                    int c = child.Childs.Count;
                    for (int a = 0; a < c; a++)
                    {
                        child.Childs[a].CheckConnectionStatus();
                        if (a == c - 1) child = child.Childs[c - 1];
                    }
                }
            }

        }

        //Check End Point
        private void CheckEnd()
        {
            Collider[] colls = Physics.OverlapSphere(EndPoint.position, CHECK_RADIUS);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].CompareTag("Item"))
                {
                    MyItem = colls[i].GetComponent<Item>();
                    MyItem.MyPaths.Add(this);
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(FirstPoint.position, CHECK_RADIUS);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(EndPoint.position, CHECK_RADIUS - .05f);

            Gizmos.color = Color.blue;
            Vector3 offset = new Vector3(0, .1f, 0) + (EndPoint.forward * .5f);
            Vector3 startPosition = EndPoint.position;
            Vector3 endPosition = startPosition + (-EndPoint.forward).normalized * 1;
            Gizmos.DrawLine(startPosition + offset, endPosition + offset);

            Vector3 arrowHead1 = endPosition + (Quaternion.Euler(0, 150, 0) * (-EndPoint.forward).normalized * 0.25f * 1);
            Vector3 arrowHead2 = endPosition + (Quaternion.Euler(0, -150, 0) * (-EndPoint.forward).normalized * 0.25f * 1);

            Gizmos.DrawLine(endPosition + offset, arrowHead1 + offset);
            Gizmos.DrawLine(endPosition + offset, arrowHead2 + offset);
            Gizmos.DrawLine(arrowHead1 + offset, arrowHead2 + offset);

        }
#endif
    }
}
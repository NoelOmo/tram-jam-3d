using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
namespace NINESOFT.COMPLETEGAMES.CARJAM3D
{
    public class Slot : MonoBehaviour
    {
        public bool IsFull => MyItem != null;
        public bool IsAvailable { get; private set; } = true;

        public Item MyItem;
        [SerializeField] private GameObject FullItem;
        [SerializeField] private Transform parkingArea;
        [SerializeField] private Color inactiveTint = new Color(0.55f, 0.55f, 0.55f, 1f);
        [SerializeField] [Range(0f, 1f)] private float inactiveTintStrength = 0.65f;

        private GameObject _pSign;
        private GameObject _plusSign;
        private bool _unlockRequested;
        private MeshRenderer[] _renderers;
        private Color[][] _originalColors;

        private void Awake()
        {
            if (parkingArea == null)
                parkingArea = transform.Find("parkingArea");

            ResolveSignObjects();
            CacheRendererColors();
            SetupClickRelays();
        }

        public void SetAvailable(bool available)
        {
            IsAvailable = available;
            ApplyVisualState(available);
            UpdateSignVisuals(available);

            if (available)
                _unlockRequested = false;
        }

        public void SetToMe(Item item)
        {
            if (!IsAvailable) return;

            MyItem = item;
            MyItem.transform.SetParent(transform);
            FullItem.transform.DOLocalMove(new Vector3(0, -0.575f, 1.768f), .25f);
        }

        public void ClearMe()
        {
            MyItem = null;
            FullItem.transform.DOLocalMove(new Vector3(0, -0.215f, 1.768f), .25f);
        }

        public void RequestUnlock()
        {
            if (IsAvailable || _unlockRequested) return;
            if (!GameManager.Instance.IsPlaying()) return;
            if (UIManager.Instance != null && UIManager.Instance.BlocksSlotUnlockInput()) return;

            _unlockRequested = true;
            SlotManager.Instance.TryUnlockSlot(this);
        }

        public void OnUnlockFailed()
        {
            _unlockRequested = false;
        }

        private void CacheRendererColors()
        {
            _renderers = GetComponentsInChildren<MeshRenderer>(true);
            _originalColors = new Color[_renderers.Length][];
            for (int i = 0; i < _renderers.Length; i++)
            {
                var mats = _renderers[i].materials;
                _originalColors[i] = new Color[mats.Length];
                for (int m = 0; m < mats.Length; m++)
                    _originalColors[i][m] = mats[m].color;
            }
        }

        private void ApplyVisualState(bool available)
        {
            if (_renderers == null) return;

            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i] == null) continue;

                var mats = _renderers[i].materials;
                for (int m = 0; m < mats.Length; m++)
                {
                    mats[m].color = available
                        ? _originalColors[i][m]
                        : Color.Lerp(_originalColors[i][m], inactiveTint, inactiveTintStrength);
                }
                _renderers[i].materials = mats;
            }
        }

        private void ResolveSignObjects()
        {
            if (parkingArea == null) return;

            _pSign = parkingArea.Find("Cube.001")?.gameObject;
            _plusSign = parkingArea.Find("Cube.002")?.gameObject;

            if (_pSign == null && FullItem != null)
                _pSign = FullItem;
        }

        private void UpdateSignVisuals(bool available)
        {
            if (_plusSign != null)
            {
                if (_pSign != null)
                    _pSign.SetActive(available);
                _plusSign.SetActive(!available);
                return;
            }

            if (_pSign != null)
                _pSign.SetActive(true);
        }

        private void SetupClickRelays()
        {
            if (parkingArea != null)
            {
                var collider = parkingArea.GetComponent<BoxCollider>();
                if (collider == null)
                {
                    collider = parkingArea.gameObject.AddComponent<BoxCollider>();
                    collider.center = new Vector3(0f, 0f, 0.9f);
                    collider.size = new Vector3(1.1f, 0.3f, 2.2f);
                }
                AttachClickRelay(parkingArea.gameObject);
            }

            var plane = transform.Find("Plane");
            if (plane != null)
                AttachClickRelay(plane.gameObject);

            if (_plusSign != null)
                AttachClickRelay(_plusSign);
        }

        private void AttachClickRelay(GameObject target)
        {
            var relay = target.GetComponent<SlotClickRelay>();
            if (relay == null)
                relay = target.AddComponent<SlotClickRelay>();
            relay.Initialize(this);
        }

    }

    public class SlotClickRelay : MonoBehaviour
    {
        private Slot _slot;

        public void Initialize(Slot slot)
        {
            _slot = slot;
        }

        private void OnMouseDown()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            if (UIManager.Instance != null && UIManager.Instance.BlocksSlotUnlockInput())
                return;

            _slot?.RequestUnlock();
        }
    }
}

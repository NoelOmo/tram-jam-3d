using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
namespace NINESOFT.COMPLETEGAMES.CARJAM3D
{
    public class SlotManager : Manager<SlotManager>
    {
        [SerializeField] private List<Slot> Slots;
        [SerializeField] private int lockedSlotCount = 3;

        private void Start()
        {
            InitializeSlotAvailability();
        }

        private void InitializeSlotAvailability()
        {
            int countToLock = Mathf.Min(lockedSlotCount, Slots.Count);
            int firstLockedIndex = Slots.Count - countToLock;

            for (int i = 0; i < Slots.Count; i++)
                Slots[i].SetAvailable(i < firstLockedIndex);
        }

        public void TryUnlockSlot(Slot slot)
        {
            if (slot == null || slot.IsAvailable) return;

            CompleteGameADManager.Instance.PlayRw(
                onRewardReceived: () =>
                {
                    slot.SetAvailable(true);
                    UIManager.Instance.SlotUnlockActivate(true);
                },
                onFailed: () =>
                {
                    slot.OnUnlockFailed();
                    UIManager.Instance.ShowMessageBox("Ad not available. Try again.");
                }
            );
        }

        public void CheckSlots()
        {
            Box curBox = GameLoop.Instance.GetCurrentLevel.Boxes[0];
            for (int i = 0; i < Slots.Count; i++)
            {
                var slot = Slots[i];
                if (!slot.IsAvailable || slot.MyItem == null) continue;

                if (slot.MyItem.Color == curBox.Color)
                {
                    SetToSlot(slot.MyItem);
                    slot.ClearMe();
                }
            }
        }

        public void SetToSlot(Item item, List<PathLine> path = null, bool sortSlots = true)
        {
            if (!GameManager.Instance.IsPlaying()) return;
            if (path == null) path = new List<PathLine>();

            Item curItem = item;
            curItem.transform.DOKill();
            if (curItem.InBox) return;

            Box currentBox = GameLoop.Instance.GetCurrentLevel.Boxes[0];
            if (currentBox.Color == curItem.Color)
            {
                Box.BoxSlot emptyBoxSlot = currentBox.GetEmptySlot();
                if (emptyBoxSlot != null)
                {
                    emptyBoxSlot.IsBusy = true;
                    curItem.InBox = true;
                    curItem.transform.SetParent(emptyBoxSlot.SlotPoint);
                    Action action = () =>
                    {
                        curItem.transform.DOLocalMove(Vector3.zero, .2f).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            emptyBoxSlot.MyItem = curItem;
                            if (currentBox.Completed)
                            {
                                GameLoop.Instance.NextBox();
                            }
                            if (sortSlots) SortSlots();
                            curItem.transform.DOKill();

                            curItem.transform.localPosition = Vector3.zero;
                            curItem.transform.localScale = Vector3.one * 1f;
                            curItem.transform.DORotate(new Vector3(0, -90, 0), .25f, RotateMode.FastBeyond360);
                        });
                    };

                    if (path.Count > 0)
                    {
                        StartCoroutine(MoveItem(curItem, path, action));
                    }
                    else
                    {
                        action?.Invoke();
                    }

                    return;
                }
            }

            curItem.transform.DOKill();
            Slot emptySlot = Slots.FirstOrDefault(s => s.IsAvailable && !s.IsFull);
            if (emptySlot != null)
            {
                emptySlot.SetToMe(curItem);
                Action action = () =>
                {
                    if (!curItem.InBox)
                        curItem.transform.DOMove(emptySlot.transform.position + new Vector3(0, .25f, 0), .25f).SetEase(Ease.Linear)
                        .OnUpdate(() =>
                        {
                            Vector3 lookPos = curItem.transform.position - emptySlot.transform.position;
                            lookPos.y = 0;
                            Quaternion rot = Quaternion.LookRotation(lookPos);
                            curItem.transform.DORotateQuaternion(rot, .15f).SetEase(Ease.Linear);
                        }).OnComplete(() =>
                        {
                            curItem.transform.DOScale(Vector3.one, .25f).SetEase(Ease.Linear);
                        });
                    if (sortSlots) SortSlots();
                };

                if (path.Count > 0)
                {
                    StartCoroutine(MoveItem(curItem, path, action));
                }
                else
                {
                    action?.Invoke();
                }
            }
            else
            {
                //fail
                GameManager.Instance.Fail();
            }

            if (AllSlotsFull()) Invoke(nameof(CheckFail), .5f);

        }

        private IEnumerator MoveItem(Item item, List<PathLine> path = null, Action onMoveCompleted = null)
        {
            // item.transform.DOKill();
            float moveTime = .2f;
            Item curItem = item;
            if (path != null)
                for (int j = 0; j < path.Count; j++)
                {
                    var targetPos = path[j].FirstPoint.position + new Vector3(0, 0.5f, 0);
                    int jj = j;
                    curItem.transform.DOMove(targetPos, moveTime).SetEase(Ease.Linear).SetDelay(jj * moveTime).OnUpdate(() =>
                    {
                        Vector3 lookPos = curItem.transform.position - targetPos;
                        lookPos.y = 0;
                        Quaternion rot = Quaternion.LookRotation(lookPos);
                        curItem.transform.rotation = Quaternion.Lerp(curItem.transform.rotation, rot, Time.deltaTime * 25f);
                    // curItem.transform.DORotateQuaternion(rot, .15f);
                });
                }

            yield return new WaitForSeconds((path.Count) * moveTime);
            onMoveCompleted?.Invoke();
        }

        private void SortSlots()
        {
            List<Item> items = new List<Item>();
            for (int i = 0; i < Slots.Count; i++)
            {
                if (Slots[i].IsAvailable && Slots[i].MyItem != null)
                {
                    items.Add(Slots[i].MyItem);
                    Slots[i].ClearMe();
                }
            }

            items = items.OrderBy(i => i.Color).ToList();
            for (int i = 0; i < items.Count; i++)
            {
                SetToSlot(items[i], sortSlots: false);
            }
        }

        private bool AllSlotsFull()
        {
            bool allSlotsFull = true;
            for (int i = 0; i < Slots.Count; i++)
            {
                if (Slots[i].IsAvailable && !Slots[i].IsFull)
                {
                    allSlotsFull = false;
                    break;
                }
            }
            return allSlotsFull;
        }

        private void CheckFail()
        {
            if (AllSlotsFull()) GameManager.Instance.Fail();
        }
    }
}

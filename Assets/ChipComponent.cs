using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public class ChipComponent : BaseClickComponent
    {
        void Awake()
        {
            int layer = LayerMask.GetMask("BlackCells");

            Ray ray = new Ray(transform.position, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit, 1f, layer))
            {
                Pair = hit.transform.GetComponent<BaseClickComponent>();
                Pair.Pair = this;
            }
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            Pair._mesh.material.color = new Color32(65, 255, 0, 255);
            CallBackEvent((CellComponent)Pair, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            Pair._mesh.material.color = new Color32(51, 31, 15, 255);
            CallBackEvent((CellComponent)Pair, false);
        }



     











        /*private Transform chip;
        void get(out Transform t)
        {
            t = GetComponent<Transform>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            get(out chip);
            print(chip.transform.name);
        }*/
    }
}

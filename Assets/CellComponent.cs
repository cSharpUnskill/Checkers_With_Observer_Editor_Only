using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

namespace Checkers
{
    public class CellComponent : BaseClickComponent
    {
        private Dictionary<NeighborType, CellComponent> _neighbors = new Dictionary<NeighborType, CellComponent>();

        public CellComponent GetNeighbors1(NeighborType type)
        {
            CellComponent neighbour;
            _neighbors.TryGetValue(type, out neighbour);
            return neighbour;
        }
        void Awake()
        {
            findNeighbours();
        }

        public void findNeighbours()
        {
            if (_color == ColorType.Black)
            {
                int layer = LayerMask.GetMask("BlackCells");

                Ray ray0 = new Ray(transform.position, new Vector3(1, 0, 1));
                Ray ray1 = new Ray(transform.position, new Vector3(-1, 0, 1));
                Ray ray2 = new Ray(transform.position, new Vector3(-1, 0, -1));
                Ray ray3 = new Ray(transform.position, new Vector3(1, 0, -1));  
    
                if (Physics.Raycast(ray0, out RaycastHit hit0, 0.8f, layer)) 
                {
                    _neighbors.Add(NeighborType.TopRight, hit0.transform.GetComponent<CellComponent>());
                }
                if (Physics.Raycast(ray1, out RaycastHit hit1, 0.8f, layer))
                {
                    _neighbors.Add(NeighborType.TopLeft, hit1.transform.GetComponent<CellComponent>());
                }
                if (Physics.Raycast(ray2, out RaycastHit hit2, 0.8f, layer))
                {
                    _neighbors.Add(NeighborType.BottomLeft, hit2.transform.GetComponent<CellComponent>());
                }
                if (Physics.Raycast(ray3, out RaycastHit hit3, 0.8f, layer))
                {
                    _neighbors.Add(NeighborType.BottomRight, hit3.transform.GetComponent<CellComponent>());
                }               
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            _mesh.material.color = new Color32(65, 255, 0, 255);              
            CallBackEvent(this, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if(GetColor == ColorType.Black)
            {
                _mesh.material.color = new Color32(51, 31, 15, 255);
            }
            else if(GetColor == ColorType.White)
            {
                _mesh.material.color = new Color32(165, 166, 73, 255);
            }
           
            CallBackEvent(this, false);
        }

		public void Configuration(Dictionary<NeighborType, CellComponent> neighbors)
		{
            if (_neighbors != null) return;
            _neighbors = neighbors;
		}

	}

    public enum NeighborType : byte
    {
        TopLeft,

        TopRight,

        BottomLeft,

        BottomRight
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Checkers
{
    public class GameManager : MonoBehaviour, IManager
    {
        [SerializeField]
        private Transform Cam;

        [SerializeField]
        private Vector3 whitePos;

        [SerializeField]
        private Vector3 blackPos;

        [SerializeField]
        private ChipComponent[] chips;

        [SerializeField]
        private CellComponent[] cells;

        [SerializeField] 
        private GameObject[] wChips;

        [SerializeField] 
        private GameObject[] bChips;

        private ChipComponent Selected;
        private CellComponent selectedCell1;
        private CellComponent selectedCell2;
        private CellComponent selectedCell1X;
        private CellComponent selectedCell2X;
        private CellComponent selectedCell3;
        private CellComponent selectedCell4;
        private CellComponent selectedCell3X;
        private CellComponent selectedCell4X;

        public ColorType _currentTurnColor = ColorType.White;

        private bool cameraMove;

        public bool isPlaying;

        public event CellInteractionHandler OnCellInteractionHandler;

        void Start()
        {
            print("Ходят Белые");

            foreach (var chip in chips)
            {
                chip.OnClickEventHandler += ManualChipClick;
            }

            foreach (var cell in cells)
            {
                cell.OnClickEventHandler += ManualCellClick;
            }
        }

        void ManualChipClick(BaseClickComponent chip)
        {
            ChipClick((ChipComponent)chip, true);
        }

        void ManualCellClick(BaseClickComponent cell)
        {
            CellClick((CellComponent)cell, true);
        }

        void Update()
        {
            wChips = GameObject.FindGameObjectsWithTag("wChips");
            bChips = GameObject.FindGameObjectsWithTag("bChips");
        }

        void Move(Transform chip, Transform cell)
        {
            StartCoroutine(LerpPosition(chip, cell.transform.position, 0.15f));
        }

        void CamMoveToBlack()
        {
            StartCoroutine(LerpPositionCamTo(Cam, blackPos, 1.5f));
            StartCoroutine(LerpPositionCamToBlackRotation(Cam, 1.5f));
        }

        void CamMoveToWhite()
        {
            StartCoroutine(LerpPositionCamTo(Cam, whitePos, 1.5f));
            StartCoroutine(LerpPositionCamToWhiteRotation(Cam, 1.5f));
        }

        private IEnumerator LerpPositionCamToBlackRotation(Transform obj, float TravelTime)
        {
            yield return new WaitForSeconds(0.5f);

            float t = 0;

            while (t < 1)
            {
                obj.localRotation = Quaternion.Slerp(obj.localRotation, Quaternion.Euler(50, -180, 0f), t * t * 0.1f);

                t += Time.deltaTime / TravelTime;

                yield return null;
            }
            obj.rotation = Quaternion.Euler(50, -180, 0f);
        }

        private IEnumerator LerpPositionCamToWhiteRotation(Transform obj, float TravelTime)
        {
            yield return new WaitForSeconds(0.5f);

            float t = 0;

            while (t < 1)
            {
                obj.localRotation = Quaternion.Slerp(obj.localRotation, Quaternion.Euler(50, -0f, 0f), t * t * 0.1f);

                t += Time.deltaTime / TravelTime;

                yield return null;
            }
            obj.rotation = Quaternion.Euler(50, -0f, 0f);
        }

        private IEnumerator LerpPositionCamTo(Transform obj, Vector3 target, float TravelTime)
        {
            cameraMove = true;
            yield return new WaitForSeconds(0.5f);
            Vector3 startPosition = obj.position;
            float t = 0;

            while (t < 1)
            {
                obj.position = Vector3.Lerp(startPosition, target, t);

                t += Time.deltaTime / TravelTime;

                yield return null;
            }
            obj.position = target;
            cameraMove = false;
        }

        private IEnumerator LerpPosition(Transform obj, Vector3 target, float TravelTime)
        {
            Vector3 startPosition = obj.position;
            float t = 0;

            while (t < 1)
            {
                obj.position = Vector3.Lerp(startPosition, target + new Vector3(0, 0.2f, 0), t * t);
                t += Time.deltaTime / TravelTime;
                yield return null;
            }
            obj.transform.position = target + new Vector3(0, 0.2f, 0);
        }

        private IEnumerator PauseCheck()
        {
            yield return new WaitForSecondsRealtime(0.3f);

            if (wChips.Length == 0)
            {
                print("Черные Победили!!!");
                UnityEditor.EditorApplication.isPaused = true;
            }

            else if (bChips.Length == 0)
            {
                print("Белые Победили!!!");
                UnityEditor.EditorApplication.isPaused = true;
            }

            else if (Selected.GetColor == ColorType.White)
            {
                if (Selected.Pair.name == "B8" || Selected.Pair.name == "D8" || Selected.Pair.name == "F8" || Selected.Pair.name == "H8")
                {
                    print("Белые Победили!!!");
                    UnityEditor.EditorApplication.isPaused = true;
                }
            }

            else if (Selected.GetColor == ColorType.Black)
            {
                if (Selected.Pair.name == "A1" || Selected.Pair.name == "C1" || Selected.Pair.name == "E1" || Selected.Pair.name == "G1")
                {
                    print("Черные Победили!!!");
                    UnityEditor.EditorApplication.isPaused = true;
                }
            }
        }

        void ChangeColor()
        {
            StartCoroutine(PauseCheck());

            if (_currentTurnColor == ColorType.White)
            {
                _currentTurnColor = ColorType.Black;
                print("Ходят Черные");
                CamMoveToBlack();
            }

            else if (_currentTurnColor == ColorType.Black)
            {
                _currentTurnColor = ColorType.White;
                print("Ходят Белые");
                CamMoveToWhite();
            }
        }

        public void CellClick(CellComponent cell, bool manual)
        {
            if (isPlaying == manual) return;

            if (cameraMove == true) return;

            if (Selected != null)
            {
                if (Selected.GetColor == ColorType.White) Selected._mesh.material.color = new Color32(255, 255, 255, 255);
                if (Selected.GetColor == ColorType.Black) Selected._mesh.material.color = new Color32(0, 0, 0, 255);

                if (selectedCell1X != null) selectedCell1X._mesh.material.color = new Color32(51, 31, 15, 255);
                if (selectedCell2X != null) selectedCell2X._mesh.material.color = new Color32(51, 31, 15, 255);
                if (selectedCell3X != null) selectedCell3X._mesh.material.color = new Color32(51, 31, 15, 255);
                if (selectedCell4X != null) selectedCell4X._mesh.material.color = new Color32(51, 31, 15, 255);

                if (selectedCell1 != null) selectedCell1._mesh.material.color = new Color32(51, 31, 15, 255);
                if (selectedCell2 != null) selectedCell2._mesh.material.color = new Color32(51, 31, 15, 255);
                if (selectedCell3 != null) selectedCell3._mesh.material.color = new Color32(51, 31, 15, 255);
                if (selectedCell4 != null) selectedCell4._mesh.material.color = new Color32(51, 31, 15, 255);

                if (Selected.GetColor == ColorType.White && _currentTurnColor == ColorType.White)
                {
                    if (selectedCell1 != null && selectedCell1.Pair == null)
                    {
                        if (cell.transform.position == selectedCell1.transform.position)
                        {
                            Move(Selected.transform, cell.transform);
                            ChangeColor();

                            Selected.Pair.Pair = null;
                            Selected.Pair = cell;
                            cell.Pair = Selected;

                            if (!isPlaying) OnCellInteractionHandler(cell.name, true);
                        }
                    }

                    else if (selectedCell1X != null && selectedCell1X.Pair == null)
                    {
                        if (cell.transform.position == selectedCell1X.transform.position)
                        {
                            if ((selectedCell1.Pair.GetColor == ColorType.White)) return;

                            Move(Selected.transform, cell.transform);
                            ChangeColor();

                            Selected.Pair.Pair = null;
                            Selected.Pair = cell;
                            cell.Pair = Selected;

                            if (selectedCell1.Pair.GetColor == ColorType.Black) { Destroy(selectedCell1.Pair.gameObject); selectedCell1.Pair = null; }

                            if (!isPlaying) OnCellInteractionHandler(cell.name, true);
                        }
                    }

                    if (selectedCell2 != null && selectedCell2.Pair == null)
                    {
                        if (cell.transform.position == selectedCell2.transform.position)
                        {
                            Move(Selected.transform, cell.transform);
                            ChangeColor();

                            Selected.Pair.Pair = null;
                            Selected.Pair = cell;
                            cell.Pair = Selected;

                            if (!isPlaying) OnCellInteractionHandler(cell.name, true);
                        }
                    }
                    else if ((selectedCell2X != null && selectedCell2X.Pair == null))
                    {
                        if (cell.transform.position == selectedCell2X.transform.position)
                        {
                            if ((selectedCell2.Pair.GetColor == ColorType.White)) return;

                            Move(Selected.transform, cell.transform);
                            ChangeColor();

                            Selected.Pair.Pair = null;
                            Selected.Pair = cell;
                            cell.Pair = Selected;

                            if (selectedCell2.Pair.GetColor == ColorType.Black) { Destroy(selectedCell2.Pair.gameObject); selectedCell2.Pair = null; }

                            if (!isPlaying) OnCellInteractionHandler(cell.name, true);
                        }
                    }
                }

                if (Selected.GetColor == ColorType.Black && _currentTurnColor == ColorType.Black)
                {
                    if (selectedCell3 != null && selectedCell3.Pair == null)
                    {
                        if (cell.transform.position == selectedCell3.transform.position)
                        {
                            Move(Selected.transform, cell.transform);
                            ChangeColor();

                            Selected.Pair.Pair = null;
                            Selected.Pair = cell;
                            cell.Pair = Selected;

                            if (!isPlaying) OnCellInteractionHandler(cell.name, true);
                        }
                    }

                    else if (selectedCell3X != null && selectedCell3X.Pair == null)
                    {
                        if (cell.transform.position == selectedCell3X.transform.position)
                        {
                            if ((selectedCell3.Pair.GetColor == ColorType.Black)) return;

                            Move(Selected.transform, cell.transform);
                            ChangeColor();

                            Selected.Pair.Pair = null;
                            Selected.Pair = cell;
                            cell.Pair = Selected;

                            if (selectedCell3.Pair.GetColor == ColorType.White) { Destroy(selectedCell3.Pair.gameObject); selectedCell3.Pair = null; }

                            if (!isPlaying) OnCellInteractionHandler(cell.name, true);
                        }
                    }

                    if (selectedCell4 != null && selectedCell4.Pair == null)
                    {
                        if (cell.transform.position == selectedCell4.transform.position)
                        {
                            Move(Selected.transform, cell.transform);
                            ChangeColor();

                            Selected.Pair.Pair = null;
                            Selected.Pair = cell;
                            cell.Pair = Selected;

                            if (!isPlaying) OnCellInteractionHandler(cell.name, true);
                        }
                    }

                    else if ((selectedCell4X != null && selectedCell4X.Pair == null))
                    {
                        if (cell.transform.position == selectedCell4X.transform.position)
                        {
                            if ((selectedCell4.Pair.GetColor == ColorType.Black)) return;

                            Move(Selected.transform, cell.transform);
                            ChangeColor();

                            Selected.Pair.Pair = null;
                            Selected.Pair = cell;
                            cell.Pair = Selected;

                            if (selectedCell4.Pair.GetColor == ColorType.White) { Destroy(selectedCell4.Pair.gameObject); selectedCell4.Pair = null; }

                            if (!isPlaying) OnCellInteractionHandler(cell.name, true);
                        }
                    }
                }
            }
        }

        public void ChipClick(ChipComponent chip, bool manual)
        {
            if (isPlaying == manual) return;

            if (cameraMove == true) return;

            if (chip.GetColor != _currentTurnColor) return;

            if (!isPlaying) OnCellInteractionHandler.Invoke(chip.Pair.name, false);

            if (Selected != null)
            {
                if (Selected.GetColor == ColorType.Black)
                {
                    Selected._mesh.material.color = Color.black;
                }

                else if (Selected.GetColor == ColorType.White)
                {
                    Selected._mesh.material.color = Color.white;
                }
            }

            Selected = chip;

            Selected._mesh.material.color = new Color32(65, 255, 0, 255);


            var topLeft = ((CellComponent)chip.Pair).GetNeighbors1(NeighborType.TopLeft);

            var topRight = ((CellComponent)chip.Pair).GetNeighbors1(NeighborType.TopRight);

            var botLeft = ((CellComponent)chip.Pair).GetNeighbors1(NeighborType.BottomLeft);

            var botRight = ((CellComponent)chip.Pair).GetNeighbors1(NeighborType.BottomRight);

            //WHITE CHIPS//

            if (chip.GetColor == ColorType.White)
            {
                if (selectedCell1X != null) selectedCell1X._mesh.material.color = new Color32(51, 31, 15, 255);
                if (selectedCell2X != null) selectedCell2X._mesh.material.color = new Color32(51, 31, 15, 255);
                if (selectedCell3X != null) selectedCell3X._mesh.material.color = new Color32(51, 31, 15, 255);
                if (selectedCell4X != null) selectedCell4X._mesh.material.color = new Color32(51, 31, 15, 255);

                if (selectedCell1 != null) selectedCell1._mesh.material.color = new Color32(51, 31, 15, 255);
                if (selectedCell2 != null) selectedCell2._mesh.material.color = new Color32(51, 31, 15, 255);
                if (selectedCell3 != null) selectedCell3._mesh.material.color = new Color32(51, 31, 15, 255);
                if (selectedCell4 != null) selectedCell4._mesh.material.color = new Color32(51, 31, 15, 255);

                selectedCell1 = topLeft;
                if (selectedCell1 != null) selectedCell1X = topLeft.GetNeighbors1(NeighborType.TopLeft);
                selectedCell2 = topRight;
                if (selectedCell2 != null) selectedCell2X = topRight.GetNeighbors1(NeighborType.TopRight);

                if (selectedCell1 != null)
                {
                    selectedCell1._mesh.material.color = new Color32(51, 31, 15, 255);
                }

                if (selectedCell2 != null)
                {
                    selectedCell2._mesh.material.color = new Color32(51, 31, 15, 255);
                }

                if (topLeft != null && topLeft.Pair == null)
                {
                    selectedCell1._mesh.material.color = new Color32(129, 195, 104, 255);
                }
                if (topRight != null && topRight.Pair == null)
                {
                    selectedCell2._mesh.material.color = new Color32(129, 195, 104, 255);
                }

                if (topLeft != null && topLeft.Pair != null)
                {
                    if (topLeft.Pair.GetColor == ColorType.Black)
                    {
                        if (selectedCell1X != null)
                        {
                            if (selectedCell1X.Pair == null) selectedCell1X._mesh.material.color = new Color32(129, 195, 104, 255);
                        }
                    }
                }

                if (topRight != null && topRight.Pair != null)
                {
                    if (topRight.Pair.GetColor == ColorType.Black)
                    {
                        if (selectedCell2X != null)
                        {
                            if (selectedCell2X.Pair == null) selectedCell2X._mesh.material.color = new Color32(129, 195, 104, 255);
                        }
                    }
                }
            }

            //BLACK CHIPS//

            if (chip.GetColor == ColorType.Black)
            {
                if (selectedCell1X != null) selectedCell1X._mesh.material.color = new Color32(51, 31, 15, 255);
                if (selectedCell2X != null) selectedCell2X._mesh.material.color = new Color32(51, 31, 15, 255);
                if (selectedCell3X != null) selectedCell3X._mesh.material.color = new Color32(51, 31, 15, 255);
                if (selectedCell4X != null) selectedCell4X._mesh.material.color = new Color32(51, 31, 15, 255);

                if (selectedCell1 != null) selectedCell1._mesh.material.color = new Color32(51, 31, 15, 255);
                if (selectedCell2 != null) selectedCell2._mesh.material.color = new Color32(51, 31, 15, 255);
                if (selectedCell3 != null) selectedCell3._mesh.material.color = new Color32(51, 31, 15, 255);
                if (selectedCell4 != null) selectedCell4._mesh.material.color = new Color32(51, 31, 15, 255);

                selectedCell3 = botLeft;
                if (selectedCell3 != null) selectedCell3X = botLeft.GetNeighbors1(NeighborType.BottomLeft);
                selectedCell4 = botRight;
                if (selectedCell4 != null) selectedCell4X = botRight.GetNeighbors1(NeighborType.BottomRight);

                if (selectedCell3 != null)
                {
                    selectedCell3._mesh.material.color = new Color32(51, 31, 15, 255);
                }

                if (selectedCell4 != null)
                {
                    selectedCell4._mesh.material.color = new Color32(51, 31, 15, 255);
                }

                if (botLeft != null && botLeft.Pair == null)
                {
                    selectedCell3._mesh.material.color = new Color32(129, 195, 104, 255);
                }

                if (botRight != null && botRight.Pair == null)
                {
                    selectedCell4._mesh.material.color = new Color32(129, 195, 104, 255);
                }

                if (botLeft != null && botLeft.Pair != null)
                {
                    if (botLeft.Pair.GetColor == ColorType.White)
                    {
                        if (selectedCell3X != null)
                        {
                            if (selectedCell3X.Pair == null) selectedCell3X._mesh.material.color = new Color32(129, 195, 104, 255);
                        }
                    }
                }

                if (botRight != null && botRight.Pair != null)
                {
                    if (botRight.Pair.GetColor == ColorType.White)
                    {
                        if (selectedCell4X != null)
                        {
                            if (selectedCell4X.Pair == null) selectedCell4X._mesh.material.color = new Color32(129, 195, 104, 255);
                        }
                    }
                }
            }
        }

        public void SetPlaying(bool replay)
        {
            isPlaying = replay;
        }

        public CellComponent[] GetCells() { return cells; }
    }

    public interface IManager
    {
        event CellInteractionHandler OnCellInteractionHandler;

        void CellClick(CellComponent cell, bool manual);

        void ChipClick(ChipComponent chip, bool manual);

        CellComponent[] GetCells();

        void SetPlaying(bool replay);
    }

    public delegate void CellInteractionHandler(string cellName, bool turn);
}

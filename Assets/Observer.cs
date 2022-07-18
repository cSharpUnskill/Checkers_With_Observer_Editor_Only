using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

namespace Checkers
{
    public class Observer : MonoBehaviour
    {
        private IManager _gameManager;

        [SerializeField]
        private bool _replay;

        private string _fileName = "Record";

        private string _address;

        private string _select = "select";

        private string _turn = "turn";

        private Dictionary<string, CellComponent> cellsByName = new Dictionary<string, CellComponent>();

        StreamWriter writer;

        void Start()
        {
            _gameManager = GetComponent<IManager>();

            _address = Environment.CurrentDirectory + @"\" + _fileName + ".txt";

            _gameManager.OnCellInteractionHandler += Write;

            _gameManager.SetPlaying(_replay);

            foreach (var cell in _gameManager.GetCells())
            {
                cellsByName.Add(cell.name, cell);
            }

            if(_replay) StartCoroutine(Replay());
        }

    
        void OnDestroy()
        {
            if(writer != null) writer.Close();
        }

        void Write(string cellName, bool turn)
        {
            if (writer == null)
            {
                writer = new StreamWriter(_address);
            }
            var code = turn ? _turn : _select;

            var line = $"{code}_{cellName}_";

            writer.WriteLine(line);
        }

        List<string> Read()
        {
            var fileText = string.Empty;

            StreamReader reader = new StreamReader(_address);

            fileText = reader.ReadToEnd();

            reader.Close();

            var lines = fileText.Split(new char[] { '\n' }).ToList();

            lines.Remove("");

            return lines;
        }

        private IEnumerator Replay()
        {
            yield return null;

            var turns = Read();

            if (turns.Count == 0) yield break;

            foreach (var turn in turns)
            {
                var currentTurnInfo = turn.Split('_');

                if (currentTurnInfo[0] == _turn)
                {
                    _gameManager.CellClick(cellsByName[currentTurnInfo[1]], false);
                }

                else if(currentTurnInfo[0] == _select)
                {
                    _gameManager.ChipClick((ChipComponent)cellsByName[currentTurnInfo[1]].Pair, false);
                }

                yield return new WaitForSeconds(currentTurnInfo[0] == _turn? 2.2f : .5f);
            }
        }
    }
}


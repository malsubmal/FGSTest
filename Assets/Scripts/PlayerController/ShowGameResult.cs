using FGSTest.Payload;
using SuperMaxim.Messaging;
using TMPro;
using UnityEngine;

namespace FGSTest.PlayerController
{
    public class ShowGameResult : MonoBehaviour
    {
        [SerializeField] GameObject _gameResultObj;
        [SerializeField] TMP_Text _resultText;

        private void Start()
        {
            Messenger.Default.Subscribe<GameResultPayload>(OnGameResult);
        }
        
        private void OnGameResult(GameResultPayload payload)
        {
            _resultText.text = payload.IsWin ? "Win" : "Lose";
            _gameResultObj.SetActive(true);
        }
        
        private void OnDestroy()
        {
            Messenger.Default.Unsubscribe<GameResultPayload>(OnGameResult);
        }
    }
}
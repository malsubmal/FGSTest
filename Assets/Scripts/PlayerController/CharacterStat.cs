using System.Collections.Generic;
using FGSTest.Payload;
using SuperMaxim.Messaging;
using UnityEngine;

namespace FGSTest.PlayerController
{
    public class CharacterStat : MonoBehaviour
    {
        [SerializeField] private List<CharacterStatValue> _initialStatType;
        
        private Dictionary<CharacterStatType, float> _currentStatType;
        
        private void Awake()
        {
            InitDictionary();
        }
        
        private void InitDictionary()
        {
            _currentStatType = new Dictionary<CharacterStatType, float>();
            foreach (var statType in _initialStatType)
            {
                _currentStatType.Add(statType.StatType, statType.Value);
            }
        }

        public void RequestStatModification(CharacterStatType statType, float changeValue)
        {
            if (_currentStatType is null)
                InitDictionary();

            if (_currentStatType is null)
            {
                Debug.LogError("Current stat type is null");
                return;
            }
            
            var hasStatType = _currentStatType.ContainsKey(statType);

            if (!hasStatType)
            {
                _currentStatType.Add(statType, changeValue);
                return;
            }
            
            _currentStatType[statType] += changeValue;
            
#if UNITY_EDITOR
            Debug.Log($"Stat type {statType} changed to {_currentStatType[statType]}");
#endif
            
            Messenger.Default.Publish(new CharacterStatUpdatePayload(statType, _currentStatType[statType]));
        }
        
    }
}
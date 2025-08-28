using System;
using System.Collections.Generic;
using Animancer;
using UnityEngine;

namespace FGSTest.PlayerController
{
    public class CharacterAnimationHandler : MonoBehaviour
    {
        [SerializeField] List<AnimationKeyValue> _animationList;
        [SerializeField] AnimancerComponent _animancer;
        
        private Dictionary<AnimationKey, TransitionAsset> _clipTable;

        private void Awake()
        {
            InitDictionary();
        }

        private void InitDictionary()
        {
            _clipTable = new Dictionary<AnimationKey, TransitionAsset>();
            foreach (var keyValue in _animationList)
            {
                _clipTable.Add(keyValue.Key, keyValue.Clip);
            }
        }

        public void PlayAnimation(AnimationKey key)
        {
            if (_clipTable is null)
                InitDictionary();
            
            if (_clipTable is null)
                return;
            
            if (_clipTable.TryGetValue(key, out var clip))
            {
                _animancer.Play(clip);
            }
        }
    }
    
    [Serializable]
    public struct AnimationKeyValue
    {
        public AnimationKey Key;
        public TransitionAsset Clip;
    }

    [Serializable]
    public enum AnimationKey
    {
        Idle = 0,
        Jump = 1,
        Landing = 2,
        Walk = 3,
        Run = 4,
    }
}
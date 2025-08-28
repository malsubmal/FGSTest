using System;
using System.ComponentModel;
using FGSTest.Payload;
using SuperMaxim.Messaging;
using UnityEngine;

namespace FGSTest.PlayerController
{
    public class GravityAction : CharacterAction
    {
        //use constant force for now
        [Header("Gravity Config")]
        [SerializeField] private float _fallSpeed;
        [SerializeField] private CharacterController _characterController;
        
        protected override void UpdateAction()
        {
            _characterController.Move(_fallSpeed * Time.deltaTime * Vector3.down);
        }
    }
}


using System;
using UnityEngine;

namespace Thrash.Lab5
{
    // interface for prey and predators
    public interface ICreature

    {

        public Rigidbody body { get; }

        public RaycastVision vision { get; }

        public Transform trans { get; }

        public float Speed { get; }

        public float TopSpeed { get; }

        public float AvoidAngle { get; }

        public float TurnSpeed { get; }

        public IState State { get; }
   

        void FixedUpdate();

        void SetState(IState state);



    }
}
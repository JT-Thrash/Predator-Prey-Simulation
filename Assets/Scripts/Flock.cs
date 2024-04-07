using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thrash.Lab5
{
    public class Flock : IState
    {

        private new float MoveSpeed => creature.Speed * 0.9f;

        public Flock(ICreature creature, params (Transform obj, Vector3 hitpoint)[] target)
        {

            
            body = creature.body;
            trans = creature.trans;
            vision = creature.vision;
            base.creature = creature;

            // check if first object in sight is flock member, else follow second object
            if (target.Length == 1) InitializeRotations(target[0].obj);
            else if (!target[0].obj.CompareTag("Prey")) InitializeRotations(target[1].obj);

            t = 0;
        }

        private void InitializeRotations(Transform target)
        {
            Following = target;
            desiredDirection = target.forward;
            desiredRotation = Quaternion.LookRotation(desiredDirection, Vector3.up);
            startRotation = trans.rotation;
        }


        // Update is called once per frame
        public override void Update()
        {

            t += TurnSpeed * Time.deltaTime;
            Turn(desiredRotation);
            body.velocity = trans.forward * MoveSpeed;


            bool predatorInSight = vision.InSight("Predator");
            bool wallInSight = vision.InSight("Wall");

            if (t >= 1 || predatorInSight || wallInSight)
                CheckSurroundings(false);

        }



    }
}
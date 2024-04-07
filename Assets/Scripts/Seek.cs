using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thrash.Lab5
{
    public class Seek : IState
    {

        private new float MoveSpeed => creature.Speed * 1.5f;

        public Seek(ICreature creature, params (Transform obj, Vector3 hitpoint)[] target)
        {
            body = creature.body;
            trans = creature.trans;
            vision = creature.vision;
            base.creature = creature;

            // check if first object in sight is prey, else seek second object
            if (target.Length == 1) InitializeRotations(target[0].hitpoint);
            else if (!target[0].obj.CompareTag("Prey")) InitializeRotations(target[1].hitpoint);

            t = 0;
        }

        private void InitializeRotations(Vector3 target)
        {
            desiredDirection = target - trans.position;
            desiredRotation = Quaternion.LookRotation(desiredDirection, Vector3.up);
            startRotation = trans.rotation;
        }



        // Update is called once per frame
        public override void Update()
        {

            t += TurnSpeed * Time.deltaTime;
            Turn(desiredRotation);
            body.velocity = trans.forward * MoveSpeed;

            if (t >= 1) CheckSurroundings(false);

        }



    }
}
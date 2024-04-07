using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thrash.Lab5
{


    public class Flee : IState
    {


        public Flee(ICreature creature, params (Transform obj, Vector3 hitpoint)[] target)
        {

            body = creature.body;
            trans = creature.trans;
            vision = creature.vision;

            base.creature = creature;


            // check if first object in sight is predator, else flee from second object
            if (target.Length == 1) InitializeRotations(target[0].hitpoint);
            else InitializeRotations(target[0].hitpoint, target[1].hitpoint);


            t = 0;
        }

        private void InitializeRotations(params Vector3[] obj_locations)
        {

            startRotation = trans.rotation;

            if (obj_locations.Length == 1)
            {
                desiredDirection = -1 * (obj_locations[0] - trans.position);
            }
            else
            {
                Vector3 dir1 = obj_locations[0] - trans.position;
                Vector3 dir2 = obj_locations[1] - trans.position;

                desiredDirection = -1 * (dir1 + dir2) / 2f;
            }

            desiredRotation = Quaternion.LookRotation(desiredDirection, Vector3.up);
        }

        // Turn away from predator while moving at top speed.
        // After turning, continue moving at top speed until running into something
        public override void Update()
        {

            t += TurnSpeed * Time.deltaTime;
            Turn(desiredRotation);
            body.velocity = trans.forward * creature.TopSpeed;

            if (t >= 1) CheckSurroundings(true);

            Debug.DrawLine(trans.position, trans.position + desiredDirection * 5, Color.blue);

        }



    }
}
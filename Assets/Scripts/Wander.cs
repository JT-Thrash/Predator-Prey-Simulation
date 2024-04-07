using UnityEngine;

namespace Thrash.Lab5
{
    public class Wander : IState
    {
        private new float TurnSpeed => creature.TurnSpeed / 4f;

        public Wander(ICreature c)
        {
            body = c.body;
            trans = c.trans;
            vision = c.vision;
            creature = c;

            creatureType = c is Prey ? "Prey" : "Predator";

            t = 1;
        }

        // Update is called once per frame
        public override void Update()
        {
            CheckSurroundings(false);


            bool finishedTurning = t >= 1;
            bool changeDirection = Random.Range(0f, 1f) > 0.5f && finishedTurning;


            if (changeDirection)
            {
                float newAngle = Choose(-45, 45);
                Vector3 newDirection = Quaternion.AngleAxis(newAngle, Vector3.up) * trans.forward;

                if (!vision.PathBlocked(trans.position, newDirection))
                {
                    desiredDirection = newDirection;
                    desiredRotation = Quaternion.LookRotation(desiredDirection, Vector3.up);
                    startRotation = trans.rotation;
                    t = 0;
                }
            }


            if (!finishedTurning) t += TurnSpeed * Time.deltaTime;
            Turn(desiredRotation);

            body.angularVelocity = Vector3.zero;
            body.velocity = trans.forward * MoveSpeed;

        }


        private int Choose(int a, int b)
        {
            int rand = Random.Range(0, 2);
            return rand == 0 ? a : b;
        }


    }
}
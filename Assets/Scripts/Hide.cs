using UnityEngine;

namespace Thrash.Lab5
{
    public class Hide : IState
    {

        private bool enteredNest;

        public Hide(ICreature c, bool enteredNest)
        {
            body = c.body;
            trans = c.trans;
            vision = c.vision;
            creature = c;

            this.enteredNest = enteredNest;

            creatureType = c is Prey ? "Prey" : "Predator";

            t = 1;
        }

        public override void Update()
        {
            if (!enteredNest) // head straight toward nest
            {
                body.angularVelocity = Vector3.zero;
                body.velocity = trans.forward * MoveSpeed;
            }
            else // stop moving once in nest
            {
                body.angularVelocity = Vector3.zero;
                body.velocity = Vector3.zero;
            }

        }



    }
}
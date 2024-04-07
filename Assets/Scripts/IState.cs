using System;
using UnityEngine;

namespace Thrash.Lab5
{
    public abstract class IState : Debuggable
    {
        protected Rigidbody body;
        protected Transform trans;
        protected RaycastVision vision;

        protected Transform NearestObj => vision.Nearest.obj;
        protected Transform SecondNearestObj => vision.Second.obj;
        protected Vector3 NearestHitPoint => vision.Nearest.obj_location;
        protected Vector3 SecondNearestHitPoint => vision.Second.obj_location;

        protected Quaternion desiredRotation, startRotation;

        protected Vector3 desiredDirection;

        protected ICreature creature;

        public Transform Following { get; protected set; }

        protected float AvoidAngle => creature.AvoidAngle;
        protected float MoveSpeed => creature.Speed;
        protected float TurnSpeed => creature.TurnSpeed;

        protected string creatureType;

        protected float t; // used for Vector3 Lerp

        public bool FinishedTurning => t >= 1;


        protected void CheckSurroundings(bool fleeing)
        {
            (Transform, Vector3) nearby1 = vision.Nearest;
            (Transform, Vector3) nearby2 = vision.Second;
            Transform obj1 = nearby1.Item1, obj2 = nearby2.Item1;
            Vector3 hitpoint1 = nearby1.Item2, hitpoint2 = nearby2.Item2;

            float angle1 = Vector3.Angle(hitpoint1 - trans.position, trans.forward);
            float angle2 = Vector3.Angle(hitpoint2 - trans.position, trans.forward);

            float dist1 = Vector3.Distance(hitpoint1, trans.position);
            float dist2 = Vector3.Distance(hitpoint2, trans.position);

            if (obj1 != null && obj2 != null)
            {
                float[] angles = new float[] { angle1, angle2 };
                float[] distances = new float[] { dist1, dist2 };
                HandleTwoObjectsNearby(angles, distances, nearby1, nearby2);

            }
            else if (obj1 != null)
            {
                HandleOneObjectNearby(nearby1, angle1, dist1);
            }
            else if (obj2 != null)
            {
                HandleOneObjectNearby(nearby2, angle2, dist2);
            }
            else if (!(this is Wander) && !fleeing)
            {
                creature.SetState(new Wander(creature));
            }
        }

        private void HandleOneObjectNearby((Transform, Vector3) nearby, float angle, float dist)
        {
            Transform obj = nearby.Item1;

            bool avoid = InTheWay(obj, angle, dist);
            bool target = IsTarget(obj);
            bool flock = IsFlock(obj);

            if (obj.CompareTag("Predator") && creature is Prey)
                creature.SetState(new Flee(creature, nearby));

            else if (target)
                creature.SetState(new Seek(creature, nearby));

            else if (avoid)
                creature.SetState(new Avoid(creature, nearby));

            else if (flock)
                creature.SetState(new Flock(creature, nearby));

            else if (!(this is Wander))
                creature.SetState(new Wander(creature));
        }

        private void HandleTwoObjectsNearby(float[] angle, float[] dist, params (Transform, Vector3)[] nearby)
        {
            Transform obj1 = nearby[0].Item1, obj2 = nearby[1].Item1;


            bool avoid1 = InTheWay(obj1, angle[0], dist[0]);
            bool avoid2 = InTheWay(obj2, angle[1], dist[1]);

            bool target1 = IsTarget(obj1);
            bool target2 = IsTarget(obj2);

            bool flock1 = IsFlock(obj1);
            bool flock2 = IsFlock(obj2);


            if ((obj1.CompareTag("Predator") || obj2.CompareTag("Predator")) && creature is Prey)
                creature.SetState(new Flee(creature, nearby));

            else if (target1 || target2)
                creature.SetState(new Seek(creature, nearby));

            else if (avoid1 && avoid2)
                creature.SetState(new Avoid(creature, nearby[0], nearby[1]));

            else if (avoid1)
                creature.SetState(new Avoid(creature, nearby[0]));

            else if (avoid2)
                creature.SetState(new Avoid(creature, nearby[1]));

            else if (flock1 || flock2)
                creature.SetState(new Flock(creature, nearby));

            else if (!(this is Wander))
                creature.SetState(new Wander(creature));
        }

        private bool IsTarget(Transform obj)
        {
            return (obj.CompareTag("Prey") && creature is Predator)
                || (obj.CompareTag("Nest") && creature is Prey && !(this is Hide));
        }

        private bool InTheWay(Transform obj, float angle, float distance)
        {
            return (obj.CompareTag("Wall")
                || (obj.tag == creatureType && creature is Predator)
                || (obj.CompareTag("Nest") && creature is Predator)
                || (obj.CompareTag("Nest") && creature is Prey && this is Hide))
                && ((angle <= AvoidAngle && distance < 4) || distance < 1);
        }

        private bool IsFlock(Transform obj)
        {
            ICreature other = obj.GetComponent<Prey>();
            bool followingMe = other != null && other.State.Following == trans;
            return obj.CompareTag("Prey") && creature is Prey
                && !(this is Hide) && !followingMe;
        }

        protected void Turn(Quaternion newRotation)
        {
            if (ValidRotation(newRotation) && t <= 1)
                trans.rotation = Quaternion.Lerp(startRotation, newRotation, t);
        }

        private bool ValidRotation(Quaternion q)
        {
            bool check = q.x != 0f
                || q.y != 0f
                || q.z != 0f
                || q.w != 0f;

            return check;
        }
        abstract public void Update();
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thrash.Lab5
{

    public class Avoid : IState
    {
        private Vector3 obstacleDirection;

        private Transform initialNearest, initialSecondNearest;

        public Avoid(ICreature creature, params (Transform obj, Vector3 hitpoint)[] obstacles)
        {
            body = creature.body;
            trans = creature.trans;
            vision = creature.vision;
            this.creature = creature;

            InitializeRotations(obstacles);

            initialNearest = obstacles[0].obj;
            if (obstacles.Length > 1)
                initialSecondNearest = obstacles[1].obj;

            t = 0;
        }

        private void InitializeRotations(params (Transform obj, Vector3 hitpoint)[] obstacles)
        {
            desiredDirection = FindDirection(obstacles);
            desiredRotation = Quaternion.LookRotation(desiredDirection, Vector3.up);
            startRotation = trans.rotation;
        }

        // Turn towards best path, while checking for nearby predators or prey
        // Check surroundings again once finished turning to determine next state
        public override void Update()
        {
            t += TurnSpeed * Time.deltaTime;
            Turn(desiredRotation);
            body.velocity = trans.forward * MoveSpeed;

            bool predatorInSight = creature is Prey && vision.InSight("Predator");

            bool preyInSight = creature is Predator && vision.InSight("Prey");

            bool newObstacle = initialSecondNearest == null && SecondNearestObj != null
                || NearestObj != initialNearest;

            if (t >= 1 || predatorInSight || preyInSight || newObstacle)
                CheckSurroundings(false);


            Debug.DrawLine(trans.position, trans.position + desiredDirection * 5, Color.blue);

        }

        // Find best path for avoiding the obstacles 
        private Vector3 FindDirection(params (Transform obj, Vector3 position)[] obstacles)
        {
            if (obstacles.Length == 1)
            {
                obstacleDirection = obstacles[0].position - trans.position;
                float targetAngle = Vector3.Angle(obstacleDirection, trans.forward);

                float rightAmount = -Vector3.Dot(obstacleDirection, trans.right);

                if (targetAngle == 0)
                    rightAmount = 1;

                float mult = Mathf.Sign(rightAmount);

                Vector3 newDir = Quaternion.AngleAxis(mult * 91, Vector3.up) * obstacleDirection;

                return newDir;
            }


            Vector3 dir1 = obstacles[0].position - trans.position;
            Vector3 dir2 = obstacles[1].position - trans.position;

            return -1 * (dir1 + dir2) / 2f;

        }

    }
}
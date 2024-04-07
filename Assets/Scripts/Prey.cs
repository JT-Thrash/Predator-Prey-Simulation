using System.Collections;
using UnityEngine;


namespace Thrash.Lab5
{
    [RequireComponent(typeof(RaycastVision))]
    [RequireComponent(typeof(Rigidbody))]
    public class Prey : MonoBehaviour, ICreature
    {

        public RaycastVision vision { get; private set; }

        public Rigidbody body { get; private set; }

        public Transform trans => transform;

        [SerializeField] private float speed = 5, turnSpeed = 5, fov, visualRange;

        private const float avoidAngle = 60;

        //prey normally much slower than top speed
        public float Speed => speed / 3;
        public float TopSpeed => speed;
        public float TurnSpeed => turnSpeed;
        public float AvoidAngle => avoidAngle;
        
        private IState state;
        public IState State => state;

        private bool backingUp = false;

        // Start is called before the first frame update
        void Awake()
        {
            vision = GetComponent<RaycastVision>();
            vision.Initialize(fov, visualRange, Color.blue);
            body = GetComponent<Rigidbody>();
            state = new Wander(this);
        }

        // Update is called once per frame
        public void FixedUpdate()
        {
            state.Update();
        }

        public void SetState(IState newState)
        {
            state = newState;
        }

        public void Die()
        {
            state = null;
            Destroy(gameObject);
        }


        private void OnTriggerExit(Collider other)
        {
            if (other.transform.CompareTag("Nest"))
            {
                StartCoroutine(wait());
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("Nest"))
            {
                SetState(new Hide(this, false));
            }
        }

        // Prey backs away from other prey they run into
        private void OnCollisionEnter(Collision collision)
        {
            
            if(collision.transform.tag == "Prey")
            {
                Vector3 direction = collision.transform.position - trans.position;
                float inFront = Vector3.Dot(direction, trans.forward);

                if(inFront > 0 && !backingUp) StartCoroutine(BackAwayFromFlock());
            }
        }

        IEnumerator wait()
        {
            yield return new WaitForSeconds(1);
            SetState(new Hide(this, true));
            yield return null;
        }

        IEnumerator BackAwayFromFlock()
        {
            backingUp = true;

            var temp = speed;
            speed = -1 * (speed / 2f);

            yield return new WaitForSeconds(.5f);
            speed = temp;

            backingUp = false;
            yield return null;
        }
    }

}
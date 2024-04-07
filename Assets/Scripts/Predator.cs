using UnityEngine;

namespace Thrash.Lab5
{
    [RequireComponent(typeof(RaycastVision))]
    [RequireComponent(typeof(Rigidbody))]
    public class Predator : MonoBehaviour, ICreature
    {

        public RaycastVision vision { get; private set; }

        public Rigidbody body { get; private set; }

        public Transform trans => transform;

        [SerializeField] private float speed = 5, turnSpeed = 5, fov, visualRange;

        private const float avoidAngle = 60;

        //predator normally moves around half their top speed
        public float Speed => speed / 2;
        public float TopSpeed => speed;
        public float TurnSpeed => turnSpeed;
        public float AvoidAngle => avoidAngle;
        
        private IState state;
        public IState State => state;

        // Start is called before the first frame update
        void Awake()
        {
            vision = GetComponent<RaycastVision>();
            vision.Initialize(fov, visualRange, Color.red);
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

        private void OnCollisionEnter(Collision other)
        {
            if (other.transform.CompareTag("Prey"))
            {
                other.transform.GetComponent<Prey>().Die();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("Nest"))
            {
                Die();
            }
        }


    }

}
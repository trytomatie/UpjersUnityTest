using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace PlantingGame
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    public class WorkerAI : MonoBehaviour
    {
        public WorkerJobType jobType;
        private Animator _animator;
        private NavMeshAgent _agent;
        private WorkerJobManager _workerJobManager;
        private float workingTime = 5.5f;
        private bool _isWorking = false;
        private Vector3 _smoothedDeltaPosition;
        public WorkerJob _currentJob;

        // Animator Hashes
        private int _workAnimatorHash = Animator.StringToHash("Work");
        private int _locomotionAnimatorHash = Animator.StringToHash("Locomotion");

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _animator = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();
            _workerJobManager = FindFirstObjectByType<WorkerJobManager>();
            _workerJobManager.AddWorker(this);
            _agent.updatePosition = false;
        }

        private void OnAnimatorMove()
        {
            _animator.applyRootMotion = true;
            Vector3 rootPosition = _animator.rootPosition;
            transform.position = rootPosition;
            _agent.nextPosition = rootPosition;
        }
        

        private void Update()
        {
            if (_currentJob != null && _agent.remainingDistance <= _agent.stoppingDistance && !_isWorking)
            {
                StopCoroutine(TurnToTargetDestination());
                StartCoroutine(TurnToTargetDestination());
                _animator.SetTrigger(_workAnimatorHash);
                _isWorking = true;
                Invoke("FinishWork", WorkingTime);
            }
            Animation();
        }

        /// <summary>
        /// Finishes the work, Invoked after the working time is over.
        /// </summary>
        private void FinishWork()
        {
            _currentJob.crop.AdvanceGrowthCycle();
            _currentJob = null;
            _isWorking = false;
            transform.position = new Vector3(transform.position.x, 0, transform.position.z); // Set Y position to 0 because of rootmotion drift
        }
        /// <summary>
        /// Assigns job to worker
        /// </summary>
        /// <param name="job"></param>
        /// <returns>false if worker already has a job</returns>
        public bool AssignJob(WorkerJob job)
        {
            if(_currentJob == null)
            {
                _currentJob = job;
                _agent.SetDestination(job.crop.transform.position);
                return true;
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// Animation Logic for workerAi.
        /// </summary>
        private void Animation()
        {
            Vector3 worldDeltaPosition = _agent.nextPosition - _agent.transform.position;
            worldDeltaPosition.y = 0;

            float dx = Vector3.Dot(transform.right, worldDeltaPosition);
            float dz = Vector3.Dot(transform.forward, worldDeltaPosition);
            Vector3 localDeltaPosition = new Vector3(dx, 0, dz);

            _smoothedDeltaPosition = Vector3.Lerp(_smoothedDeltaPosition, localDeltaPosition, 0.2f);

            Vector3 velocity = _smoothedDeltaPosition / Time.deltaTime;

            _animator.SetFloat(_locomotionAnimatorHash, velocity.magnitude, 0.1f, Time.deltaTime);
            //_animator.SetFloat("Dx", dx);
            //_animator.SetFloat("Dz", dz);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator TurnToTargetDestination()
        {
            Vector3 targetPosition = _agent.destination;
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 90);
                yield return null;
            }
        }

        private void OnDestroy()
        {
            _workerJobManager.RemoveWorker(this);
            
        }

        public float WorkingTime { get => workingTime; set => workingTime = value; }
    }
}


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
        private float workingTime = 8.5f;
        private bool _isWorking = false;
        private Vector3 _smoothedDeltaPosition;
        public WorkerJob _currentJob;
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

                _animator.SetTrigger("Work");
                _isWorking = true;
                Invoke("FinishWork", workingTime);
            }
            Animation();
        }

        private void FinishWork()
        {
            _currentJob.crop.AdvanceGrowthCycle();
            _currentJob = null;
            _isWorking = false;
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

        private void Animation()
        {
            Vector3 worldDeltaPosition = _agent.nextPosition - _agent.transform.position;
            worldDeltaPosition.y = 0;

            float dx = Vector3.Dot(transform.right, worldDeltaPosition);
            float dz = Vector3.Dot(transform.forward, worldDeltaPosition);
            Vector3 localDeltaPosition = new Vector3(dx, 0, dz);

            _smoothedDeltaPosition = Vector3.Lerp(_smoothedDeltaPosition, localDeltaPosition, 0.2f);

            Vector3 velocity = _smoothedDeltaPosition / Time.deltaTime;

            _animator.SetFloat("Locomotion", velocity.magnitude, 0.1f, Time.deltaTime);
            _animator.SetFloat("Dx", dx);
            _animator.SetFloat("Dz", dz);
        }

        private void OnDestroy()
        {
            _workerJobManager.RemoveWorker(this);
            
        }
    }
}


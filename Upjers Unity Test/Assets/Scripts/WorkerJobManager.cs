using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
namespace PlantingGame
{
    public class WorkerJobManager : MonoBehaviour
    {
        private List<WorkerAI> _workers = new List<WorkerAI>();
        private List<WorkerJob> _jobs = new List<WorkerJob>();

        private void Start()
        {
            StartCoroutine(JobListUpdate());
        }
        public void AddJob(WorkerJobType jobType, Crop crop)
        {
            WorkerJob job = new WorkerJob(jobType, crop);
            _jobs.Add(job);
        }

        public void AddWorker(WorkerAI worker)
        {
            if (!_workers.Contains(worker))
            {
                _workers.Add(worker);
            }
            else
            {
                Debug.LogError($"Worker {worker.name} is already in the list.",worker);
            }
        }

        /// <summary>
        /// Trys to assign a job to a worker every 0.5 seconds per job.
        /// </summary>
        /// <returns></returns>
        public IEnumerator JobListUpdate()
        {
            while(true)
            {
                if (_jobs.Count > 0)
                {
                    List<WorkerAI> availableWorkers = _workers.Where(w => w._currentJob == null).ToList();
                    List<WorkerJob> availableJobs = new List<WorkerJob>();
                    List<WorkerJob> assignedJobs = new List<WorkerJob>();
                    availableJobs.AddRange(_jobs);
                    if(availableWorkers.Count == 0)
                    {
                        yield return new WaitForSeconds(0.5f);
                        continue;
                    }
                    // Assign jobs to available workers
                    foreach (WorkerAI worker in availableWorkers)
                    {
                        try
                        {
                            WorkerJob job = availableJobs.First(j => j.jobType == worker.jobType);
                            if (worker.AssignJob(job))
                            {
                                availableJobs.Remove(job);
                                assignedJobs.Add(job);
                            }
                            
                        }
                        catch (Exception e)
                        {
                            Debug.Log($"No available job for worker {worker.name} / {worker.jobType}.");
                        }

                        yield return new WaitForSeconds(0.5f);
                    }

                    _jobs.RemoveAll(j => assignedJobs.Contains(j));
                }
                print("Working");
                yield return new WaitForSeconds(0.5f);
            }
        }

        public void RemoveWorker(WorkerAI worker)
        {
            if (_workers.Contains(worker))
            {
                _workers.Remove(worker);
            }
            else
            {
                Debug.LogError($"Worker {worker.name} is not in the list.",worker);
            }
        }
    }
}




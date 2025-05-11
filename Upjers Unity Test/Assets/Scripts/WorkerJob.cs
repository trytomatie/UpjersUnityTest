using System;
using UnityEngine;

namespace PlantingGame
{
    public class WorkerJob
    {
        public WorkerJob(WorkerJobType jobType,Crop crop)
        {
            this.jobType = jobType;
            this.crop = crop;
        }
        public WorkerJobType jobType;
        public Crop crop;
    }

    public enum WorkerJobType
    {
        None,
        Gardening,
        Harvesting,
    }   
}
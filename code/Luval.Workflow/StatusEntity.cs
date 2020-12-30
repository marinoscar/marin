using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace Luval.Workflow
{
    public abstract class StatusEntity
    {
        public StatusEntity()
        {
            Id = Guid.NewGuid().ToString();
            UtcCreatedOn = DateTime.UtcNow;
            UtcUpdatedOn = UtcCreatedOn;
            Status = ActivityStatus.Created;
            UtcEndedOn = null;
        }
        public string Id { get; internal set; }
        public string ExceptionReason { get; internal set; }
        public ActivityStatus Status { get; internal set; }
        public DateTime UtcStartedOn { get; internal set; }
        public DateTime? UtcEndedOn { get; internal set; }
        public DateTime UtcCreatedOn { get; internal set; }
        public DateTime UtcUpdatedOn { get; internal set; }

        public virtual void SetStart()
        {
            Status = ActivityStatus.InProgress;
            UtcUpdatedOn = DateTime.Now;
            UtcStartedOn = DateTime.UtcNow;
        }

        public virtual void SetException(Exception ex)
        {
            Status = ActivityStatus.Failed;
            ExceptionReason = ex.ToString().Substring(0, 5000);
            UtcUpdatedOn = DateTime.UtcNow;
            UtcEndedOn = UtcUpdatedOn;
        }

        public virtual void SetComplete()
        {
            Status = ActivityStatus.Success;
            UtcUpdatedOn = DateTime.UtcNow;
            UtcEndedOn = UtcUpdatedOn;
        }
    }
}

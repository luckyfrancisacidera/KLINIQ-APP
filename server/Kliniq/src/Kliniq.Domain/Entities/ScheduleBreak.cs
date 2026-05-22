using Kliniq.Domain.Common;

namespace Kliniq.Domain.Entities
{
    public class ScheduleBreak : BaseEntity
    {
        public Guid ScheduleId { get; private set; }
        public TimeOnly StartTime { get; private set; }
        public TimeOnly EndTime { get; private set; }

        private ScheduleBreak() { }

        internal ScheduleBreak(Guid scheduleId, TimeOnly startTime, TimeOnly endTime)
        {
            Id = Guid.NewGuid();
            ScheduleId = scheduleId;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}

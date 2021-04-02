using System;

namespace _04_shared_domain.Data
{
    public class DutyShard : IDataShard
    {
        public DutyShard() 
        { 
        }

        public DutyShard(long id, long employmentId, long dutyId, string dutyType, DateTime start, DateTime end)
        {
            this.Id = id;
            this.EmploymentId = employmentId;
            this.DutyId = dutyId;
            this.DutyType = dutyType;
            this.Start = start;
            this.End = end;
        }

        public long Id { get; set; }
        public long EmploymentId { get; set; }
        public long DutyId { get; set; }
        public string DutyType { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public DeviationShard[] Deviations { get; set; }
    }
}
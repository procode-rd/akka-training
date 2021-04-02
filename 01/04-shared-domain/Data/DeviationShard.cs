using System;

namespace _04_shared_domain.Data
{
    public class DeviationShard : IDataShard
    {
        public DeviationShard()
        {
        }
        public DeviationShard(long id, long dutyId, long deviationId, string deviationType, DateTime start, DateTime end)
        {
            this.Id = id;
            this.DutyId = dutyId;
            this.DeviationId = deviationId;
            this.DeviationType = deviationType;
            this.Start = start;
            this.End = end;
        }

        public long Id { get; set; }
        public long DutyId { get; set; }
        public long DeviationId { get; set; }
        public string DeviationType { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
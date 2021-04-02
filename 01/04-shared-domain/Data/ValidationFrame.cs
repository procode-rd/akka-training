using System;

namespace _04_shared_domain.Data
{
    public enum ValidationType
    {
        Ok,
        Notice,
        Warning,
        Error,
    }

    public class ValidationFrame : IDataShard
    {
        public ValidationFrame() 
        { 
        }

        public ValidationFrame(long id, long employmentId, ValidationType validationType, DateTime when, string message)
        {
            this.Id = id;
            this.EmploymentId = employmentId;
            this.ValidationType = validationType;
            this.When = when;
            this.Message = message;
        }

        public long Id { get; set; }
        public long EmploymentId { get; set; }
        public ValidationType ValidationType { get; set; }
        public DateTime When { get; set; }
        public string Message { get; set; }
    }

}
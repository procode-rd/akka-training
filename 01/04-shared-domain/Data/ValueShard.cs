namespace _04_shared_domain.Data
{
    public enum ValueType
    {
        Absence = 1,
        Normtime,
    }

    public class ValueShard : IDataShard
    {
        public ValueShard()
        {
        }

        public ValueShard(long id, long employmentId, ValueType valueType, decimal value)
        {
            this.Id = id;
            this.ValueType = valueType;
            this.Value = value;
            this.EmploymentId = employmentId;
        }

        public long Id { get; set; }
        public ValueType ValueType { get; set; }
        public decimal Value { get; set; }
        public long EmploymentId { get; set; }
    }
}
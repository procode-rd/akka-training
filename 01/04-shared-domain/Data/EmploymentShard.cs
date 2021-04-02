namespace _04_shared_domain.Data
{
    public class EmploymentShard : IDataShard
    {
        public EmploymentShard() 
        { 
        }

        public EmploymentShard(long id, string givenName, string surname)
        {
            this.Id = id;
            this.Name = givenName;
            this.Surname = surname;
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
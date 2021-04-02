using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _04_shared_domain.Data
{
    public interface IId
    {
        long Id { get; set; }
    }

    public interface IDataShard : IId
    {
    }
}

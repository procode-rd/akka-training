using _04_shared_domain.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _04_shared_domain.Request
{
    public class LoadDutyplanRequest : ApiRequest
    {
        public LoadDutyplanRequest(long customerId, long organizationId, DateTime from, DateTime to)
        {
            this.CustomerId = customerId;
            this.OrganizationId = organizationId;
            this.From = from;
            this.To = to;
        }

        public long CustomerId { get; }
        public long OrganizationId { get; }
        public DateTime From { get; }
        public DateTime To { get; }
    }
}

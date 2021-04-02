using System;
using System.Collections.Generic;
using System.Text;

namespace _04_shared_domain
{
    public abstract class ApiRequest
    {
        public Guid Id { get; }

        protected ApiRequest()
        {
            this.Id = Guid.NewGuid();
        }

        public override string ToString()
            => $"Request[{this.GetType().Name}, Id={this.Id}]";
    }
}

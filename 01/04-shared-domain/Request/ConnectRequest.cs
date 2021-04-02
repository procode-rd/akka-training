using System;
using System.Collections.Generic;
using System.Text;

namespace _04_shared_domain.Request
{
    public class ConnectRequest : ApiRequest
    {
    }

    public class ConnectResponse : ApiResponse<ConnectRequest>
    {
        public ConnectResponse()
            : base(default, default, default)
        {
        }

        public ConnectResponse(ApiRequest request, Version version, string message)
            : this(request, true, version, message)
        {
        }

        public ConnectResponse(ApiRequest request, bool result, Version version, string message = default)
            : base(request, result, message)
        {
            this.Version = version;
        }

        public Version Version { get; set; }
    }

}

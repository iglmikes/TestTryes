using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcClient.Support
{
    public class GrpcBusClientOptions
    {
        public string ServerUrl { get; set; } = "https://localhost:5001";
        public bool UseHttps { get; set; } = true;
        public TimeSpan KeepAliveInterval { get; set; } = TimeSpan.FromSeconds(60);
        public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromSeconds(15);
        public int MaxRetryAttempts { get; set; } = 3;
    }
}

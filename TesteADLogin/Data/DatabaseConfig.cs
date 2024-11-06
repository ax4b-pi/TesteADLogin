using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TesteADLogin.Data
{
    public class DatabaseConfig
    {
        public string ConnectionString { get; set; }
        public int MaxRetryCount { get; set; } = 3;
        public int CommandTimeout { get; set; } = 30;
        public bool EnableSensitiveDataLogging { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeterKottas.DotNetCore.EnsureRunning.SqlServer.DTO
{
    public class RunnerSqlDTO
    {
        public int Id { get; set; }

        public int ActionId { get; set; }

        public DateTime LastHeartBeat { get; set; }
    }
}

using PeterKottas.DotNetCore.EnsureRunning.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using PeterKottas.DotNetCore.EnsureRunning.SqlServer.DTO;
using PeterKottas.DotNetCore.EnsureRunning.DTO;
using PeterKottas.DotNetCore.EnsureRunning.Enum;
using System.Text.RegularExpressions;

namespace PeterKottas.DotNetCore.EnsureRunning.SqlServer
{
    public class SqlServerStorage : IStorage
    {
        private string connectionString;
        private SqlServerConfig config;
        private IDbConnection sqlConnection;
        private bool isConnected;
        private object lockObj = new { };

        public SqlServerStorage(string connectionString, SqlServerConfig config)
        {
            sqlConnection = new SqlConnection(connectionString);
            this.connectionString = connectionString;
            this.config = config;
            isConnected = false;
        }

        private void RunBatches(string sql)
        {
            string pattern = "[\\s](?i)GO(?-i)";
            var matcher = new Regex(pattern, RegexOptions.Compiled);
            int start = 0;
            int end = 0;
            Match batch = matcher.Match(sql);
            if (!batch.Success)
            {
                sqlConnection.Execute(sql);
            }
            else
            {
                while (batch.Success)
                {
                    end = batch.Index;
                    string batchQuery = sql.Substring(start, end - start).Trim();
                    //execute the batch
                    sqlConnection.Execute(batchQuery);
                    start = end + batch.Length;
                    batch = matcher.Match(sql, start);
                }
            }
        }

        private void Uninstall()
        {
            Console.WriteLine("uninstall");
            RunBatches(SqlServerSetup.UninstalSql);
        }

        private void Install()
        {
            Console.WriteLine("install");
            RunBatches(SqlServerSetup.InstalSql);
        }

        private void Reinstall()
        {
            Uninstall();
            Install();
        }

        public ConnectResponseDTO Connect()
        {
            sqlConnection.Open();
            try
            {
                var dbVersion = sqlConnection.QueryFirst<int>("select * from version");
                if (dbVersion < SqlServerSetup.CurrentVersion)
                {
                    Reinstall();
                }
            }
            catch (Exception e)
            {
                Reinstall();
            }
            isConnected = true;
            return new ConnectResponseDTO();
        }

        private IEnumerable<RunnerSqlDTO> CleanUpRunners(IEnumerable<RunnerSqlDTO> allRunners, int HeartBeatIntervalMs, IDbTransaction transactionScope)
        {
            var oldRunners = allRunners.Where(runner =>
                (DateTime.Now.ToUniversalTime() - runner.LastHeartBeat).TotalMilliseconds > HeartBeatIntervalMs);
            var currentRunners = allRunners.Where(runner =>
                (DateTime.Now.ToUniversalTime() - runner.LastHeartBeat).TotalMilliseconds < HeartBeatIntervalMs);
            foreach (var oldRunner in oldRunners)
            {
                sqlConnection.Execute(@"delete from Runners where Id = @runnerId;",
                    new { runnerId = oldRunner.Id }, transactionScope);
            }
            return currentRunners;
        }

        public TryStartResponseDTO TryStart(TryStartRequestDTO request)
        {
            lock (lockObj)
            {
                var ret = new TryStartResponseDTO();
                using (var transactionScope = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        var action = sqlConnection.QueryFirstOrDefault<ActionSqlDTO>($@"select * from actions where ActionId = @actionId",
                            new { actionId = request.ActionId }, transactionScope);
                        if (action == null)
                        {
                            action = new ActionSqlDTO()
                            {
                                ActionId = request.ActionId
                            };

                            action.Id = sqlConnection.QueryFirst<int>(@"INSERT INTO Actions (ActionId) Values(@actionId);
                            SELECT CAST(SCOPE_IDENTITY() as int)", new { actionId = request.ActionId }, transactionScope);
                        }
                        var runners = sqlConnection.Query<RunnerSqlDTO>($@"select * from runners where ActionId = @actionId",
                        new { actionId = action.Id }, transactionScope);
                        var currentRunners = CleanUpRunners(runners, request.HeartBeatTimeoutMs, transactionScope);
                        if (currentRunners.Count() < request.NumberOfTimes)
                        {
                            ret.Status = TryStartStatusEnum.CanStart;
                            ret.HeartBeatId = sqlConnection.QueryFirst<int>(@"INSERT INTO Runners 
                                (ActionId, LastHeartBeat) Values(@actionId,@lastHeartBeat);
                                SELECT CAST(SCOPE_IDENTITY() as int)",
                                new { actionId = action.Id, lastHeartBeat = DateTime.Now.ToUniversalTime() },
                                transactionScope);
                        }
                        else
                        {
                            ret.Status = TryStartStatusEnum.CannotStart;
                        }
                        transactionScope.Commit();
                    }
                    catch (Exception)
                    {
                        transactionScope.Rollback();
                        throw;
                    }
                }
                return ret;
            }
        }

        public bool IsConnected()
        {
            return isConnected;
        }

        public void Dispose()
        {
            isConnected = false;
            sqlConnection.Dispose();
        }

        public HeartBeatResponseDTO HeartBeat(HeartBeatRequestDTO request)
        {
            lock (lockObj)
            {
                using (var transactionScope = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        var runner = sqlConnection.QueryFirstOrDefault<RunnerSqlDTO>($@"select * from runners where 
                            Id = @runnerId",
                            new { runnerId = request.HeartBeatId }, transactionScope);
                        if (runner == null)
                        {
                            return new HeartBeatResponseDTO()
                            {
                                Status = HeartBeatStatusEnum.Stop
                            };
                        }
                        sqlConnection.Execute("update Runners set LastHeartBeat = @val where Id = @id", new { val = DateTime.Now.ToUniversalTime(), id = runner.Id }, transactionScope);
                        transactionScope.Commit();
                        return new HeartBeatResponseDTO()
                        {
                            Status = HeartBeatStatusEnum.Continue
                        };
                    }
                    catch (Exception)
                    {
                        transactionScope.Rollback();
                        throw;
                    }

                }
            }
        }
    }
}

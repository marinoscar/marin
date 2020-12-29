using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.FastSpeedTestApi
{
    public class SpeedTest
    {

        private TestOptions _options;
        private ILogger _logger;

        public SpeedTest(string token) : this(token, null)
        {

        }

        public SpeedTest(TestOptions options) : this(options, null)
        {

        }

        public SpeedTest(string token, ILogger logger) : this(new TestOptions(token), logger)
        {

        }

        public SpeedTest(TestOptions options, ILogger logger)
        {
            if (options == null) throw new ArgumentNullException("options");
            if (string.IsNullOrWhiteSpace(options.Token)) throw new ArgumentNullException("token");

            _options = options;
            _logger = logger;
        }

        public  SpeedTestResult GetSpeed()
        {
            var targetResult = GetTargets();
            if (targetResult == null || targetResult.Response.StatusCode != HttpStatusCode.OK)
                throw new InvalidOperationException(string.Format("Failed to acquire targets status code {0} and description {1}", 
                    targetResult.Response.StatusCode, targetResult.Response.StatusDescription));

            LogInfo("Starting test for targets {0}{1}", Environment.NewLine, string.Join(Environment.NewLine, targetResult.Targets.Select(i => i.UrlHost)));
            var start = DateTime.UtcNow;

            var tasks = RunConcurrently(targetResult);

            var execStatus = tasks.Select(i => new PackageExecutionStatus() 
                                    {
                                        PackageResult = i.Result, TaskStatus = i.Status
                                    }).ToList();

            var bitsPerSecond = execStatus.Where(i => i.TaskStatus == TaskStatus.RanToCompletion && i.PackageResult.StatusCode == HttpStatusCode.OK)
                .Select(i => i.PackageResult.BitsPerSecond).Max();
            var res = new SpeedTestResult()
            {
                TestUnit = _options.Unit,
                ExecutionStatus = execStatus,
                BytesPerSecond = bitsPerSecond / 8,
                DownloadSpeed = ConvertSpeedToUnitOpt(bitsPerSecond, _options.Unit),
                TotalDuration = DateTime.UtcNow.Subtract(start)
            };
            LogInfo(PrintResult(res));
            return res;
        }

        private IEnumerable<Task<PackageResult>> RunConcurrently(ApiTargetResult targetResult)
        {
            var tokenSource = new CancellationTokenSource();
            var tasks = targetResult.Targets.Select(i => GetPackage(i, tokenSource.Token)).ToArray();
            RunInBatch(tasks, _options.TimeoutInMilliseconds, tokenSource.Token, _options.ConcurrentConnections);
            return tasks;
        }

        private void RunInBatch(Task[] tasks, int timeoutInMs, CancellationToken token, int batchSize)
        {
            while(tasks.Any(i => i.Status == TaskStatus.Created))
            {
                LogInfo("Tasks remaining: {0}", tasks.Count(i => i.Status == TaskStatus.Created));
                var toRun = tasks.Where(i => i.Status == TaskStatus.Created).Take(batchSize).ToArray();
                foreach (var task in toRun)
                {
                    task.Start();
                }
                Task.WaitAll(toRun, timeoutInMs, token);
                LogInfo("Tasks completed: {0}", tasks.Count(i => i.Status != TaskStatus.Created));
            }
        }

        private string PrintResult(SpeedTestResult res)
        {
            var sb = new StringWriter();
            sb.WriteLine();
            sb.WriteLine();
            sb.WriteLine("Test Completed");
            sb.WriteLine();
            sb.WriteLine("Download Speed: {0} {1}ps", res.DownloadSpeed.ToString("N0"), res.TestUnit);
            sb.WriteLine("Total Test Duration: {0}", res.TotalDuration);
            sb.WriteLine();
            foreach (var item in res.ExecutionStatus)
            {
                if (item.TaskStatus != TaskStatus.RanToCompletion)
                {
                    sb.WriteLine("FAILED: {0}", item.PackageResult.Target.Name);
                    continue;
                }
                sb.WriteLine("Status: {0} - Duration: {1} - Size: {2} City: {3} - Host: {4} ", item.PackageResult.StatusCode, item.PackageResult.Duration, item.PackageResult.SizeInBits.ToString("N0"), item.PackageResult.Target.Location.City, item.PackageResult.Target.UrlHost);
            }
            sb.WriteLine();
            return sb.ToString();
        }

        private  ApiTargetResult GetTargets()
        {
            var client = new RestClient("https://api.fast.com/netflix/speedtest/v2");
            var request = new RestRequest("", Method.GET, DataFormat.Json);
            request.AddParameter("https", _options.UseHttps, ParameterType.QueryString);
            request.AddParameter("token", _options.Token, ParameterType.QueryString);
            request.AddParameter("urlCount", _options.UrlCount, ParameterType.QueryString);
            
            LogInfo("Getting targets from api");
            var response = client.Execute<ApiTargetResult>(request);
            response.Data.Response = response;
            response.Data.Targets = UpdateTargets(response.Data.Targets);
            LogInfo("Api response {0} {1}", response.StatusCode, response.StatusDescription);
            return response.Data;
        }

        private List<Target> UpdateTargets(IEnumerable<Target> targets)
        {
            var res = new List<Target>();
            foreach (var t in targets)
            {
                res.AddRange(new[] { 
                    new Target() { Location = t.Location, Name = t.Name, Url = t.Url.Replace("/speedtest?", "/speedtest/range/0-0?") },
                    new Target() { Location = t.Location, Name = t.Name, Url = t.Url.Replace("/speedtest?", "/speedtest/range/0-2048?") },
                    new Target() { Location = t.Location, Name = t.Name, Url = t.Url.Replace("/speedtest?", "/speedtest/range/0-26214400?") },
                });
            }
            return res;
        }

        private Task<PackageResult> GetPackage(Target target, CancellationToken cancellationToken)
        {
            return new Task<PackageResult>(() => {
                var start = DateTime.UtcNow;
                LogInfo("Package request {0}", target.Name);
                var response = WithHttpClient(target.Url).Result;
                var duration = DateTime.UtcNow.Subtract(start);
                LogInfo("Request completed {0} for {1}", duration, target.Name);
                return new PackageResult()
                {
                    Duration = duration,
                    Target = target,
                    SizeInBits = response.SizeInBits,
                    StatusCode = response.StatusCode,
                    BitsPerSecond = response.SizeInBits / (decimal)duration.TotalSeconds
                };
            }, cancellationToken);
        }

        private Task<RequestResult> WithHttpClient(string url)
        {
            return Task<RequestResult>.Run(() => {
                var client = new HttpClient();
                var response = client.GetAsync(url).Result;
                var bytes = response.Content.Headers.ContentLength.Value;
                return new RequestResult() { StatusCode = response.StatusCode, SizeInBits = bytes * 8 };
            });
        }

        private  decimal ConvertSpeedToUnitOpt(decimal value, TestUnit unit)
        {
            switch (unit)
            {
                case TestUnit.Kb:
                    return value / 1024;
                case TestUnit.Mb:
                    return value / (1024 * 1024);
                case TestUnit.Gb:
                    return value / (1024 * 1024 * 1024);
                default:
                    return value;
            }
        }

        #region Logging

        private void LogInfo(string message, params object[] args) { DoLog(LogType.Info, message, args); }
        private void LogWarning(string message, params object[] args) { DoLog(LogType.Warning, message, args); }
        private void LogError(string message, params object[] args) { DoLog(LogType.Error, message, args); }

        private void DoLog(LogType logType, string message, params object[] args)
        {
            if (_logger == null) return;
            Action<string, object[]> log;
            switch (logType)
            {
                case LogType.Warning:
                    log = _logger.LogWarning;
                    break;
                case LogType.Error:
                    log = _logger.LogError;
                    break;
                default:
                    log = _logger.LogInformation;
                    break;
            }
            log(message, args);
        }

        private enum LogType { Info, Warning, Error } 

        #endregion
    }
}

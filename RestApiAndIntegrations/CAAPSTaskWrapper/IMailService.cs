using System;
using System.Threading.Tasks;

namespace AD.CAAPSTaskWrapper
{
    public interface  IMailService
    {
        public Task<bool> SendEmailReport(string runOptionsPath, string runOptionsSwitches, DateTime startTime, int exitCode, string stdOutput, string errOutput, bool verbose = false);
    };
}

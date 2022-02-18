using System;
using AD.CAAPS.ErpPaymentRequest.Common;
using Microsoft.Extensions.Configuration;
using System.IO;
using NLog;
using NLog.Extensions.Logging;
using AD.CAAPS.Services;
using AD.CAAPS.Entities;
using AD.CAAPS.Common;
using System.Threading.Tasks;


namespace AD.CAAPS.ErpPaymentRequest.ThinkChildCare
{
    internal class Program
    {
        private static async Task<int> Main(string[] args)
        {
            var program = new CustomProgram();
            return await program.Main(args).ConfigureAwait(false);
        }
    }
}

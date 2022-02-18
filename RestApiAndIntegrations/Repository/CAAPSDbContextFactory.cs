using AD.CAAPS.Common;
using AD.CAAPS.Entities;
using Microsoft.EntityFrameworkCore;
using NLog;
using System;

namespace AD.CAAPS.Repository
{
    public static class CAAPSDbContextFactory
    {
        // private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        //Dynamically establish a connection between entity framework to a database by passing a connectionString
        public static CAAPSDbContext Create(DBConfiguration dbConfiguration)
        {
            if (dbConfiguration is null)
            {
                throw new ArgumentNullException(nameof(dbConfiguration));
            }

            if (string.IsNullOrWhiteSpace(dbConfiguration.ConnectionString))
            {
                throw new ConfigurationMissingException("No value assigned for required configuration value: \"dbConfiguration.ConnectionString\"");
            }

            // logger.Debug("Attempting to generate and return CAAPSDbContext.");
            return new CAAPSDbContext(GetContextOptionsBuilder(dbConfiguration.ConnectionString).Options, dbConfiguration);
        }

        public static DbContextOptionsBuilder<CAAPSDbContext> GetContextOptionsBuilder(string connectionString)
        {
            // logger.Debug("Creating options builder for CAAPSDbContext");
            return new DbContextOptionsBuilder<CAAPSDbContext>()
                                       .UseSqlServer(connectionString)
                                       .UseLazyLoadingProxies(false);
        }
    }
}
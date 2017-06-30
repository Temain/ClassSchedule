using System.Web.Configuration;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace ClassSchedule.Web
{
    public class NLogConfig
    {
        public static void Configure()
        {
            // Step 1. Create configuration object 
            var config = new LoggingConfiguration();

            // Step 2. Create targets and add them to the configuration 
            var databaseTarget = new DatabaseTarget();
            config.AddTarget("database", databaseTarget);           

            // Step 3. Set target properties 
            databaseTarget.ConnectionString = WebConfigurationManager.ConnectionStrings["ClassScheduleConnection"].ConnectionString;
            databaseTarget.CommandText =
                @"INSERT INTO [serv].[LogEntry] ([Date], [Level], [Logger], [ClassMethod], [Message], [Username], [RequestUri], [RemoteAddress], [UserAgent], [Exception]) 
                    VALUES (@Date, @Level, @Logger, @ClassMethod, @Message, @Username, @RequestUri, @RemoteAddress, @UserAgent, @Exception);";

            databaseTarget.Parameters.Add(new DatabaseParameterInfo("@Date", "${date:format=yyyy-MM-ddTHH\\:mm\\:ss.fff}"));
            databaseTarget.Parameters.Add(new DatabaseParameterInfo("@Level", "${level}"));
            databaseTarget.Parameters.Add(new DatabaseParameterInfo("@Logger", "${logger}"));
            databaseTarget.Parameters.Add(new DatabaseParameterInfo("@ClassMethod", "${callsite:className=false:includeSourcePath=false:methodName=true}"));
            databaseTarget.Parameters.Add(new DatabaseParameterInfo("@Message", "${message}"));
            databaseTarget.Parameters.Add(new DatabaseParameterInfo("@Username", @"${aspnet-user-identity:whenEmpty=${gdc:UserName}}"));
            databaseTarget.Parameters.Add(new DatabaseParameterInfo("@RequestUri", "${aspnet-request:item=HTTP_METHOD} ${aspnet-request:serverVariable=Url}"));
            databaseTarget.Parameters.Add(new DatabaseParameterInfo("@RemoteAddress", "${aspnet-request:serverVariable=remote_host}"));
            databaseTarget.Parameters.Add(new DatabaseParameterInfo("@UserAgent", "${aspnet-request:serverVariable=HTTP_USER_AGENT}"));
            databaseTarget.Parameters.Add(new DatabaseParameterInfo("@Exception", "${exception:format=ToString}${newline}"));
            //databaseTarget.Parameters.Add(new DatabaseParameterInfo("@Exception", "${exception:format=Message,Type,ShortType,ToString,Method,StackTrace,Data}${newline}"));

            AsyncTargetWrapper wrapper = new AsyncTargetWrapper
            {
                WrappedTarget = databaseTarget,
                QueueLimit = 5000,
                OverflowAction = AsyncTargetWrapperOverflowAction.Discard
            };
            SimpleConfigurator.ConfigureForTargetLogging(wrapper, LogLevel.Debug);

            // Step 4. Define rules
            var rule = new LoggingRule("*", LogLevel.Debug, databaseTarget);
            config.LoggingRules.Add(rule);

            // Step 5. Activate the configuration
            LogManager.Configuration = config;
        }
    }
}
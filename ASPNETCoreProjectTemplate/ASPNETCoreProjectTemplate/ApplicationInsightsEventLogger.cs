using System.Collections.Generic;
using Microsoft.ApplicationInsights;

namespace ASPNETCoreProjectTemplate
{
    public class ApplicationInsightsEventLogger : IEventLogger
    {
        private readonly TelemetryClient telemetryClient;

        public ApplicationInsightsEventLogger(TelemetryClient telemetryClient)
        {
            this.telemetryClient = telemetryClient;
        }

        public void Log(LogEvent @event)
        {
            telemetryClient.TrackEvent(@event.EventName, @event.Fields);
        }

        public void LogEvent(string name)
        {
            telemetryClient.TrackEvent(name);
        }

        public void LogEvent(string name, IDictionary<string, string> fields)
        {
            telemetryClient.TrackEvent(name, fields, null);
        }
    }
}
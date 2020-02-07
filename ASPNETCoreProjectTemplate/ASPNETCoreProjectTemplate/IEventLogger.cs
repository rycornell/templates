using System.Collections.Generic;

namespace ASPNETCoreProjectTemplate
{
    /// <summary>
    /// Interface for logging an event.
    /// </summary>
    public interface IEventLogger
    {
        void LogEvent(string name);

        void LogEvent(string name, IDictionary<string, string> fields);

        void Log(LogEvent @event);
    }

    /// <summary>
    /// Contains data about an event.
    /// </summary>
    public class LogEvent
    {
        Dictionary<string, string> _fields;

        public LogEvent(string eventName)
        {
            EventName = eventName;
            _fields = new Dictionary<string, string>();
        }

        public string EventName { get; }
        public string User { get; set; }
        public bool? Authorized { get; set; }
        public bool? Successful { get; set; }
        public IDictionary<string, string> Fields
        {
            get
            {
                _fields[nameof(User)] = User;
                _fields[nameof(Authorized)] = Authorized.ToString();
                _fields[nameof(Successful)] = Successful.ToString();
                return _fields;
            }
        }
    }
}
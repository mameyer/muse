using TSGenerator.Enums;

namespace TSGenerator.Modules
{
    public abstract class ModuleBase
    {
        protected LogAction Log;

        public delegate void LogAction(string s, LogMessageType logMessageType = LogMessageType.Info, params object[] args);

        public ModuleBase(LogAction log)
        {
            this.Log = log;
        }
    }
}

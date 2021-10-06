namespace LcmsNetPlugins.Agilent.Pumps
{
    public enum AgilentPumpStateGeneric
    {
        PRERUN = 0,
        RUN = 1,
        POSTRUN = 2,
        SHUTDOWN = 3,
        LEAK = 4,
        STARTUP = 5,
    }

    public enum AgilentPumpStateAnalysis
    {
        NO_ANALYSIS = 0,
        ANALYSIS = 1,
        STARTUP = 2,
    }

    public enum AgilentPumpStateError
    {
        NO_ERROR = 0,
        ERROR = 1,
        STARTUP = 2,
    }

    public enum AgilentPumpStateNotReady
    {
        READY = 0,
        NOT_READY = 1,
        STARTUP = 2,
    }

    public enum AgilentPumpStateTest
    {
        NO_TEST = 0,
        TEST = 1,
        STARTUP = 2,
    }
}

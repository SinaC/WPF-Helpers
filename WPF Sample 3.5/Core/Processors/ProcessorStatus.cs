namespace SampleWPF.Core.Processors
{
    public enum ProcessorStatus
    {
        Created,
        Initializing,
        Initialized,
        NotValidated, // Final state
        Executing,
        Executed,
        UnhandledException, // Final state
        Terminated, // Final state
        TerminatedWithErrors, // Final state
    }
}

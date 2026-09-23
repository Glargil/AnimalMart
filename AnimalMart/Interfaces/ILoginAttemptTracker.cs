namespace AnimalMart.Interfaces
{
    public interface ILoginAttemptTracker
    {
        bool IsLockedOut(string ipAddress);
        void RecordFailure(string ipAddress);
        void RecordSuccess(string ipAddress);
    }
}

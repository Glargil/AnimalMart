using AnimalMart.Interfaces;
using System.Collections.Concurrent;

namespace AnimalMart.Services
{
    public class LoginAttemptTracker : ILoginAttemptTracker
    {
        private const int MaxAttempts = 5;
        private static readonly TimeSpan Window = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(5);

        private class AttemptRecord
        {
            public int FailureCount;
            public DateTimeOffset WindowStart;
            public DateTimeOffset? LockedUntil;
        }

        private readonly ConcurrentDictionary<string, AttemptRecord> _attempts = new();

        public bool IsLockedOut(string ipAddress)
        {
            if (!_attempts.TryGetValue(ipAddress, out var record))
            {
                return false;
            }

            lock (record)
            {
                if (record.LockedUntil is DateTimeOffset lockedUntil)
                {
                    if (DateTimeOffset.UtcNow < lockedUntil)
                    {
                        return true;
                    }
                    record.LockedUntil = null;
                    record.FailureCount = 0;
                }
                return false;
            }
        }

        public void RecordFailure(string ipAddress)
        {
            var record = _attempts.GetOrAdd(
                ipAddress,
                _ => new AttemptRecord { WindowStart = DateTimeOffset.UtcNow }
            );

            lock (record)
            {
                var now = DateTimeOffset.UtcNow;
                if (now - record.WindowStart > Window)
                {
                    record.WindowStart = now;
                    record.FailureCount = 0;
                }
                record.FailureCount++;

                if (record.FailureCount >= MaxAttempts)
                {
                    record.LockedUntil = now + LockoutDuration;
                }
            }
        }

        public void RecordSuccess(string ipAddress)
        {
            _attempts.TryRemove(ipAddress, out _);
        }
    }
}

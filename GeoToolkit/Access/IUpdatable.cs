using System;

namespace GeoToolkit.Access
{
    public interface IUpdatable
    {
        object Update(object param = null);
        object UpdateSyncronized(object param = null);

        DateTime LastUpdated { get; }

        TimeSpan UpdateInterval { get; set; }

        bool UpdateAllowed { get; }
    }
}
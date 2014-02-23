using System;

namespace GeoToolkit.Access
{
    public interface IUpdatable<TRes, TPar> : IUpdatable
    {
        TRes Update(TPar param = default(TPar));
        TRes UpdateSyncronized(TPar param = default(TPar));

        event EventHandler<UpdatingEventArgs<TPar>> Updating;

        event EventHandler<UpdatedEventArgs<TRes>> Updated;
    }
}
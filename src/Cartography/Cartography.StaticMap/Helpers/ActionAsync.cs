using System.Threading;
using System.Threading.Tasks;

namespace Cartography.Static.Helpers
{
    public delegate Task ActionAsync(CancellationToken ct);
    public delegate Task ActionAsync<in T1>(CancellationToken ct, T1 value);
    public delegate Task ActionAsync<in T1, in T2>(CancellationToken ct, T1 t1, T2 t2);
    public delegate Task ActionAsync<in T1, in T2, in T3>(CancellationToken ct, T1 t1, T2 t2, T3 t3);
    public delegate Task ActionAsync<in T1, in T2, in T3, in T4>(CancellationToken ct, T1 t1, T2 t2, T3 t3, T4 t4);
}
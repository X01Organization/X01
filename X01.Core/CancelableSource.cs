
using System.Threading;

namespace X01
{

    public class CancelableSource<TObject>
    {
        readonly TObject _obj;
        readonly CancellationToken? _ct;
        public TObject Obj
        {
            get
            {
                _ct?.ThrowIfCancellationRequested();
                return (_obj);
            }
        }
        public CancelableSource(TObject obj, CancellationToken? ct)
        {
            _obj = obj;
            _ct = ct;
        }
        public CancelableSource(TObject obj) : this(obj, null)
        {
        }
        //public static implicit operator TObject(CancelableSource<TObject> c) => c.Obj;
        // public static explicit operator CancelableSource<TObject>(TObject b, CancellationToken? ct) => new CancelableSource<TObject>(b, ct);
    }

    public static class CancelableSourceExt
    {
        public static IEnumerable<TItem> MonitorCancellation(this CancelableSource cs, IEnumerable<TItem> enumerable)
        {
            return enumerable.select(x =>
            {
                cs.throwifcancellationrequested
            return x;
            });
        }
 
using System;

namespace Emilie.Core.Common
{
    public sealed class EquatableWeakReference
    {
        private int _hashCode;
        private WeakReference _weakRef;

        public EquatableWeakReference(object o)
        {
            _weakRef = new WeakReference(o);
            _hashCode = o.GetHashCode();
        }

        public bool IsAlive => _weakRef.IsAlive; 

        public object Target => _weakRef.Target; 

        public override bool Equals(object o)
        {
            if (o == null)
                return false;

            if (o.GetHashCode() != _hashCode)
                return false;

            if (o == this || object.ReferenceEquals(o, Target))
                return true;

            return false;
        }

        public override int GetHashCode() => _hashCode;
    }
}

using System;

namespace Argosy.Framework {
    public abstract class DroppablePrimitive<T> : IDroppablePrimitive<T> where T : IConvertible, IComparable<T>, IEquatable<T> {
        public T PrimitiveDropInfo { get; private set; }
        public double Weight { get; set; }
        public bool GuaranteedDrop { get; set; }

        #region CONSTRUCTORS
        public DroppablePrimitive(T primitiveDropInfo, double weight, bool guaranteedDrop) {
            if (primitiveDropInfo == null) {
                throw new ArgumentNullException();
            }

            PrimitiveDropInfo = primitiveDropInfo;
            Weight = weight;
            GuaranteedDrop = guaranteedDrop;
        }
        #endregion

        #region EQUALITY METHODS
        public override bool Equals(object obj) {
            if (obj == null) return false;
            if (this.GuaranteedDrop) return false;
            if (this.GetType() != obj.GetType()) return false;

            return this.GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode() {
            return this.PrimitiveDropInfo.GetHashCode();
        }
        #endregion
    }
}
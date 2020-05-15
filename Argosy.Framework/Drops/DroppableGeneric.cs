using System;

namespace Argosy.Framework {
    public abstract class DroppableGeneric<T> : IDroppableGeneric<T> where T : ICustomDropInfo {
        public T CustomDropInfo { get; protected set; }
        public double Weight { get; set; }
        public bool GuaranteedDrop { get; set; }

        public virtual void SetCustomDropInfo(T customDropInfo) {
            CustomDropInfo = customDropInfo;
        }

        #region CONSTRUCTORS
        public DroppableGeneric(double weight, bool guaranteedDrop) {
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
            if (CustomDropInfo == null) {
                throw new ArgumentNullException();
            }

            return this.CustomDropInfo.UniqueID.GetHashCode();
        }
        #endregion
    }
}
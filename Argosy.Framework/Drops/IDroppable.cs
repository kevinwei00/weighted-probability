using System;

namespace Argosy.Framework {
    /// <summary>
    /// Implementers must also override Object.Equals() and Object.GetHashCode()!
    /// </summary>
    public interface IDroppable {
        double Weight { get; set; }
        bool GuaranteedDrop { get; set; }
    }

    /// <summary>
    /// Implementers must also override Object.Equals() and Object.GetHashCode()!
    /// </summary>
    public interface IDroppablePrimitive<T> : IDroppable where T : IConvertible, IComparable<T>, IEquatable<T> {
        T PrimitiveDropInfo { get; }
    }

    /// <summary>
    /// Implementers must also override Object.Equals() and Object.GetHashCode()!
    /// </summary>
    public interface IDroppableGeneric<T> : IDroppable where T : ICustomDropInfo {
        T CustomDropInfo { get; }
        void SetCustomDropInfo(T customDropInfo);
    }

    /// <summary>
    /// The implementer contains a Unique ID which protects against adding duplicate drops to a drop table.
    /// </summary>
    public interface ICustomDropInfo {
        string UniqueID { get; }
    }
}
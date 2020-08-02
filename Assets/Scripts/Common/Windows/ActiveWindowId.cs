using UnityEngine;

using System;
using System.Collections.Generic;

namespace STP.Common.Windows {
    public readonly struct ActiveWindowId {
            public static ActiveWindowId Empty => new ActiveWindowId(null, null);
            
            public readonly Type       WindowType;
            public readonly GameObject WindowRoot;

            public bool IsEmpty => ((WindowType == null) || !WindowRoot);

            ActiveWindowId(Type windowType, GameObject windowRoot) {
                WindowType = windowType;
                WindowRoot = windowRoot;
            }

            sealed class WindowTypeWindowRootEqualityComparer : IEqualityComparer<ActiveWindowId> {
                public bool Equals(ActiveWindowId x, ActiveWindowId y) {
                    return Equals(x.WindowType, y.WindowType) && Equals(x.WindowRoot, y.WindowRoot);
                }

                public int GetHashCode(ActiveWindowId obj) {
                    unchecked {
                        return ((obj.WindowType != null ? obj.WindowType.GetHashCode() : 0) * 397) ^ (obj.WindowRoot != null ? obj.WindowRoot.GetHashCode() : 0);
                    }
                }
            }

            public static bool operator ==(ActiveWindowId? a, ActiveWindowId? b) {
                if ( !a.HasValue || !b.HasValue ) {
                    return ReferenceEquals(a, b);
                }
                return ((a.Value.WindowType == b.Value.WindowType) && (a.Value.WindowRoot == b.Value.WindowRoot));
            }

            public static bool operator !=(ActiveWindowId? a, ActiveWindowId? b) {
                return !(a == b);
            }

            public static implicit operator ActiveWindowId((Type windowType, GameObject windowRoot) t) {
                return new ActiveWindowId(t.windowType, t.windowRoot);
            }

            public static IEqualityComparer<ActiveWindowId> WindowTypeWindowRootComparer { get; } = new WindowTypeWindowRootEqualityComparer();

            public bool Equals(ActiveWindowId other) {
                return Equals(WindowType, other.WindowType) && Equals(WindowRoot, other.WindowRoot);
            }

            public override bool Equals(object obj) {
                return obj is ActiveWindowId other && Equals(other);
            }

            public override int GetHashCode() {
                unchecked {
                    return ((WindowType != null ? WindowType.GetHashCode() : 0) * 397) ^ (WindowRoot != null ? WindowRoot.GetHashCode() : 0);
                }
            }
        }
}

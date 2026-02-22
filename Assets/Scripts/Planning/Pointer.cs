using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Planning
{
    public class Pointer
    {
        public object value; // Either a concrete value or another Pointer
        public Type type;    // Declared type for this logical variable
        private List<object> bannedList = new List<object>(); // pointers (and their values) that this pointer cannot unify with 

        private static int counter = 0; // global counter for unique variable names
        public string Name { get; private set; } // unique name for unbound pointers

        // Constructor for unbound variable
        public Pointer(Type t)
        {
            this.type = t ?? throw new ArgumentNullException(nameof(t));
            this.Name = $"?v{counter}";
            counter++;
        }

        // Constructor from concrete value
        public Pointer(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "Cannot infer type from null value.");

            this.value = value;
            this.type = value.GetType();
            this.Name = value.ToString(); // use value's string representation as name for better debugging
        }

        // Constructor from value and type
        public Pointer(object value, Type t)
        {
            if (t == null)
                throw new ArgumentNullException(nameof(t));

            if (value != null && !t.IsInstanceOfType(value))
                throw new ArgumentException($"Value of type {value.GetType().Name} does not match declared type {t.Name}");

            this.value = value;
            this.type = t;
            this.Name = $"?v{counter}";
            counter++;
        }

        /// Get the final value (resolves pointer chains)
        public object Get() => Get(new HashSet<Pointer>());

        private object Get(HashSet<Pointer> visited)
        {
            if (visited.Contains(this))
                throw new InvalidOperationException("Cycle detected in pointer chain.");

            visited.Add(this);

            return value is Pointer p ? p.Get(visited) : value;
        }

        /// Typed version of Get()
        public T Get<T>()
        {
            object val = Get();
            if (val == null) return default;

            if (val is T tVal) return tVal;

            throw new InvalidCastException($"Cannot cast value of type {val.GetType().Name} to {typeof(T).Name}");
        }

        /// Returns declared type
        public Type GetDeclaredType() => type;

        /// Sets value, enforcing type and avoiding cycles
        public void Set(object val)
        {
            if (val != null && !type.IsInstanceOfType(val))
                throw new InvalidOperationException($"Pointer of type {type.Name} cannot be set to value of type {val.GetType().Name}");

            Set(val, new HashSet<Pointer>());
        }

        private void Set(object val, HashSet<Pointer> visited)
        {
            if (visited.Contains(this))
                throw new InvalidOperationException("Cycle detected while setting value.");

            visited.Add(this);

            if (value is Pointer p)
                p.Set(val, visited);
            else
                value = val;
        }

        public void Ban(IEnumerable<object> objects)
        {
            foreach (object o in objects)
            {
               bannedList.Add(o);
            }
            //PrintBanList();
        }

        public bool IsBanned(object o)
        {
            foreach (object b in bannedList)
            {
                if (b is Pointer p)
                {
                    // If banned pointer is bound, check its value
                    var val = p.Get();
                    if (val != null && object.Equals(o, val))
                        return true;

                    // Also check if 'o' is the same pointer as 'p'
                    if (object.ReferenceEquals(o, p))
                        return true;
                }
                else
                {
                    if (object.Equals(o, b))
                        return true;
                }
            }

            return false;
        }

        public void PrintBanList()
        {
            string s = "";
            foreach(object b in bannedList)
            {
                s += b.ToString() + ", ";
            }
            Debug.Log($"Ban list for {this.Name}: {s}");
        }

        /// Checks if bound to a concrete value
        public bool IsBound() => Get() != null;

        /// Checks whether the current value is another Pointer
        public bool IsPointer() => value is Pointer;

        /// Bind this pointer to another, unifying them
        public void BindTo(Pointer other)
        {
            if (this == other) return;

            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (this.type != other.type)
                throw new InvalidOperationException($"Cannot bind Pointer<{type.Name}> to Pointer<{other.type.Name}>");

            if (CreatesCycle(other))
                throw new InvalidOperationException("Binding would create a cycle.");

            if (!this.IsBound())
            {
                this.value = other;
            }
            else if (!other.IsBound())
            {
                other.value = this;
            }
            else if (!object.Equals(this.Get(), other.Get()))
            {
                throw new InvalidOperationException("Conflict during unification: values differ.");
            }
        }

        private bool CreatesCycle(Pointer other)
        {
            var visited = new HashSet<Pointer>();
            Pointer current = other;

            while (current != null)
            {
                if (current == this)
                    return true;

                if (visited.Contains(current))
                    return false;

                visited.Add(current);
                current = current.value as Pointer;
            }

            return false;
        }

        /// Checks whether two pointers resolve to the same final value
        public bool isSameValue(Pointer p) =>
            object.Equals(this.Get(), p?.Get());

        /// Clone this pointer (preserves variable name)
        public Pointer Clone()
        {
            Pointer cloned;
            if (value is Pointer p)
                cloned = new Pointer(p.Clone(), type);
            else
                cloned = new Pointer(value, type);


            cloned.bannedList = new List<object>(); // start fresh
            return cloned;
        }

        /// Debug-friendly string
        public override string ToString()
        {
            var val = Get();
            return val != null ? val.ToString() : Name;
        }

    }
}

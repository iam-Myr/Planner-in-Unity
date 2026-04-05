using System;
using System.Collections.Generic;
using UnityEngine;

namespace Planning
{
    public class Pointer
    {
        public object value; // Either a concrete value or another Pointer
        public Type type;

        private List<object> bannedList = new List<object>();

        // GLOBAL COUNTER (total pointers ever created)
        private static int globalCounter = 0;

        // Clone counters per originId
        private static Dictionary<int, int> cloneCounters = new Dictionary<int, int>();

        // Identity fields
        private int originId;   // lineage id (never changes)
        private int cloneId;    // clone number within lineage
        private int globalId;   // unique instance id

        public string Name { get; private set; }

        // ========================
        // Constructors
        // ========================

        public Pointer(Type t)
        {
            this.type = t ?? throw new ArgumentNullException(nameof(t));

            originId = globalCounter;
            cloneId = 0;
            globalId = globalCounter++;

            Name = FormatName();
        }

        public Pointer(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            this.value = value;
            this.type = value.GetType();

            originId = globalCounter;
            cloneId = 0;
            globalId = globalCounter++;

            Name = FormatName();
        }

        public Pointer(object value, Type t)
        {
            if (t == null)
                throw new ArgumentNullException(nameof(t));

            if (value != null && !t.IsInstanceOfType(value))
                throw new ArgumentException($"Value of type {value.GetType().Name} does not match declared type {t.Name}");

            this.value = value;
            this.type = t;

            originId = globalCounter;
            cloneId = 0;
            globalId = globalCounter++;

            Name = FormatName();
        }

        private string FormatName()
        {
            // If value is concrete (not another Pointer) use it as name
            if (value != null && !(value is Pointer))
                return value.ToString();

            return $"?v{originId}-{cloneId}-{globalId}";
        }

        // ========================
        // Value resolution
        // ========================

        public object Get() => Get(new HashSet<Pointer>());

        private object Get(HashSet<Pointer> visited)
        {
            if (visited.Contains(this))
                throw new InvalidOperationException("Cycle detected in pointer chain.");

            visited.Add(this);

            return value is Pointer p ? p.Get(visited) : value;
        }

        public T Get<T>()
        {
            object val = Get();
            if (val == null) return default;

            if (val is T tVal) return tVal;

            throw new InvalidCastException($"Cannot cast value of type {val.GetType().Name} to {typeof(T).Name}");
        }

        public Type GetDeclaredType() => type;

        // ========================
        // Setting / Binding
        // ========================

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

        public void BindTo(object other)
        {
            if (other is Pointer p)
                BindTo(p);
            else
                Set(other);
        }

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
                throw new InvalidOperationException(
                    $"Conflict during unification: values differ. " +
                    $"This value: {this.Get()}, Other value: {other.Get()}"
                );
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

        // ========================
        // Ban logic
        // ========================

        public void Ban(IEnumerable<object> objects)
        {
            foreach (object o in objects)
                bannedList.Add(o);
        }

        public bool IsBanned(object o)
        {
            foreach (object b in bannedList)
            {
                if (b is Pointer p)
                {
                    var val = p.Get();
                    if (val != null && object.Equals(o, val))
                        return true;

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

        // ========================
        // State checks
        // ========================

        public bool IsBound() => Get() != null;

        public bool IsPointer() => value is Pointer;

        public bool isSameValue(Pointer p) =>
            object.Equals(this.Get(), p?.Get());

        // ========================
        // Clone (KEY PART)
        // ========================

        public Pointer Clone(Dictionary<Pointer, Pointer> map)
        {
            if (map.ContainsKey(this))
                return map[this];

            // Ensure lineage counter exists
            if (!cloneCounters.ContainsKey(originId))
                cloneCounters[originId] = 0;

            int newCloneId = ++cloneCounters[originId];

            Pointer cloned = new Pointer(type)
            {
                originId = this.originId,
                cloneId = newCloneId,
                globalId = globalCounter++
            };

            cloned.Name = cloned.FormatName();

            map[this] = cloned;

            // Clone value
            if (value is Pointer p)
                cloned.value = p.Clone(map);
            else
                cloned.value = value;

            // Clone banned list
            cloned.bannedList = new List<object>();
            foreach (var b in bannedList)
            {
                if (b is Pointer bp)
                    cloned.bannedList.Add(bp.Clone(map));
                else
                    cloned.bannedList.Add(b);
            }

            return cloned;
        }

        // ========================
        // Debug
        // ========================

        public override string ToString()
        {
            var val = Get();
            return val != null ? val.ToString() : Name;
        }

    }
}
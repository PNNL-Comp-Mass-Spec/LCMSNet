﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LcmsNetCommonControls.Controls.CueBannerOverlay
{
    /// <summary>
    /// A generic dictionary, which allows both its keys and values
    /// to be garbage collected if there are no other references
    /// to them than from the dictionary itself.
    /// </summary>
    ///
    /// <remarks>
    /// If either the key or value of a particular entry in the dictionary
    /// has been collected, then both the key and value become effectively
    /// unreachable. However, left-over WeakReference objects for the key
    /// and value will physically remain in the dictionary until
    /// RemoveCollectedEntries is called. This will lead to a discrepancy
    /// between the Count property and the number of iterations required
    /// to visit all of the elements of the dictionary using its
    /// enumerator or those of the Keys and Values collections. Similarly,
    /// CopyTo will copy fewer than Count elements in this situation.
    /// </remarks>
    /// <remarks>Copied from https://blogs.msdn.microsoft.com/nicholg/2006/06/04/presenting-weakdictionarytkey-tvalue/ </remarks>
    public sealed class WeakDictionary<TKey, TValue> : BaseDictionary<TKey, TValue>
        where TKey : class
        where TValue : class
    {

        private Dictionary<object, WeakReference2<TValue>> dictionary;
        private WeakKeyComparer<TKey> comparer;

        /// <summary>
        /// Constructor
        /// </summary>
        public WeakDictionary()
            : this(0, null) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="capacity"></param>
        public WeakDictionary(int capacity)
            : this(capacity, null) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="comparer"></param>
        public WeakDictionary(IEqualityComparer<TKey> comparer)
            : this(0, comparer) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="comparer"></param>
        public WeakDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            this.comparer = new WeakKeyComparer<TKey>(comparer);
            this.dictionary = new Dictionary<object, WeakReference2<TValue>>(capacity, this.comparer);
        }

        /// <inheritdoc />
        /// <remarks>
        /// WARNING: The count returned here may include entries for which
        /// either the key or value objects have already been garbage
        /// collected. Call RemoveCollectedEntries to weed out collected
        /// entries and update the count accordingly.</remarks>
        public override int Count
        {
            get { return this.dictionary.Count; }
        }

        /// <inheritdoc />
        public override void Add(TKey key, TValue value)
        {


            if (key == null) throw new ArgumentNullException("key");
            WeakReference2<TKey> weakKey = new WeakKeyReference<TKey>(key, this.comparer);
            WeakReference2<TValue> weakValue = WeakReference2<TValue>.Create(value);
            this.dictionary.Add(weakKey, weakValue);
        }

        /// <inheritdoc />
        public override bool ContainsKey(TKey key)
        {
            return this.dictionary.ContainsKey(key);
        }

        /// <inheritdoc />
        public override bool Remove(TKey key)
        {
            return this.dictionary.Remove(key);
        }

        /// <inheritdoc />
        public override bool TryGetValue(TKey key, out TValue value)
        {
            WeakReference2<TValue> weakValue;
            if (this.dictionary.TryGetValue(key, out weakValue))
            {
                value = weakValue.Target;
                return weakValue.IsAlive;
            }
            value = null;
            return false;
        }

        /// <inheritdoc />
        protected override void SetValue(TKey key, TValue value)
        {
            WeakReference2<TKey> weakKey = new WeakKeyReference<TKey>(key, this.comparer);
            this.dictionary[weakKey] = WeakReference2<TValue>.Create(value);
        }

        /// <inheritdoc />
        public override void Clear()
        {
            this.dictionary.Clear();
        }

        /// <inheritdoc />
        public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (KeyValuePair<object, WeakReference2<TValue>> kvp in this.dictionary)
            {
                WeakReference2<TKey> weakKey = (WeakReference2<TKey>)(kvp.Key);
                WeakReference2<TValue> weakValue = kvp.Value;
                TKey key = weakKey.Target;
                TValue value = weakValue.Target;
                if (weakKey.IsAlive && weakValue.IsAlive)
                    yield return new KeyValuePair<TKey, TValue>(key, value);
            }
        }

        /// <summary>
        /// Removes the left-over weak references for entries in the dictionary
        /// whose key or value has already been reclaimed by the garbage
        /// collector. This will reduce the dictionary's Count by the number
        /// of dead key-value pairs that were eliminated.
        /// </summary>
        public void RemoveCollectedEntries()
        {
            List<object> toRemove = null;
            foreach (KeyValuePair<object, WeakReference2<TValue>> pair in this.dictionary)
            {
                WeakReference2<TKey> weakKey = (WeakReference2<TKey>)(pair.Key);
                WeakReference2<TValue> weakValue = pair.Value;

                if (!weakKey.IsAlive || !weakValue.IsAlive)
                {
                    if (toRemove == null)
                        toRemove = new List<object>();
                    toRemove.Add(weakKey);
                }
            }

            if (toRemove != null)
            {
                foreach (object key in toRemove)
                    this.dictionary.Remove(key);
            }
        }
    }

    /// <summary>
    /// Adds strong typing to WeakReference.Target using generics. Also,
    /// the Create factory method is used in place of a constructor
    /// to handle the case where target is null, but we want the
    /// reference to still appear to be alive.
    /// </summary>
    internal class WeakReference2<T> : WeakReference where T : class
    {
        public static WeakReference2<T> Create(T target)
        {
            if (target == null)
                return WeakNullReference<T>.Singleton;

            return new WeakReference2<T>(target);
        }

        protected WeakReference2(T target)
            : base(target, false) { }

        public new T Target
        {
            get { return (T)base.Target; }
        }
    }

    /// <summary>
    /// Provides a weak reference to a null target object, which, unlike
    /// other weak references, is always considered to be alive. This
    /// facilitates handling null dictionary values, which are perfectly
    /// legal.
    /// </summary>
    internal class WeakNullReference<T> : WeakReference2<T> where T : class
    {
        public static readonly WeakNullReference<T> Singleton = new WeakNullReference<T>();

        private WeakNullReference() : base(null) { }

        public override bool IsAlive
        {
            get { return true; }
        }
    }

    /// <summary>
    /// Provides a weak reference to an object of the given type to be used in
    /// a WeakDictionary along with the given comparer.
    /// </summary>
    internal sealed class WeakKeyReference<T> : WeakReference2<T> where T : class
    {
        public readonly int HashCode;

        public WeakKeyReference(T key, WeakKeyComparer<T> comparer)
            : base(key)
        {
            // retain the object's hash code immediately so that even
            // if the target is GC'ed we will be able to find and
            // remove the dead weak reference.
            this.HashCode = comparer.GetHashCode(key);
        }
    }

    /// <summary>
    /// Compares objects of the given type or WeakKeyReferences to them
    /// for equality based on the given comparer. Note that we can only
    /// implement IEqualityComparer&lt;T&gt; for T = object as there is no
    /// other common base between T and WeakKeyReference&lt;T&gt;. We need a
    /// single comparer to handle both types because we don't want to
    /// allocate a new weak reference for every lookup.
    /// </summary>
    internal sealed class WeakKeyComparer<T> : IEqualityComparer<object>
        where T : class
    {

        private IEqualityComparer<T> comparer;

        internal WeakKeyComparer(IEqualityComparer<T> comparer)
        {
            if (comparer == null)
                comparer = EqualityComparer<T>.Default;

            this.comparer = comparer;
        }

        public int GetHashCode(object obj)
        {
            WeakKeyReference<T> weakKey = obj as WeakKeyReference<T>;
            if (weakKey != null) return weakKey.HashCode;
            return this.comparer.GetHashCode((T)obj);
        }

        // Note: There are actually 9 cases to handle here.
        //
        //  Let Wa = Alive Weak Reference
        //  Let Wd = Dead Weak Reference
        //  Let S  = Strong Reference
        //
        //  x  | y  | Equals(x,y)
        // -------------------------------------------------
        //  Wa | Wa | comparer.Equals(x.Target, y.Target)
        //  Wa | Wd | false
        //  Wa | S  | comparer.Equals(x.Target, y)
        //  Wd | Wa | false
        //  Wd | Wd | x == y
        //  Wd | S  | false
        //  S  | Wa | comparer.Equals(x, y.Target)
        //  S  | Wd | false
        //  S  | S  | comparer.Equals(x, y)
        // -------------------------------------------------
        public new bool Equals(object x, object y)
        {
            bool xIsDead, yIsDead;
            T first = GetTarget(x, out xIsDead);
            T second = GetTarget(y, out yIsDead);

            if (xIsDead)
                return yIsDead ? x == y : false;

            if (yIsDead)
                return false;

            return this.comparer.Equals(first, second);
        }

        private static T GetTarget(object obj, out bool isDead)
        {
            WeakKeyReference<T> wref = obj as WeakKeyReference<T>;
            T target;
            if (wref != null)
            {
                target = wref.Target;
                isDead = !wref.IsAlive;
            }
            else
            {
                target = (T)obj;
                isDead = false;
            }
            return target;
        }
    }

    /// <summary>
    /// Represents a dictionary mapping keys to values.
    /// </summary>
    /// <remarks>
    /// Provides the plumbing for the portions of IDictionary&lt;TKey,
    /// TValue&gt; which can reasonably be implemented without any
    /// dependency on the underlying representation of the dictionary.
    /// </remarks>
    /// <remarks>Copied from https://blogs.msdn.microsoft.com/nicholg/2006/06/04/implementing-idictionarytkey-tvalue-isnt-trivial/ </remarks>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(PREFIX + "DictionaryDebugView`2" + SUFFIX)]
    public abstract class BaseDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private const string PREFIX = "System.Collections.Generic.Mscorlib_";
        private const string SUFFIX = ",mscorlib,Version=2.0.0.0,Culture=neutral,PublicKeyToken=b77a5c561934e089";

        private KeyCollection keys;
        private ValueCollection values;

        /// <summary>
        /// Constructor
        /// </summary>
        protected BaseDictionary() { }

        /// <inheritdoc />
        public abstract int Count { get; }

        /// <inheritdoc />
        public abstract void Clear();

        /// <inheritdoc />
        public abstract void Add(TKey key, TValue value);

        /// <inheritdoc />
        public abstract bool ContainsKey(TKey key);

        /// <inheritdoc />
        public abstract bool Remove(TKey key);

        /// <inheritdoc />
        public abstract bool TryGetValue(TKey key, out TValue value);

        /// <inheritdoc />
        public abstract IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();

        /// <summary>
        /// Set the value of the key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected abstract void SetValue(TKey key, TValue value);

        /// <inheritdoc />
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <inheritdoc />
        public ICollection<TKey> Keys
        {
            get
            {
                if (this.keys == null)
                    this.keys = new KeyCollection(this);

                return this.keys;
            }
        }

        /// <inheritdoc />
        public ICollection<TValue> Values
        {
            get
            {
                if (this.values == null)
                    this.values = new ValueCollection(this);

                return this.values;
            }
        }

        /// <inheritdoc />
        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (!this.TryGetValue(key, out value))
                    throw new KeyNotFoundException();

                return value;
            }
            set
            {
                SetValue(key, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue value;
            if (!this.TryGetValue(item.Key, out value))
                return false;

            return EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Copy(this, array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!this.Contains(item))
                return false;


            return this.Remove(item.Key);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private abstract class Collection<T> : ICollection<T>
        {
            protected readonly IDictionary<TKey, TValue> dictionary;

            protected Collection(IDictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary;
            }

            public int Count
            {
                get { return this.dictionary.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                Copy(this, array, arrayIndex);
            }

            public virtual bool Contains(T item)
            {
                foreach (T element in this)
                    if (EqualityComparer<T>.Default.Equals(element, item))
                        return true;
                return false;
            }

            public IEnumerator<T> GetEnumerator()
            {
                foreach (KeyValuePair<TKey, TValue> pair in this.dictionary)
                    yield return GetItem(pair);
            }

            protected abstract T GetItem(KeyValuePair<TKey, TValue> pair);

            public bool Remove(T item)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public void Add(T item)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public void Clear()
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        [DebuggerDisplay("Count = {Count}")]
        [DebuggerTypeProxy(PREFIX + "DictionaryKeyCollectionDebugView`2" + SUFFIX)]
        private class KeyCollection : Collection<TKey>
        {
            public KeyCollection(IDictionary<TKey, TValue> dictionary)
                : base(dictionary) { }

            protected override TKey GetItem(KeyValuePair<TKey, TValue> pair)
            {
                return pair.Key;
            }
            public override bool Contains(TKey item)
            {
                return this.dictionary.ContainsKey(item);
            }
        }

        [DebuggerDisplay("Count = {Count}")]
        [DebuggerTypeProxy(PREFIX + "DictionaryValueCollectionDebugView`2" + SUFFIX)]
        private class ValueCollection : Collection<TValue>
        {
            public ValueCollection(IDictionary<TKey, TValue> dictionary)
                : base(dictionary) { }

            protected override TValue GetItem(KeyValuePair<TKey, TValue> pair)
            {
                return pair.Value;
            }
        }

        private static void Copy<T>(ICollection<T> source, T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException("arrayIndex");

            if ((array.Length - arrayIndex) < source.Count)
                throw new ArgumentException("Destination array is not large enough. Check array.Length and arrayIndex.");

            foreach (T item in source)
                array[arrayIndex++] = item;
        }
    }
}
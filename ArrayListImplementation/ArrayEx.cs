using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrayListImplementation
{
    public class ArrayEx<T>
    {
        // Fields
        private const int GROW_BY = 10;
        private int m_count;
        private T[] m_data;
        private int m_updateCode;


        // Constructors

        /// <summary>
        /// Initializes a new instance of the ArrayEx(T) class that is empty.
        /// </summary>
        public ArrayEx()
        {
            Initialize(GROW_BY);
        }

        /// <summary>
        /// Initializes a new instance of the ArrayEx(T) class that is empty and has the specified initiali capacity
        /// </summary>
        /// <param name="capacity">The number of elements that the new array can initially store.</param>
        public ArrayEx(int capacity)
        {
            Initialize(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the ArrayEx(T) class that contains the items in the array.
        /// </summary>
        /// <param name="items">Adds the items to the ArrayEx(T).</param>
        public ArrayEx(IEnumerable<T> items)
        {
            Initialize(GROW_BY);

            foreach (T item in items)
            {
                Add(item);
            }
        }

        // Methods

        /// <summary>
        /// Initializes a new instance of the ArrayEx(T) class that is empty.
        /// </summary>
        private void Initialize(int capacity)
        {
            m_data = new T[capacity];
        }

        /// <summary>
        /// Adds an object to the end of the ArrayEx(T).
        /// </summary>
        /// <param name="item">The item to add to the end of the ArrayEx(T).</param>
        public void Add(T item)
        {
            if (m_data.Length <= m_count)
            {
                Capacity += GROW_BY;
            }

            // We will need to assign the item to the last element and then increment the count variable
            m_data[m_count++] = item;
            ++m_updateCode;
        }

        /// <summary>
        /// Inserts an item into the ArrayEx(T) at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The item to insert. A value of null will cause an exception later.</param>
        public void Insert(int index, T item)
        {
            if (index < 0 || index >= m_count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (m_count + 1 >= Capacity)
            {
                Capacity = m_count + GROW_BY;
            }

            // First we need to shift all elements at the location up by one
            for (int i = m_count; i >= index && i > 0; --i)
            {
                m_data[i] = m_data[i - 1];
            }

            m_data[index] = item;

            ++m_count;
            ++m_updateCode;
        }

        /// <summary>
        /// Removes the item located at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to remove</param>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= m_count)
            {
                // Item has already been removed.
                return;
            }

            int count = Count;

            // Shift all of the elements after the specified index down one.
            for (int i = index + 1; i <= count; ++i)
            {
                m_data[i - 1] = m_data[i];
            }

            // Decrement the count to reflect the item being removed.
            --m_count;
            ++m_updateCode;

            m_data[m_count] = default(T);
        }

        /// <summary>
        /// Removes the first occurence of the specified item from the ArrayEx(T).
        /// </summary>
        /// <param name="item">The item to remove from the ArrayEx(T).</param>
        /// <returns>True if an item was removed, false otherwise.</returns>
        public bool Remove(T item)
        {
            return Remove(item, false);
        }

        /// <summary>
        /// Removes the first or all occurences of the specified item from the ArrayEx(T).
        /// </summary>
        /// <param name="item">The item to remove from the ArrayEx(T)</param>
        /// <param name="allOccurrences">True if all occurences of the item should be removed, False if only the first</param>
        /// <returns>True if an item was removed, false otherwise</returns>
        public bool Remove(T item, bool allOccurrences)
        {
            int shiftto = 0;
            bool shiftmode = false;
            bool removed = false;

            int count = m_count;
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;

            for (int i = 0; i < count; ++i)
            {
                if (comparer.Equals(m_data[i], item) && (allOccurrences || !shiftmode))
                {
                    // Decrement the count since we have found an instance
                    --m_count;
                    removed = true;

                    // Check to see if we have already found one occurence of the item we are removing
                    if (!shiftmode)
                    {
                        // We will start shifting to the position of the first occurence.
                        shiftto = i;
                        // Enable shifting
                        shiftmode = true;
                    }
                    continue;
                }

                if (shiftmode)
                {
                    // Since we are shifting elements we need to shift the element down and then update the shiftto index to the next element.
                    m_data[shiftto++] = m_data[i];
                }
            }

            for (int i = m_count; i < count; ++i)
            {
                m_data[i] = default(T);
            }

            if (removed)
            {
                ++m_updateCode;
            }

            return removed;
        }

        /// <summary>
        /// Clears all values from the ArrayEx(T)
        /// </summary>
        public void Clear()
        {
            Array.Clear(m_data, 0, m_count);
            m_count = 0;
            ++m_updateCode;
        }

        /// <summary>
        /// Checks to see if the item is present in the ArrayEx(T)
        /// </summary>
        /// <param name="item">The item to see if the array contains.</param>
        /// <returns>True if the item is in the array, false if it is not.</returns>
        public bool Contains(T item)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < m_count; i++)
            {
                if (comparer.Equals(m_data[i], item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the index of the specified item.
        /// </summary>
        /// <param name="item">The item to get the index of.</param>
        /// <returns>-1 if the item isn't found in the array, the index of the first instance of the item otherwise.</returns>
        public int IndexOf(T item)
        {
            return Array.IndexOf<T>(m_data, item, 0, m_count);
        }

        /// <summary>
        /// Copies the elements of the ArrayEx<T> to a new array.
        /// </summary>
        /// <returns>An array containing copies of the elements of the ArrayEx<T></returns>
        public T[] ToArray()
        {
            T[] tmp = new T[Count];

            for (int i = 0; i < Count; ++i)
            {
                tmp[i] = m_data[i];
            }

            return tmp;
        }

        // Properties

        /// <summary>
        /// Gets or sets the size of the internal data array.
        /// </summary>
        public int Capacity
        {
            get { return m_data.Length; }
            set
            {
                // We do not support truncating the stored data
                // So throw an exceptin if the array is less than Count.
                if (value < Count)
                {
                    throw new ArgumentOutOfRangeException("value", "The value is less than Count");
                }

                // We do not need to do anything if the newly specified capacity is the same as the old one.
                if (value == Capacity)
                {
                    return;
                }

                // We will need to create a new array and move all of the values in the old array to the new one
                T[] tmp = new T[value];
                for (int i = 0; i < Count; ++i)
                {
                    tmp[i] = m_data[i];
                }

                m_data = tmp;
                ++m_updateCode;
            }
        }

        /// <summary>
        /// Gets the number of elements actually contained in the ArrayEx(T).
        /// </summary>
        public int Count
        {
            get { return m_count; }
        }

        /// <summary>
        /// States if the ArrayEx(T) is empty
        /// </summary>
        public bool IsEmpty
        {
            get { return m_count <= 0; }
        }
        /// <summary>
        /// Gets or sets an element in the ArrayEx(T).
        /// </summary>
        /// <param name="index">The index of the element.</param>
        /// <returns>The value of the element.</returns>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= m_count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                return m_data[index];
            }
            set
            {
                if (index < 0 || index >= m_count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                m_data[index] = value;
                ++m_updateCode;
            }
        }
    }
}

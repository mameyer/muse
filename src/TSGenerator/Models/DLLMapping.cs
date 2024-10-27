using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TSGenerator.Models
{
    public class DLLMapping : IDictionary<string, ModelFileDefinition[]>
    {
        private readonly Dictionary<string, ModelFileDefinition[]> _data;

        public DLLMapping(IGrouping<string, ModelFileDefinition>[] data)
        {
            this._data = data?.ToDictionary(e => e.Key, f => f.ToArray()) ?? new Dictionary<string, ModelFileDefinition[]>();
        }

        public ModelFileDefinition[] this[string key] { get => this._data[key]; set => this._data[key] = value; }

        public ICollection<string> Keys => this._data.Keys;

        public ICollection<ModelFileDefinition[]> Values => this._data.Values;

        public int Count => this._data.Count;

        public bool IsReadOnly => false;

        public void Add(string key, ModelFileDefinition[] value)
        {
            this[key] = value;
        }

        public void Add(KeyValuePair<string, ModelFileDefinition[]> item)
        {
            this[item.Key] = item.Value;
        }

        public void Clear()
        {
            this._data.Clear();
        }

        public bool Contains(KeyValuePair<string, ModelFileDefinition[]> item)
            => this.ContainsKey(item.Key);

        public bool ContainsKey(string key)
             => this._data.ContainsKey(key);

        public void CopyTo(KeyValuePair<string, ModelFileDefinition[]>[] array, int arrayIndex)
        {
            if (this.Count == 0) return;

            int idx = 0;
            foreach (var item in this._data)
            {
                if (idx < arrayIndex) continue;
                array[idx++] = item;
            }
        }

        public IEnumerator<KeyValuePair<string, ModelFileDefinition[]>> GetEnumerator()
            => this._data.GetEnumerator();

        public bool Remove(string key)
            => this._data.Remove(key);

        public bool Remove(KeyValuePair<string, ModelFileDefinition[]> item)
            => this.Remove(item.Key);

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out ModelFileDefinition[] value)
            => this.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}

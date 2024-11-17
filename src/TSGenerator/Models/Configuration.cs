using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TSGenerator.Models
{
    public class Configuration : IEnumerable<BuildConfiguration>
    {
        private readonly BuildConfiguration[] _data;

        public Configuration(FileInfo file)
        {
            var items = new List<BuildConfiguration>();

            string jsonString = System.IO.File.ReadAllText(file.FullName);
            try
            {
                var jsonDocument = JsonDocument.Parse(jsonString);
                var configItems = jsonDocument.RootElement.EnumerateArray();
                foreach (var item in configItems)
                {
                    items.Add(item.Deserialize<BuildConfiguration>());
                }
            }
            catch { }

            this._data = items.ToArray();
        }

        public IEnumerator<BuildConfiguration> GetEnumerator()
        {
            foreach (var item in _data)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}

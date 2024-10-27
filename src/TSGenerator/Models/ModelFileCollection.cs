using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TSGenerator.Models
{
    public class ModelFileCollection : IEnumerable<ModelFile>
    {
        private readonly List<ModelFile> _data;

        public ModelFileCollection()
        {
            this._data = new List<ModelFile>();
        }

        public IEnumerator<ModelFile> GetEnumerator()
        {
            foreach (var item in this._data)
            {
                yield return item;
            }
        }

        public void Add(ModelFile modelFile)
        {
            this._data.Add(modelFile);
        }

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();

        public DLLMapping GetDLLMapping()
            => new(this._data
                .Where(e => e.Dlls != null)
                .SelectMany(e => e.Dlls)
                .GroupBy(e => e.DllPath)
                .ToArray());
    }
}

namespace Muse.Models
{
    public class TestBuilder
    {
        private int _id;
        private string _name;

        public TestBuilder SetId(int id)
        {
            this._id = id;
            return this;
        }

        public TestBuilder SetName(string name)
        {
            this._name = name;
            return this;
        }
    }

    public class BuilderA : TestBuilder
    {
        public BuilderA()
        {
            this.SetId(1);
            this.SetName("Test");
        }
    }
}
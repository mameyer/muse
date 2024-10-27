namespace TSGenerator.Models
{
    public class BuildSource : ConfigurationBaseItem
    {
        public string Project { get; set; }

        public override string ToString()
            => $"{this.Project}: {this.Path}";
    }
}

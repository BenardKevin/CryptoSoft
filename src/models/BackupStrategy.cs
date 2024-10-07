namespace prosoft_easysave.src.models
{
    public enum TypeOfSave
    {
        COMPLETE,
        DIFFERENTIAL
    }

    public class BackupStrategy
    {
        public int Id { get; set; }
        public string CreationDate { get; set; }
        public string Name { get; set; }
        public TypeOfSave TypeOfSave { get; set; }
        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }
    }
}
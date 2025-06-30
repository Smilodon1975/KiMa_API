namespace KiMa_API.Models.Dto
{
    public class QuestionDefinitionDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = null!;
        public string Type { get; set; } = null!;

        // nur nötig, wenn Du Checkbox-/Grid-Fragen auswerten willst
        public List<OptionDto>? Options { get; set; }
        public List<RowDto>? Rows { get; set; }
    }

    public class OptionDto
    {
        public string Label { get; set; } = null!;
        public string Value { get; set; } = null!;
        public bool Exclude { get; set; }
    }

    public class RowDto
    {
        public string Label { get; set; } = null!;
        public string Value { get; set; } = null!;
        public bool Exclude { get; set; }
    }
}

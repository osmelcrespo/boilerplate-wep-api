namespace Core.Models
{
    public class ResponseData
    {
        public bool Error { get; set; }
        public int ErrorValue { get; set; }
        public string Description { get; set; }
        public object Data { get; set; }
        public object OthersValidations { get; set; }

        public ResponseData()
        {
            Error = false;
            Description = "Saved!";
        }
    }
}

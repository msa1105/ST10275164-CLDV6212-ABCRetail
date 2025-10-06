namespace ST10275164_CLDV6212_POE.Models
{
    public class ContractViewModel
    {
        public string FileName { get; set; }
        public string FileUrl { get; set; }

        public List<string> ContractUris { get; set; } = new List<string>();
    }
}
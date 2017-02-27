namespace LogApi.Models
{
    /// <summary>
    /// Register application result model
    /// </summary>
    public class RegisterAppResult
    {
        public string display_name { get; set; }
        public string application_id { get; set; }
        public string secret { get; set; }
    }
}
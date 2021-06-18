namespace Sushi.Mediakiwi.Headless.BasicAuthentication
{
    public class Credential
    {
        public string Password { get; set; }
        public string Username { get; set; }
        /// <summary>
        /// The string match 
        /// </summary>
        public string Match { get; set; } = "azurewebsites.net";
    }
}

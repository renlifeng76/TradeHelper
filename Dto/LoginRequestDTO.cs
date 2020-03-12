using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace TradeHelper.Dto
{
    public class LoginRequestDTO
    {
        [Required]
        [JsonProperty("username")]
        public string Username { get; set; }


        [Required]
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}

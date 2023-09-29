using System.Net;

namespace MagicVillaApi.Models
{
    public class APIResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccessful { get; set; } = true;
        public List<string> ErrorMesgges { get; set; }
        public object Response { get; set; }
    }
}

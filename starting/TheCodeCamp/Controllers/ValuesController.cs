using System.Web.Http;

namespace TheCodeCamp.Controllers
{
  public class ValuesController : ApiController
  {
    public string[] Get()
    {
      return new[] { "Hello", "From", "Pluralsight" };
    }

  }
}

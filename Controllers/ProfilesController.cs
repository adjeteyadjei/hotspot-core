using Hotvenues.Services;

namespace Hotvenues.Controllers
{
    public class ProfilesController : BaseApi<IProfileService, ProfileDto>
    {
        public ProfilesController(IProfileService service) : base(service) { }
    }
}
using System.Threading.Tasks;
using BTCPayServer.Client.Models;
using BTCPayServer.Data;
using BTCPayServer.Security;
using BTCPayServer.Security.APIKeys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BTCPayServer.Controllers.RestApi.ApiKeys
{
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.ApiKey)]
    public class ApiKeysController : ControllerBase
    {
        private readonly APIKeyRepository _apiKeyRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApiKeysController(APIKeyRepository apiKeyRepository, UserManager<ApplicationUser> userManager)
        {
            _apiKeyRepository = apiKeyRepository;
            _userManager = userManager;
        }
    
        [HttpGet("~/api/v1/api-keys/current")]
        public async Task<ActionResult<ApiKeyData>> GetKey()
        {
            ControllerContext.HttpContext.GetAPIKey(out var apiKey);
            var data = await _apiKeyRepository.GetKey(apiKey);
            return Ok(FromModel(data));
        }

        [HttpDelete("~/api/v1/api-keys/current")]
        public async Task<ActionResult<ApiKeyData>> RevokeKey()
        {
            ControllerContext.HttpContext.GetAPIKey(out var apiKey);
            await _apiKeyRepository.Remove(apiKey, _userManager.GetUserId(User));
            return Ok();
        }
        
        private static ApiKeyData FromModel(APIKeyData data)
        {
            return new ApiKeyData()
            {
                Permissions = data.GetPermissions(),
                ApiKey = data.Id,
                UserId = data.UserId,
                Label = data.Label
            };
        }
    }
}

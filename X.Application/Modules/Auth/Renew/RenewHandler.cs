

using X.Application.Interfaces;
using X.Application.Models;
using X.Application.Modules.Auth.LogIn;
using X.Application.Modules.Auth.Renew;
using X.Shared.Helpers;
using X.Shared.Responses;

public class RenewHandler (
    ICacheService cacheService,
    IToken tokenService
)
{
    public async Task<GenericResponse<UserLogInResponse>> Execute(RenewAuthCommand command)
    {

        var findRefreshToken = cacheService.Get<RefreshToken>(CacheHelper.AuthRefreshTokenKey(model.RefreshToken))
                        ?? throw new NotFoundException(ResponseConstants.AUTH_REFRESH_TOKEN_NOT_FOUND);

        var token = TokenHelper.Create(findRefreshToken.CollaboratorId, configuration, cacheService);
        var refreshToken = TokenHelper.CreateRefresh(findRefreshToken.CollaboratorId, configuration, cacheService);

        cacheService.Delete(CacheHelper.AuthRefreshTokenKey(model.RefreshToken));

        return ResponseHelper.Create(new LoginAuthResponse
        {
            Token = token,
            RefreshToken = refreshToken
        });
    }
}
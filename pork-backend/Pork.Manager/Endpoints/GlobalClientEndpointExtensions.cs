using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pork.Manager.Dtos;
using Pork.Shared;

namespace Pork.Manager.Endpoints;

public static class GlobalClientEndpointExtensions {
    public static void MapGlobalClientEndpoints(this WebApplication app) {
        app.MapGroup("/clients/global/{globalClientId}")
            .MapSetNickname();
    }

    private static IEndpointRouteBuilder MapSetNickname(this IEndpointRouteBuilder group) {
        group.MapPut("/nickname",
                async ([FromServices] DataContext dataContext, Guid globalClientId,
                        [FromBody] SetNicknameRequestDto request)
                    => {
                    var client = await dataContext.GlobalClients.FirstOrDefaultAsync(e =>
                        e.Id == globalClientId);
                    if (client is null) {
                        return Results.NotFound();
                    }

                    client.Nickname = request.Nickname;
                    await dataContext.SaveChangesAsync();
                    return Results.Ok();
                })
            .WithName("SetClientNickname");
        return group;
    }
}
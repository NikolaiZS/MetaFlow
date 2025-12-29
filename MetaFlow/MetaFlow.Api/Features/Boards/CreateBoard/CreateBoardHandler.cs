using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Boards;
using MetaFlow.Domain.Entities;
using MetaFlow.Domain.Models;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Boards.CreateBoard;

public class CreateBoardHandler : IRequestHandler<CreateBoardCommand, Result<BoardResponse>>
{
    private readonly SupabaseService _supabaseService;

    public CreateBoardHandler(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
    }

    public async Task<Result<BoardResponse>> Handle(
        CreateBoardCommand request,
        CancellationToken cancellationToken)
    {
        var client = _supabaseService.GetClient();

        var methodology = await client
            .From<MethodologyPreset>()
            .Select("id,name,display_name,description,icon,category,is_system,is_active,created_by,created_at,updated_at")
            .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.MethodologyPresetId.ToString())
            .Single();

        if (methodology == null)
        {
            return Result.Failure<BoardResponse>("Methodology preset not found");
        }

        var user = await client
            .From<User>()
            .Select("id,email,username,full_name,avatar_url,email_verified,last_login_at,created_at,updated_at")
            .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.UserId.ToString())
            .Single();

        if (user == null)
        {
            return Result.Failure<BoardResponse>("User not found");
        }

        var board = new Board
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            OwnerId = request.UserId,
            MethodologyPresetId = request.MethodologyPresetId,
            CustomConfig = new BoardConfig
            {
                Theme = "light",
                ShowArchived = false,
                DefaultColumnLimit = 0,
                CustomFields = new List<string>(),
                ColorScheme = new Dictionary<string, string>()
            },
            IsPublic = request.IsPublic,
            IsTemplate = request.IsTemplate,
            IsArchived = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var insertResponse = await client
            .From<Board>()
            .Insert(board);

        var createdBoard = insertResponse.Models.FirstOrDefault();

        if (createdBoard == null)
        {
            return Result.Failure<BoardResponse>("Failed to create board");
        }

        var response = new BoardResponse(
            createdBoard.Id,
            createdBoard.Name,
            createdBoard.Description,
            createdBoard.OwnerId,
            user.Username,
            createdBoard.MethodologyPresetId,
            methodology.DisplayName,
            createdBoard.IsPublic,
            createdBoard.IsTemplate,
            createdBoard.IsArchived,
            createdBoard.CreatedAt,
            createdBoard.UpdatedAt
        );

        return Result.Success(response);
    }
}

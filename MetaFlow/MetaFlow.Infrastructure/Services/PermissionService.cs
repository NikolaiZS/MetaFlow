using MetaFlow.Domain.Entities;

namespace MetaFlow.Infrastructure.Services
{
    public class PermissionService
    {
        private readonly SupabaseService _supabaseService;

        public PermissionService(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<bool> CanManageBoard(Guid boardId, Guid userId)
        {
            var client = _supabaseService.GetClient();

            var board = await client
                .From<Board>()
                .Select("id,owner_id")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, boardId.ToString())
                .Single();

            if (board?.OwnerId == userId)
            {
                return true;
            }

            var member = await client
                .From<BoardMember>()
                .Select("id,role")
                .Filter("board_id", Supabase.Postgrest.Constants.Operator.Equals, boardId.ToString())
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Single();

            return member?.Role == "admin";
        }

        public async Task<bool> CanCreateCards(Guid boardId, Guid userId)
        {
            var client = _supabaseService.GetClient();

            var board = await client
                .From<Board>()
                .Select("id,owner_id")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, boardId.ToString())
                .Single();

            if (board?.OwnerId == userId)
            {
                return true;
            }

            var member = await client
                .From<BoardMember>()
                .Select("id,role")
                .Filter("board_id", Supabase.Postgrest.Constants.Operator.Equals, boardId.ToString())
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Single();

            return member != null && member.Role != "viewer";
        }

        public async Task<bool> CanEditCards(Guid boardId, Guid userId)
        {
            return await CanCreateCards(boardId, userId);
        }

        public async Task<bool> CanDeleteCards(Guid boardId, Guid userId)
        {
            var client = _supabaseService.GetClient();

            var board = await client
                .From<Board>()
                .Select("id,owner_id")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, boardId.ToString())
                .Single();

            if (board?.OwnerId == userId)
            {
                return true;
            }

            var member = await client
                .From<BoardMember>()
                .Select("id,role")
                .Filter("board_id", Supabase.Postgrest.Constants.Operator.Equals, boardId.ToString())
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Single();

            return member != null && (member.Role == "admin");
        }

        public async Task<bool> HasBoardAccess(Guid boardId, Guid userId)
        {
            var client = _supabaseService.GetClient();

            var board = await client
                .From<Board>()
                .Select("id,owner_id,is_public")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, boardId.ToString())
                .Single();

            if (board == null)
            {
                return false;
            }

            if (board.OwnerId == userId)
            {
                return true;
            }

            if (board.IsPublic)
            {
                return true;
            }

            var member = await client
                .From<BoardMember>()
                .Select("id")
                .Filter("board_id", Supabase.Postgrest.Constants.Operator.Equals, boardId.ToString())
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Single();

            return member != null;
        }
    }
}

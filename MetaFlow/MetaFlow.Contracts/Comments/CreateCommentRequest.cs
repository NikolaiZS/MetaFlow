namespace MetaFlow.Contracts.Comments
{
    public record CreateCommentRequest(
        string Contetnt,
        Guid? ParentCommentId = null
        );
}
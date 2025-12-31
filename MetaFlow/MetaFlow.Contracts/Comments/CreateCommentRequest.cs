namespace MetaFlow.Contracts.Comments
{
    public record CreateCommentRequest(
        string Content,
        Guid? ParentCommentId = null
        );
}
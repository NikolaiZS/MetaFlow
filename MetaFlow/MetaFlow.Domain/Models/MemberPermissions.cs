namespace MetaFlow.Domain.Models
{
    public class MemberPermissions
    {
        public bool CanCreateCards { get; set; } = true;
        public bool CanEditCards { get; set; } = true;
        public bool CanDeleteCards { get; set; } = false;
        public bool CanMoveCards { get; set; } = true;
        public bool CanManageColumns { get; set; } = false;
        public bool CanInviteMembers { get; set; } = false;
        public bool CanManageBoard { get; set; } = false;
        public bool CanArchiveBoard { get; set; } = false;
    }
}
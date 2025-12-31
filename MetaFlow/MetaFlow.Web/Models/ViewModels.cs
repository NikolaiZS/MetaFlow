using System.ComponentModel.DataAnnotations;

namespace MetaFlow.Web.Models;

public class LoginViewModel
{
    [Required]
    public string EmailOrUsername { get; set; } = "";
    [Required]
    public string Password { get; set; } = "";
}

public class RegisterViewModel
{
    [Required]
    public string FullName { get; set; } = "";
    [Required]
    public string Username { get; set; } = "";
    [Required, EmailAddress]
    public string Email { get; set; } = "";
    [Required]
    public string Password { get; set; } = "";
}

public class CreateBoardViewModel
{
    [Required]
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public Guid MethodologyPresetId { get; set; }
    public bool IsPublic { get; set; }
}

public class CreateColumnViewModel
{
    [Required]
    public string Name { get; set; } = "";
}

public class CreateCardViewModel
{
    [Required]
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
}

public class CreateCommentViewModel
{
    [Required]
    public string Content { get; set; } = "";
}

public class UploadAttachmentViewModel
{
    [Required]
    public string FileName { get; set; } = "";
    [Required]
    public string FileUrl { get; set; } = "";
}

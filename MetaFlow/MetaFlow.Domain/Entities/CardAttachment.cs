using MetaFlow.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Domain.Entities
{
    public class CardAttachment : BaseEntity
    {
        public Guid CardId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public long? FileSize { get; set; }
        public string? MimeType { get; set; }
        public string? ThumbnailUrl { get; set; }
        public Guid UploadedById { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public Card Card { get; set; } = null!;
        public User UploadedBy { get; set; } = null!;
    }

}

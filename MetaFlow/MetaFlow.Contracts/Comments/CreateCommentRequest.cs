using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Contracts.Comments
{
    public record CreateCommentRequest(
        string Contetnt,
        Guid? ParentCommentId = null
        );
}

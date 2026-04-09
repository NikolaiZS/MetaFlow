using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Contracts.Tags
{
    public record UpdateTagRequest(
    string? Name,
    string? Color
    );
}

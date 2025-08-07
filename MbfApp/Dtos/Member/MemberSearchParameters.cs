using System;

namespace MbfApp.Dtos.Member;

public class MemberSearchParameters
{
    public int? PageIndex { get; set; }
    public int? PageSize { get; set; }
    public string? SearchTerm { get; set; }
}

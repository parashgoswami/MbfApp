using MbfApp.Dtos.MemberLedgers;

namespace MbfApp.Services;

public class MemberLedgerException : Exception
{
    public List<MemberLedgerErrorDto>? Errors { get; set; }

    public MemberLedgerException(string message, List<MemberLedgerErrorDto> errors = null!) : base(message)
    {
        Errors = errors;
    }
}

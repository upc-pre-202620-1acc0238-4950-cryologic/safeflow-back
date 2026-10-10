using SafeFlow.API.Iam.Domain.Model.Aggregates;
using SafeFlow.API.Iam.Domain.Model.ValueObjects;

namespace SafeFlow.API.Iam.Application.Outbound;

public interface ITokenService
{
    SessionToken Generate(User user);
    ValidatedSession? Validate(string token);
}

using SafeFlow.API.Logistics.Application.Errors;
using SafeFlow.API.Logistics.Domain.Model.Queries;
using SafeFlow.API.Shared.Application.Patterns;

namespace SafeFlow.API.Logistics.Application.Services;

public interface IShipmentQueryService
{
    Task<Result<IReadOnlyList<ShipmentDetails>, LogisticsError>> Handle(GetShipmentsQuery query,
        CancellationToken ct = default);
    Task<Result<ShipmentDetails, LogisticsError>> Handle(GetShipmentByIdQuery query, CancellationToken ct = default);
    Task<Result<ShipmentValidation, LogisticsError>> Handle(GetShipmentValidationQuery query,
        CancellationToken ct = default);
}

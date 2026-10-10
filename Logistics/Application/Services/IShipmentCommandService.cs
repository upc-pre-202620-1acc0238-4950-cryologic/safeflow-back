using SafeFlow.API.Logistics.Application.Errors;
using SafeFlow.API.Logistics.Domain.Model.Aggregates;
using SafeFlow.API.Logistics.Domain.Model.Commands;
using SafeFlow.API.Shared.Application.Patterns;

namespace SafeFlow.API.Logistics.Application.Services;

public interface IShipmentCommandService
{
    Task<Result<Shipment, LogisticsError>> Handle(CreateShipmentCommand command, CancellationToken ct = default);
    Task<Result<Shipment, LogisticsError>> Handle(DepartShipmentCommand command, CancellationToken ct = default);
    Task<Result<Shipment, LogisticsError>> Handle(DeliverShipmentCommand command, CancellationToken ct = default);
}

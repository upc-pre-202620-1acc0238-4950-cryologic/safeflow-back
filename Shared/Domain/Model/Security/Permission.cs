namespace SafeFlow.API.Shared.Domain.Model.Security;

/// <summary>Configurable business operations, mirroring the Android app permissions.</summary>
public enum Permission
{
    WRITE_INVENTORY,
    READ_INVENTORY,
    READ_SHIPMENTS,
    CREATE_SHIPMENT,
    DELIVER,
    SUMMARY,
    RISKS,
    ALERTS,
    RESOLVE
}

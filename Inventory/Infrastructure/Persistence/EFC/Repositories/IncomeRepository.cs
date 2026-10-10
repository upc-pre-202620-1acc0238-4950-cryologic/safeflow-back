using SafeFlow.API.Inventory.Domain.Model.Aggregates;
using SafeFlow.API.Inventory.Domain.Repositories;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SafeFlow.API.Inventory.Infrastructure.Persistence.EFC.Repositories;

public sealed class IncomeRepository(AppDbContext context) : BaseRepository<Income>(context), IIncomeRepository;

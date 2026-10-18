
using Microsoft.EntityFrameworkCore;

namespace TaskFlow.Infrastructure.Persistence;

public sealed class TaskFlowDbContext : DbContext
{
    public TaskFlowDbContext(
        DbContextOptions<TaskFlowDbContext> options)
        : base(options)
    {
    }
}

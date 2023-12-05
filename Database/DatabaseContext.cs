using System.Reflection;
using HydroSales.Authorization;
using HydroSales.Domain;
using Microsoft.EntityFrameworkCore;

namespace HydroSales.Database;

public interface IDatabaseMigrator
{
    Task Migrate();
}

public interface IDatabase
{
    IQueryable<TEntity> Query<TEntity>(string id = null, bool includeRemoved = false, bool ignoreTenant = false) where TEntity : class, IEntity;
    void Remove(IEntity entity);
    Task AddAsync(IEntity entity);
    Task SaveChangesAsync();
    Task<User> GetCurrentUser();
}

public class DatabaseContext(DbContextOptions<DatabaseContext> options, IAuthorizationService authorization) : DbContext(options), IDatabase, IDatabaseMigrator
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("nocase");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<Enum>()
            .HaveConversion<string>()
            .HaveMaxLength(100);
    }

    public async Task<User> GetCurrentUser()
    {
        var userId = authorization.GetCurrentUserId();

        if (userId == null)
        {
            return null;
        }

        return await Query<User>()
            .Include(u => u.Settings)
            .SingleOrDefaultAsync(u => u.Id == userId);
    }

    public IQueryable<TEntity> Query<TEntity>(string id = null, bool includeRemoved = false, bool ignoreTenant = false) where TEntity : class, IEntity
    {
        var query = Set<TEntity>().AsQueryable();

        if (typeof(IRemovable).IsAssignableFrom(typeof(TEntity)) && !includeRemoved)
        {
            query = query.Where(e => !((IRemovable)e).IsRemoved);
        }

        if (typeof(ITenant).IsAssignableFrom(typeof(TEntity)) && !ignoreTenant)
        {
            var currentUserId = authorization.GetCurrentUserId();
            query = query.Where(e => ((ITenant)e).User.Id == currentUserId);
        }

        if (id != null)
        {
            query = query.Where(e => e.Id == id);
        }

        return query;
    }

    public Task<int> ExecuteSqlAsync(FormattableString sqlInterpolated) =>
        Database.ExecuteSqlInterpolatedAsync(sqlInterpolated);

    public async Task AddAsync(IEntity entity) =>
        await AddAsync(entity, CancellationToken.None);

    public void Remove(IEntity entity) =>
        base.Remove(entity);

    public Task Migrate() =>
        Database.MigrateAsync();

    public Task SaveChangesAsync() =>
        base.SaveChangesAsync();
}
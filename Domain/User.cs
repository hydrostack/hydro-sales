using HydroSales.Database;
using HydroSales.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HydroSales.Domain;

public class User : IEntity
{
    private User()
    {
    }

    public string Id { get; private set; }
    public UserSettings Settings { get; set; }

    public static User Create(bool insertTestData = false)
    {
        var user = new User
        {
            Id = IdProvider.NewId(),
            Settings = UserSettings.Create()
        };
        
        return user;
    }
}

public class UserEntity : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).HasMaxLength(36);
    }
}

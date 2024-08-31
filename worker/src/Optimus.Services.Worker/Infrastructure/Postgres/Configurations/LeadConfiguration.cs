using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Optimus.Services.Worker.Cores;

namespace Optimus.Services.Worker.Infrastructure.Postgres.Configurations;

public class LeadConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> builder)
    {
        builder.ToTable("leads");

        builder.HasKey(le => le.Id);

        builder.Property(le => le.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(le => le.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(le => le.VectorKey)
            .HasColumnName("vector_key")
            .IsRequired();
        
        builder.Property(le => le.LeadType)
            .HasColumnName("lead_type")
            .IsRequired()
            .HasConversion(new EnumToStringConverter<LeadType>());

        builder.Property(le => le.Tier)
            .HasColumnName("tier")
            .IsRequired()
            .HasConversion(new EnumToStringConverter<Tier>());

        builder.Property(le => le.CompanyName)
            .HasColumnName("company_name")
            .HasMaxLength(255);

        builder.Property(le => le.ContactName)
            .HasColumnName("contact_name")
            .HasMaxLength(255);

        builder.Property(le => le.Position)
            .HasColumnName("position")
            .HasMaxLength(255);

        builder.Property(le => le.Email)
            .HasColumnName("email")
            .HasMaxLength(255);

        builder.Property(le => le.PhoneNumber)
            .HasColumnName("phone_number")
            .HasMaxLength(255);

        builder.Property(le => le.CompanyPhoneNumber)
            .HasColumnName("company_phone_number")
            .HasMaxLength(255);

        builder.Property(le => le.Volume)
            .HasColumnName("volume")
            .HasMaxLength(255);

        builder.Property(le => le.ModeOfTransportation)
            .HasColumnName("modeOfTransportation")
            .HasMaxLength(255);

        builder.Property(le => le.Competitor)
            .HasColumnName("competitor")
            .HasMaxLength(255);

        builder.Property(le => le.Industry)
            .HasColumnName("industry")
            .HasMaxLength(255);

        builder.Property(le => le.Address)
            .HasColumnName("address")
            .HasMaxLength(255);

        builder.Property(le => le.IsValid)
            .HasColumnName("isValid")
            .IsRequired();

        builder.HasOne(le => le.User)
            .WithMany()
            .HasForeignKey(le => le.UserId);
    }
}
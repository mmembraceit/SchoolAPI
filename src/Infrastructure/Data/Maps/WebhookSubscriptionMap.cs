using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentApi.Domain.Models.Webhooks;

namespace StudentApi.Infrastructure.Data.Maps;

public class WebhookSubscriptionMap : BaseMap<WebhookSubscriptionEntity>
{
    public override void Configure(EntityTypeBuilder<WebhookSubscriptionEntity> entity)
    {
        base.Configure(entity);

        entity.ToTable("webhook_subscriptions");

        entity.Property(e => e.TenantId).IsRequired();

        entity.Property(e => e.Url)
            .HasMaxLength(2048)
            .IsRequired();

        entity.Property(e => e.Events)
            .HasMaxLength(1000)
            .IsRequired()
            .HasDefaultValue("[]");
    }
}

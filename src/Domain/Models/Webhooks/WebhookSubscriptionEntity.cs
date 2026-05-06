using StudentApi.Domain.Models;

namespace StudentApi.Domain.Models.Webhooks;

public class WebhookSubscriptionEntity : BaseEntity
{
    public Guid TenantId { get; set; }

    /// <summary>Target URL that receives the POST on each matching event.</summary>
    public string Url { get; set; } = null!;

    /// <summary>JSON array of event type strings, e.g. ["student.created","student.updated"].</summary>
    public string Events { get; set; } = "[]";
}

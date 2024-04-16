using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedInterfaces;

[Flags]
public enum TypeChannel
{
    None = 0,
    Email = 2,
    Telegram = 4,
    File = 8,    
}

public enum NotificationStatus
{
    None = 0,
    Sent = 2,
    Failed = 4,
    Wait = 8,
}

public enum SubscriptionStatus
{
    None = 0,
    Enable = 2,
    Disable = 4,
    Wait = 8,
}


public interface INotification
{
    Guid Id { get; set; }
    Guid UserGuid { get; set; }
    long UserId { get; set; }
    string Theme { get; set; }
    string Message { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime? SendDate { get; set; }

    TypeChannel TypeChannel { get; set; }

    NotificationStatus Status { get; set; }
    SubscriptionStatus SubscriptionStatus { get; set; }
}
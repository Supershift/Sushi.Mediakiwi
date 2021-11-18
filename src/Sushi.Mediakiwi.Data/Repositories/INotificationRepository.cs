using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data.Repositories
{
    /// <summary>
    /// Defines an interface to store and retrieve <see cref="Notification"/> entities.
    /// </summary>
    public interface INotificationRepository
    {
        Notification Save(Notification notification);
        Task<Notification> SaveAsync(Notification notification);
    }
}

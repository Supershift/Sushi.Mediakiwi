using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    public interface IEnvironmentVersion
    {
        int ID { get; set; }
        DateTime ServerEnvironmentVersion { get; set; }
        DateTime? Updated { get; set; }
        decimal Version { get; set; }

        bool Save();

        Task<bool> SaveAsync();
    }
}
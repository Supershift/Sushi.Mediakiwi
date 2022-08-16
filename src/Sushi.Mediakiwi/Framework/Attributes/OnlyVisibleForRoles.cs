using System;
using System.Collections.Generic;
using System.Linq;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// Represents a OnlyVisibleWhenTrue entity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OnlyVisibleForRoles : Attribute
    {

        /// <summary>
        /// The role names to which this is visible
        /// </summary>
        internal ICollection<string> RoleNames { get; set; }

        /// <summary>
        /// Only visible for the supplied Role
        /// </summary>
        /// <param name="property"></param>
        /// <param name="roleNames"></param>
        public OnlyVisibleForRoles(string roleName) : this(new List<string>() { roleName }) { }

        /// <summary>
        /// Only visible for the supplied Role
        /// </summary>
        /// <param name="property"></param>
        /// <param name="roleNames"></param>
        public OnlyVisibleForRoles(Data.IApplicationRole role) : this(new List<string>() { role.Name }) { }

        /// <summary>
        /// Only visible for the supplied Roles
        /// </summary>
        /// <param name="property"></param>
        /// <param name="roleNames"></param>
        public OnlyVisibleForRoles(ICollection<Data.IApplicationRole> roles) : this(roles.Select(x => x.Name).ToList()) { }

        /// <summary>
        /// Only visible for the supplied Roles
        /// </summary>
        /// <param name="property"></param>
        public OnlyVisibleForRoles(ICollection<string> roleNames)
        {
            RoleNames = new List<string>();

            foreach (var roleName in roleNames)
            {
                if (string.IsNullOrWhiteSpace(roleName) == false)
                {
                    RoleNames.Add(roleName.ToLowerInvariant());
                }
            }
        }
    }
}

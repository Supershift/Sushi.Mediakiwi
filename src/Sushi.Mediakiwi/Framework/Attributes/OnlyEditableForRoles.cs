using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OnlyEditableForRoles : Attribute
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
        public OnlyEditableForRoles(string roleName) : this(new List<string>() { roleName }) { }

        /// <summary>
        /// Only visible for the supplied Role
        /// </summary>
        /// <param name="property"></param>
        /// <param name="roleNames"></param>
        public OnlyEditableForRoles(Data.IApplicationRole role) : this(new List<string>() { role.Name }) { }

        /// <summary>
        /// Only visible for the supplied Roles
        /// </summary>
        /// <param name="property"></param>
        /// <param name="roleNames"></param>
        public OnlyEditableForRoles(ICollection<Data.IApplicationRole> roles) : this(roles.Select(x => x.Name).ToList()) { }

        /// <summary>
        /// Only visible for the supplied Roles
        /// </summary>
        /// <param name="property"></param>
        public OnlyEditableForRoles(ICollection<string> roleNames)
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

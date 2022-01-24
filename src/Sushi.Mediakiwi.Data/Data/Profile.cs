using System;
using System.Data;
using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.Utilities;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(ProfileMap))]
    public class Profile : IProfile
    {
        public class ProfileMap : DataMap<Profile>
        {
            public ProfileMap()
            {
                Table("wim_Profiles");
                Id(x => x.ID, "Profile_Key").Identity().ReadOnly();
                Map(x => x.GUID, "Profile_Guid");
                Map(x => x.Created, "Profile_Created").SqlType(SqlDbType.DateTime);
                Map(x => x.RememberMe, "Profile_RememberMe");
                Map(x => x.Email, "Profile_Email").SqlType(SqlDbType.NVarChar);
                Map(x => x.Password, "Profile_Password").SqlType(SqlDbType.NVarChar);
                Map(x => x.DataString, "Profile_Data").SqlType(SqlDbType.Xml);
            }
        }

        public int ID { get; set; }
        public Guid GUID { get; set; }
        public DateTime Created { get; set; }
        public bool? RememberMe { get; set; }
        public string Email { get; set; }
        public string Username { get; set; } 
        public string Password { get; set; }
        public string DataString { get; set; }

        private CustomData m_Data;

        /// <summary>
        /// Holds all customData properties
        /// </summary>
        public CustomData Data
        {
            get
            {
                if (m_Data == null)
                {
                    m_Data = new CustomData(DataString);
                }
                return m_Data;
            }
            set
            {
                m_Data = value;
                DataString = m_Data.Serialized;
            }
        }

        public static async Task<List<Profile>> FetchAllAsync()
        {
            var connector = new Connector<Profile>();
            var filter = connector.CreateQuery();
            var result = await connector.FetchAllAsync(filter);
            return result;
        }

        public static async Task<Profile> FetchSingleAsync(int id)
        {
            var connector = new Connector<Profile>();
            var result = await connector.FetchSingleAsync(id);
            return result;
        }

        public static async Task<Profile> FetchSingleByGuidAsync(Guid guid)
        {
            var connector = new Connector<Profile>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.GUID, guid);

            var result = await connector.FetchSingleAsync(filter);
            return result;
        }

        public static async Task<Profile> FetchSingleByEmailAsync(string emailAddress)
        {
            var connector = new Connector<Profile>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.Email, emailAddress);

            var result = await connector.FetchSingleAsync(filter);
            return result;
        }

        public async Task SaveAsync()
        {
            if (GUID == Guid.Empty)
            {
                GUID = Guid.NewGuid();
            }

            DataString = Data?.Serialized;
            Email = Email.ToLowerInvariant();

            var connector = new Connector<Profile>();
            await connector.SaveAsync(this);
        }

        public async Task Delete()
        {
            var connector = new Connector<Profile>();
            await connector.DeleteAsync(this);
        }

        public async Task SetPasswordAsync(string cleartextPassword)
        {
            PasswordHasher<Profile> hasher = new PasswordHasher<Profile>();
            Password = await Task.Run(() => hasher.HashPassword(this, cleartextPassword));
        }

        public async Task<bool> CheckPasswordAsync(string cleartextPassword)
        {
            PasswordHasher<Profile> hasher = new PasswordHasher<Profile>();
            var checkResult = await Task.Run(() => hasher.VerifyHashedPassword(this, Password, cleartextPassword));
            switch (checkResult)
            {
                default:
                case Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed: return false;
                case Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success: return true;
                case Microsoft.AspNetCore.Identity.PasswordVerificationResult.SuccessRehashNeeded:
                    {
                        await SetPasswordAsync(cleartextPassword);
                        await SaveAsync();
                        return true;
                    }
            }
        }


        #region IIdentity members

        public string AuthenticationType => "MKPROFILE";

        public bool IsAuthenticated { get; set; }

        public string Name => Email;

        #endregion IIdentity members
    }
}
using Raven.Client.UniqueConstraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkushApp.Model
{
    public static class Role
    {
        public const int ADMIN = 1;
        public const int MANAGER = 2;
        public const int OPERATOR = 3;
    }
    public class AppUser
    {
        [UniqueConstraint]
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public int RoleId { get; set; }
        public string RoleName
        {
            get
            {
                switch (RoleId)
                {
                    case Role.ADMIN:
                        return "ADMIN";
                    case Role.MANAGER:
                        return "MANAGER";
                    case Role.OPERATOR:
                    default:
                        return "OPERATOR";
                }
            }
        }
    }
}

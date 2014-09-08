using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EkushApp.Model;

namespace EkushApp.EmbededDB
{
    public class UserMapReduceIndex : AbstractIndexCreationTask<Users>
    {
        public UserMapReduceIndex()
        {
            Map = users => from user in users
                           select new
                           {
                               user.UserId,
                               user.Username
                           };
        }
    }
}

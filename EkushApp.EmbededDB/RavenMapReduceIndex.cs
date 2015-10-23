using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EkushApp.Model;

namespace EkushApp.EmbededDB
{
    public class AppUserMapReduceIndex : AbstractIndexCreationTask<AppUser>
    {
        public AppUserMapReduceIndex()
        {
            Map = users => from user in users
                           select new
                           {
                               user.Username,
                               user.Password
                           };
        }
    }
    public class HardwareMapIndex : AbstractIndexCreationTask<Hardware>
    {
        public HardwareMapIndex()
        {
            Map = hardwares => from hardware in hardwares
                               select new
                               {
                                   hardware.SerialNo,
                                   hardware.Status
                               };
        }
    }

    public class UserMapReduceIndex : AbstractIndexCreationTask<User>
    {
        public UserMapReduceIndex()
        {
            Map = users => from user in users
                           select new
                           {
                               user.Id,
                               user.Name
                           };
        }
    }
    public class SupplierMapReduceIndex : AbstractIndexCreationTask<Supplier>
    {
        public SupplierMapReduceIndex()
        {
            Map = suppliers => from supplier in suppliers
                               select new
                               {
                                   supplier.Id,
                                   supplier.Name
                               };
        }
    }
    public class BbSearchTermMapReduceIndex : AbstractIndexCreationTask<BbCircularSearch>
    {
        public BbSearchTermMapReduceIndex()
        {
            Map = searchTerms => from searchTerm in searchTerms
                                 select new
                                 {
                                     searchTerm.SearchKey,
                                 };
        }
    }
    public class BbCircularMapReduceIndex : AbstractIndexCreationTask<BbCircular>
    {
        public BbCircularMapReduceIndex()
        {
            Map = circulars => from circular in circulars
                               select new
                               {
                                   circular.Tag,
                                   circular.DepartmentKey,
                                   circular.CategoryKey,
                                   circular.Title,
                                   Published = circular.PublishDate.Date
                               };
        }
    }
}

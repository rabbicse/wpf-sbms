using Raven.Abstractions.Data;
using Raven.Abstractions.Linq;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EkushApp.Model;
using EkushApp.Logging;
using EkushApp.Utility.Extensions;
using System.IO;
using Raven.Json.Linq;
using Raven.Abstractions.Exceptions;
using System.Collections.Concurrent;
using Raven.Client.UniqueConstraints;
using System.Threading;
using Raven.Server;
using Raven.Database.Config;
using Raven.Abstractions.FileSystem;
using Raven.Client.FileSystem;

namespace EkushApp.EmbededDB
{
    public class DbHandler : IDisposable
    {
        #region Declaration(s)
        public static string DatabasePath { get; set; }
        public static string DbFilePath { get; set; }
        private readonly object _lockObject = new object();
        private SemaphoreSlim _syncLock = new SemaphoreSlim(1);
        #endregion

        #region Property(s)
        private static DbHandler _instance;
        public static DbHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DbHandler();
                }
                return _instance;
            }
        }
        private IDocumentStore DocumentStore
        {
            get { return RavenServer.Value.DocumentStore; }
        }
        private IFilesStore FileStore
        {
            get { return RavenServer.Value.FilesStore; }
        }
        private Lazy<RavenDbServer> RavenServer = new Lazy<RavenDbServer>(() =>
        {
            var ravenConfiguration = new RavenConfiguration
            {
                DataDirectory = DatabasePath,
                FlushIndexToDiskSizeInMb = 10,
                ResetIndexOnUncleanShutdown = true,
                Settings = { 
                //{"Raven/StorageEngine", "voron" },
                //{"Raven/Voron/MaxBufferPoolSize", "2"},
                //{"Raven/Voron/MaxScratchBufferSize", "512"},
                //{"Raven/Voron/AllowOn32Bits", "true"},
                {"Raven/TransactionMode", "Lazy"},
                {"Raven/MaxNumberOfItemsToIndexInSingleBatch", "128"},
                {"Raven/MaxNumberOfItemsToPreFetchForIndexing", "128"}}
            };
            var ravenServer = new RavenDbServer(ravenConfiguration);
            ravenServer.UseEmbeddedHttpServer = true;
            ravenServer.DocumentStore.RegisterListener(new UniqueConstraintsStoreListener());
            ravenServer.DocumentStore.Conventions.DefaultUseOptimisticConcurrency = true;
            ravenServer.DocumentStore.Conventions.DefaultQueryingConsistency = Raven.Client.Document.ConsistencyOptions.AlwaysWaitForNonStaleResultsAsOfLastWrite;
            ravenServer.Initialize();

            var createFs = Task.Run(async () =>
            {
                await ravenServer.FilesStore.AsyncFilesCommands.Admin.CreateOrUpdateFileSystemAsync(new FileSystemDocument
                {
                    Settings = { { "Raven/FileSystem/DataDir", DbFilePath } }
                }, "sbms_filesystem").ConfigureAwait(false);
            });
            createFs.Wait();
            ravenServer.FilesStore.DefaultFileSystem = "sbms_filesystem";

            return ravenServer;
        });
        #endregion

        #region Constructor(s)
        public DbHandler()
        {
            IndexCreation.CreateIndexes(typeof(AppUserMapReduceIndex).Assembly, DocumentStore);
            IndexCreation.CreateIndexes(typeof(HardwareMapIndex).Assembly, DocumentStore);
            IndexCreation.CreateIndexes(typeof(SupplierMapReduceIndex).Assembly, DocumentStore);
            IndexCreation.CreateIndexes(typeof(UserMapReduceIndex).Assembly, DocumentStore);
            IndexCreation.CreateIndexes(typeof(BbSearchTermMapReduceIndex).Assembly, DocumentStore);
            IndexCreation.CreateIndexes(typeof(BbCircularMapReduceIndex).Assembly, DocumentStore);
        }
        ~DbHandler()
        {
            Dispose(false);
        }
        public async Task ResetIndexes()
        {
            await DocumentStore.AsyncDatabaseCommands.ResetIndexAsync("BbSearchTermMapReduceIndex");
            await DocumentStore.AsyncDatabaseCommands.ResetIndexAsync("BbCircularMapReduceIndex");
        }
        #endregion

        #region AppUser Operation(s)
        public async Task<bool> SaveAppUserData(AppUser user)
        {
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    await session.Advanced.DocumentStore.AsyncDatabaseCommands.DeleteByIndexAsync("AppUserMapReduceIndex",
                                                    new IndexQuery
                                                    {
                                                        Query = "Username:" + user.Username
                                                    }, new BulkOperationOptions { AllowStale = false });
                    await session.StoreAsync(user);
                    await session.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when save user data.", x);
            }
            return false;
        }
        public async Task<bool> AuthenticateUser(string username, string password, Action<AppUser> appUser)
        {
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    var response = await session.Advanced.AsyncLuceneQuery<AppUser>("AppUserMapReduceIndex").Where("Username:" + username + " AND Password:" + password).WaitForNonStaleResultsAsOfLastWrite().ToListAsync();
                    if (response != null && response.Count > 0)
                    {
                        appUser(response.First());
                        return true;
                    }
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when authenticate user.", x);
            }
            return false;
        }
        public async Task<List<AppUser>> GetUsers()
        {
            List<AppUser> users = new List<AppUser>();
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    var response = await session.Advanced.AsyncDocumentQuery<AppUser, AppUserMapReduceIndex>().ToListAsync();
                    users.AddRange(response);
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when authenticate user.", x);
            }
            return users;
        }
        public async Task<List<AppUser>> GetAppUser()
        {
            List<AppUser> users = new List<AppUser>();
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    var response = await session.Advanced.AsyncDocumentQuery<AppUser, AppUserMapReduceIndex>().ToListAsync();
                    users.AddRange(response);
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when authenticate user.", x);
            }
            return users;
        }
        public async Task DeleteAppUser(AppUser user)
        {
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    var delOperation = await session.Advanced.DocumentStore.AsyncDatabaseCommands.DeleteByIndexAsync("AppUserMapReduceIndex",
                                                    new IndexQuery
                                                    {
                                                        Query = "Username: " + user.Username
                                                    }, new BulkOperationOptions { AllowStale = false });
                    await delOperation.WaitForCompletionAsync();
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when delete hardware.", x);
            }
        }
        #endregion

        #region Hardware
        public async Task SaveHardware(Hardware hardware)
        {
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    session.Advanced.UseOptimisticConcurrency = true;
                    await session.Advanced.DocumentStore.AsyncDatabaseCommands.DeleteByIndexAsync("HardwareMapIndex",
                                                    new IndexQuery
                                                    {
                                                        Query = "SerialNo: " + hardware.SerialNo
                                                    }, new BulkOperationOptions { AllowStale = false });
                    var maxIds = await session.Advanced.AsyncDocumentQuery<Hardware, HardwareMapIndex>().OrderByDescending(p => p.SerialNo).ToListAsync();
                    if (maxIds != null && maxIds.Count > 0)
                    {
                        var maxId = maxIds.Select(id => (long)id.SerialNo).Max();
                        hardware.SerialNo = maxId + 1;
                    }
                    else
                    {
                        hardware.SerialNo = 1;
                    }
                    await session.StoreAsync(hardware);
                    await session.SaveChangesAsync();
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when save hardware.", x);
            }
        }
        public async Task<List<Hardware>> GetHardwareCollection()
        {
            List<Hardware> hardwareBag = new List<Hardware>();
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    var hardWares = await session.Advanced.AsyncLuceneQuery<Hardware>("HardwareMapIndex")
                        .WaitForNonStaleResultsAsOfLastWrite().OrderBy(p => p.SerialNo).ToListAsync();
                    if (hardWares != null && hardWares.Count > 0)
                    {
                        hardwareBag.AddRange(hardWares);
                    }
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when save hardware.", x);
            }
            return hardwareBag;
        }
        public async Task DeleteHardware(Hardware hardware)
        {
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    var delOperation = await session.Advanced.DocumentStore.AsyncDatabaseCommands.DeleteByIndexAsync("HardwareMapIndex",
                                                    new IndexQuery
                                                    {
                                                        Query = "SerialNo: " + hardware.SerialNo
                                                    }, new BulkOperationOptions { AllowStale = false });
                    await delOperation.WaitForCompletionAsync();
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when delete hardware.", x);
            }
        }
        #endregion

        #region User
        public async Task SaveUser(User user)
        {
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    await session.Advanced.DocumentStore.AsyncDatabaseCommands.DeleteByIndexAsync("UserMapReduceIndex",
                                                    new IndexQuery
                                                    {
                                                        Query = "Id: " + user.Id
                                                    }, new BulkOperationOptions { AllowStale = false });
                    var maxIds = await session.Advanced.AsyncLuceneQuery<User>("UserMapReduceIndex")
                        .WaitForNonStaleResultsAsOfLastWrite().OrderByDescending(p => p.Id).ToListAsync();
                    if (maxIds != null && maxIds.Count > 0)
                    {
                        var maxId = maxIds.Select(id => (long)id.Id).Max();
                        user.Id = maxId + 1;
                    }
                    else
                    {
                        user.Id = 1;
                    }
                    await session.StoreAsync(user);
                    await session.SaveChangesAsync();
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when save hardware.", x);
            }
        }
        public async Task<List<User>> GetUserCollection()
        {
            List<User> hardwareBag = new List<User>();
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    var hardWares = await session.Advanced.AsyncLuceneQuery<User>("UserMapReduceIndex")
                        .WaitForNonStaleResultsAsOfLastWrite().ToListAsync();
                    if (hardWares != null && hardWares.Count > 0)
                    {
                        hardwareBag.AddRange(hardWares);
                    }
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when save hardware.", x);
            }
            return hardwareBag;
        }
        public async Task DeleteUser(User user)
        {
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    var delOperation = await session.Advanced.DocumentStore.AsyncDatabaseCommands.DeleteByIndexAsync("UserMapReduceIndex",
                                                    new IndexQuery
                                                    {
                                                        Query = "Id: " + user.Id
                                                    }, new BulkOperationOptions { AllowStale = false });
                    await delOperation.WaitForCompletionAsync();
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when delete hardware.", x);
            }
        }
        #endregion

        #region Supplier
        public async Task SaveSupplier(Supplier supplier)
        {
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    await session.StoreAsync(supplier);
                    await session.SaveChangesAsync();
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when save hardware.", x);
            }
        }
        public async Task<List<Supplier>> GetSupplierCollection()
        {
            List<Supplier> hardwareBag = new List<Supplier>();
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    var hardWares = await session.Advanced.AsyncLuceneQuery<Supplier>("SupplierMapReduceIndex")
                        .WaitForNonStaleResultsAsOfLastWrite().ToListAsync();
                    if (hardWares != null && hardWares.Count > 0)
                    {
                        hardwareBag.AddRange(hardWares);
                    }
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when save hardware.", x);
            }
            return hardwareBag;
        }
        public async Task DeleteSupplier(Supplier supplier)
        {
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    var delOperation = await session.Advanced.DocumentStore.AsyncDatabaseCommands.DeleteByIndexAsync("SupplierMapReduceIndex",
                                                    new IndexQuery
                                                    {
                                                        Query = "Id: " + supplier.Id
                                                    }, new BulkOperationOptions { AllowStale = false });
                    await delOperation.WaitForCompletionAsync();
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when delete hardware.", x);
            }
        }
        #endregion

        #region BbSearch
        public async Task<List<BbCircularSearch>> GetSearchTermCollection(string searchKey)
        {
            List<BbCircularSearch> collection = new List<BbCircularSearch>();
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    var list = await session.Advanced.AsyncDocumentQuery<BbCircularSearch, BbSearchTermMapReduceIndex>()
                        .WhereEquals("SearchKey", searchKey)
                        .WaitForNonStaleResultsAsOfLastWrite().ToListAsync();
                    if (list != null && list.Count > 0)
                    {
                        collection.AddRange(list);
                    }
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when save hardware.", x);
            }
            return collection;
        }
        #endregion

        #region BbCircular(s)
        public async Task<bool> SaveBbCircularData(BbCircular bbCircular)
        {
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    await session.StoreAsync(bbCircular);
                    await session.SaveChangesAsync();
                }
                using (var fileSession = FileStore.OpenAsyncSession())
                {
                    var stream = File.OpenRead(bbCircular.FileWithFullPath);
                    var metadata = new RavenJObject
                    {
                        {"File", bbCircular.FileName},
                    };
                    fileSession.RegisterUpload("file/" + bbCircular.FileName, stream, metadata);
                    await fileSession.SaveChangesAsync(); // actually upload the file
                }
                return true;
            }
            catch (Exception x)
            {
                Log.Error("Error when save user data.", x);
            }
            return false;
        }
        public async Task<List<BbCircular>> GetRecentCircular()
        {
            List<BbCircular> collection = new List<BbCircular>();
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    var list = await session.Advanced.AsyncDocumentQuery<BbCircular, BbCircularMapReduceIndex>()
                        .WaitForNonStaleResultsAsOfLastWrite()
                        .OrderByDescending(q => q.PublishDate)
                        .ToListAsync();
                    if (list != null && list.Count > 0)
                    {
                        collection.AddRange(list);
                    }
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when save hardware.", x);
            }
            return collection;
        }
        public async Task<List<BbCircular>> SearchCircularBySearchKey(string searchKey)
        {
            List<BbCircular> collection = new List<BbCircular>();
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    var list = await session.Advanced.AsyncDocumentQuery<BbCircular, BbCircularMapReduceIndex>()
                        .Search("SearchTermKey", searchKey)
                        .WaitForNonStaleResultsAsOfLastWrite()
                        .ToListAsync();
                    if (list != null && list.Count > 0)
                    {
                        collection.AddRange(list);
                    }
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when save hardware.", x);
            }
            return collection;
        }
        public async Task<List<BbCircular>> SearchCircularByTitle(string title)
        {
            List<BbCircular> collection = new List<BbCircular>();
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    var list = await session.Advanced.AsyncDocumentQuery<BbCircular, BbCircularMapReduceIndex>()
                        .Search("Title", title)
                        .WaitForNonStaleResultsAsOfLastWrite()
                        .ToListAsync();
                    if (list != null && list.Count > 0)
                    {
                        collection.AddRange(list);
                    }
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when save hardware.", x);
            }
            return collection;
        }
        public async Task<List<BbCircular>> SearchCircularByPubDate(DateTime from, DateTime to)
        {
            List<BbCircular> collection = new List<BbCircular>();
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    var list = await session.Advanced.AsyncDocumentQuery<BbCircular, BbCircularMapReduceIndex>()
                        .WhereBetween(x => x.PublishDate, from.Date, to.Date)
                        .WaitForNonStaleResultsAsOfLastWrite()
                        .ToListAsync();
                    if (list != null && list.Count > 0)
                    {
                        collection.AddRange(list);
                    }
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when save hardware.", x);
            }
            return collection;
        }
        public async Task<bool> DownloadFile(string filename, string savePath)
        {
            Stream stream = null;
            try
            {
                using (var session = FileStore.OpenAsyncSession())
                {
                    var file = await session.Query()
                                    .WhereEquals("File", filename)
                                    .FirstOrDefaultAsync();

                    stream = await session.DownloadAsync("file/" + filename);

                    using (FileStream fs = File.Create(Path.Combine(savePath, filename)))
                    {
                        await stream.CopyToAsync(fs);
                    }
                    return true;
                }
            }
            catch (Exception x)
            {
                Log.Error("There was an error when downloading file.", x);
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            return false;
        }
        public async Task DeleteBbCircular(BbCircular bbCircular)
        {
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    var query = session.Advanced.AsyncDocumentQuery<BbCircular, BbCircularMapReduceIndex>()
                        .WhereEquals("Title", bbCircular.Title)
                        .AndAlso()
                        .WhereEquals("PublishDate", bbCircular.PublishDate);
                    var delOperation = await session.Advanced.DocumentStore.AsyncDatabaseCommands.DeleteByIndexAsync("BbCircularMapReduceIndex",
                                                    new IndexQuery
                                                    {
                                                        Query = query.ToString()
                                                    }, new BulkOperationOptions { AllowStale = true });
                    await delOperation.WaitForCompletionAsync();
                }
                await FileStore.AsyncFilesCommands.DeleteAsync("file/" + bbCircular.FileName);
            }
            catch (Exception x)
            {
                Log.Error("Error when delete hardware.", x);
            }
        }
        #endregion

        #region Generic(s)
        public async Task<bool> SaveData<T>(T data)
        {
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    await session.StoreAsync(data);
                    await session.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when save data.", x);
            }
            return false;
        }
        public async Task<List<T>> GetAllData<T>()
        {
            List<T> dataCollection = new List<T>();
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    var dataList = await session.Advanced.AsyncDocumentQuery<T>()
                        .WaitForNonStaleResultsAsOfLastWrite().ToListAsync();
                    if (dataList != null && dataList.Count > 0)
                    {
                        dataCollection.AddRange(dataList);
                    }
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when save hardware.", x);
            }
            return dataCollection;
        }
        #endregion

        #region Method(s)
        public static void ShutDownDatabase()
        {
            if (_instance != null)
            {
                _instance.DocumentStore.Dispose();
                _instance.Dispose();
                _instance = null;
            }
        }
        #endregion

        #region IDisposeable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
        }
        #endregion
    }
}

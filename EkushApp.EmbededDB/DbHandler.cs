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

namespace EkushApp.EmbededDB
{
    public class DbHandler : IDisposable
    {
        #region Declaration(s)
        public static string DatabasePath { get; set; }
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
        private Lazy<IDocumentStore> DocStore = new Lazy<IDocumentStore>(() =>
        {
            var documentStore = new EmbeddableDocumentStore()
            {
                DataDirectory = DatabasePath
            };

            documentStore.RegisterListener(new UniqueConstraintsStoreListener());
            documentStore.Configuration.ResetIndexOnUncleanShutdown = true;
            documentStore.Configuration.MaxPageSize = 10000;
            documentStore.Configuration.MaxNumberOfItemsToIndexInSingleBatch = 1024 * 1024;
            documentStore.Configuration.MaxNumberOfItemsToPreFetchForIndexing = 1024 * 1024;
            documentStore.Configuration.MaxNumberOfItemsToReduceInSingleBatch = 1024 * 1024;
            documentStore.Configuration.NewIndexInMemoryMaxBytes = 128;
            documentStore.Configuration.InitialNumberOfItemsToIndexInSingleBatch = 1024;
            documentStore.Initialize();

            return documentStore;
        });
        private IDocumentStore DocumentStore
        {
            get { return DocStore.Value; }
        }
        #endregion

        #region Constructor(s)
        public DbHandler()
        {
            IndexCreation.CreateIndexes(typeof(AppUserMapReduceIndex).Assembly, DocumentStore);
            IndexCreation.CreateIndexes(typeof(HardwareMapReduceIndex).Assembly, DocumentStore);
            IndexCreation.CreateIndexes(typeof(SupplierMapReduceIndex).Assembly, DocumentStore);
            IndexCreation.CreateIndexes(typeof(UserMapReduceIndex).Assembly, DocumentStore);
        }
        ~DbHandler()
        {
            Dispose(false);
        }
        #endregion

        #region AppUser Operation(s)
        public void SaveUserData(List<AppUser> userCollection)
        {
            try
            {
                using (var bulkInsert = DocumentStore.BulkInsert())
                {
                    userCollection.AsParallel().ForAll(p => bulkInsert.Store(p));
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when saving user data.", x);
            }
        }
        public async Task SaveAppUserData(AppUser user)
        {
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    await session.StoreAsync(user);
                    await session.SaveChangesAsync();
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when save user data.", x);
            }
        }
        public async Task<bool> AuthenticateUser(string username, string password)
        {
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    var response = await session.Advanced.AsyncLuceneQuery<AppUser>("AppUserMapReduceIndex").Where("Username: " + username).WaitForNonStaleResultsAsOfLastWrite().ToListAsync();
                    return response != null && response.Any(p => p.Password.Equals(password));
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
                    var response = await session.Advanced.AsyncLuceneQuery<AppUser>("AppUserMapReduceIndex").WaitForNonStaleResultsAsOfLastWrite().ToListAsync();
                    users.AddRange(response);
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when authenticate user.", x);
            }
            return users;
        }
        #endregion

        #region Hardware
        public async Task SaveHardware(Hardware hardware)
        {
            try
            {
                using (var session = DocumentStore.OpenAsyncSession())
                {
                    await session.StoreAsync(hardware);
                    await session.SaveChangesAsync();
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when save hardware.", x);
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
                    await session.StoreAsync(user);
                    await session.SaveChangesAsync();
                }
            }
            catch (Exception x)
            {
                Log.Error("Error when save hardware.", x);
            }
        }
        #endregion

        #region Hardware
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

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.IdentityServer.Grants;
using Volo.Abp.MongoDB;


namespace Volo.Abp.IdentityServer.MongoDB
{
    public class MongoPersistentGrantRepository : MongoDbRepository<IAbpIdentityServerMongoDbContext, PersistedGrant, Guid>, IPersistentGrantRepository
    {
        public MongoPersistentGrantRepository(IMongoDbContextProvider<IAbpIdentityServerMongoDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<PersistedGrant>> GetListAsync(string subjectId, string sessionId, string clientId, string type, bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await Filter(subjectId, sessionId, clientId, type)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<PersistedGrant> FindByKeyAsync(string key, CancellationToken cancellationToken = default)
        {

            return await GetMongoQueryable()
                .FirstOrDefaultAsync(x => x.Key == key, GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<PersistedGrant>> GetListBySubjectIdAsync(string subjectId, CancellationToken cancellationToken = default)
        {
            return await GetMongoQueryable()
                .Where(x => x.SubjectId == subjectId)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<PersistedGrant>> GetListByExpirationAsync(DateTime maxExpirationDate, int maxResultCount,
            CancellationToken cancellationToken = default)
        {
            return await GetMongoQueryable()
                .Where(x => x.Expiration != null && x.Expiration < maxExpirationDate)
                .OrderBy(x => x.ClientId)
                .Take(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task DeleteAsync(
            string subjectId = null,
            string sessionId = null,
            string clientId = null,
            string type = null,
            CancellationToken cancellationToken = default)
        {
            var persistedGrants = await Filter(subjectId, sessionId, clientId, type)
                .ToListAsync(GetCancellationToken(cancellationToken));

            foreach (var persistedGrant in persistedGrants)
            {
                await DeleteAsync(persistedGrant, false, GetCancellationToken(cancellationToken));
            }
        }

        public virtual async Task DeleteAsync(string subjectId, string clientId, CancellationToken cancellationToken = default)
        {
            await DeleteAsync(
                x => x.SubjectId == subjectId && x.ClientId == clientId,
                cancellationToken: GetCancellationToken(cancellationToken)
            );
        }

        public virtual async Task DeleteAsync(string subjectId, string clientId, string type, CancellationToken cancellationToken = default)
        {
            await DeleteAsync(
                x => x.SubjectId == subjectId && x.ClientId == clientId && x.Type == type,
                cancellationToken: GetCancellationToken(cancellationToken)
            );
        }

        private IMongoQueryable<PersistedGrant> Filter(
            string subjectId,
            string sessionId,
            string clientId,
            string type)
        {
            return GetMongoQueryable()
                .WhereIf<PersistedGrant, IMongoQueryable<PersistedGrant>>(!subjectId.IsNullOrWhiteSpace(), x => x.SubjectId == subjectId)
                .WhereIf<PersistedGrant, IMongoQueryable<PersistedGrant>>(!sessionId.IsNullOrWhiteSpace(), x => x.SessionId == sessionId)
                .WhereIf<PersistedGrant, IMongoQueryable<PersistedGrant>>(!clientId.IsNullOrWhiteSpace(), x => x.ClientId == clientId)
                .WhereIf<PersistedGrant, IMongoQueryable<PersistedGrant>>(!type.IsNullOrWhiteSpace(), x => x.Type == type)
                .As<IMongoQueryable<PersistedGrant>>();
        }
    }
}

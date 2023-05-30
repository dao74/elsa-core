using Elsa.MongoDB.Common;
using Elsa.MongoDB.Extensions;
using Elsa.Workflows.Core.Contracts;
using Elsa.Workflows.Runtime.Contracts;
using Elsa.Workflows.Runtime.Entities;
using MongoDB.Driver.Linq;

namespace Elsa.MongoDB.Stores.Runtime;

/// <inheritdoc />
public class MongoTriggerStore : ITriggerStore
{
    private readonly MongoStore<StoredTrigger> _mongoStore;
    private readonly IPayloadSerializer _serializer;

    /// <summary>
    /// Constructor.
    /// </summary>
    public MongoTriggerStore(MongoStore<StoredTrigger> mongoStore, IPayloadSerializer serializer)
    {
        _mongoStore = mongoStore;
        _serializer = serializer;
    }

    /// <inheritdoc />
    public async ValueTask SaveAsync(StoredTrigger record, CancellationToken cancellationToken = default) => 
        await _mongoStore.SaveAsync(record, cancellationToken);

    /// <inheritdoc />
    public async ValueTask SaveManyAsync(IEnumerable<StoredTrigger> records, CancellationToken cancellationToken = default) => 
        await _mongoStore.SaveManyAsync(records, cancellationToken);

    /// <inheritdoc />
    public async ValueTask<IEnumerable<StoredTrigger>> FindManyAsync(TriggerFilter filter, CancellationToken cancellationToken = default) => 
        await _mongoStore.FindManyAsync(query => Filter(query, filter), cancellationToken);

    /// <inheritdoc />
    public async ValueTask ReplaceAsync(IEnumerable<StoredTrigger> removed, IEnumerable<StoredTrigger> added, CancellationToken cancellationToken = default)
    {
        var filter = new TriggerFilter { Ids = removed.Select(r => r.Id).ToList() };
        await DeleteManyAsync(filter, cancellationToken);
        await _mongoStore.SaveManyAsync(added, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask<long> DeleteManyAsync(TriggerFilter filter, CancellationToken cancellationToken = default) =>
        await _mongoStore.DeleteWhereAsync(query => Filter(query, filter), cancellationToken);
    
    private static IMongoQueryable<StoredTrigger> Filter(IMongoQueryable<StoredTrigger> queryable, TriggerFilter filter) => 
        (filter.Apply(queryable) as IMongoQueryable<StoredTrigger>)!;
}
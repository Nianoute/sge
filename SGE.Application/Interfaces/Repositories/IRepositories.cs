using System.Linq.Expressions;

namespace SGE.Application.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    /// <summary>
    ///     Retrieves an entity of type <typeparamref name="T" /> by it unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     A task representing the asynchronous operation, with the resultbeing the entity of type <typeparamref name="T" />
    ///     if it exists,
    ///     or null if no entity with the specified identifier is found.
    /// </returns>
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves all entities of type <typeparamref name="T" /> from therepository.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     A task representing the asynchronous operation, with the result being an enumerable of all entities of type
    ///     <typeparamref name="T" />.
    /// </returns>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Finds entities of type <typeparamref name="T" /> that match the specified predicate.
    /// </summary>
    /// <param name="predicate">A LINQ expression used to filter entities based on specific criteria.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     A task representing the asynchronous operation, with the result being an enumerable of entities of type
    ///     <typeparamref name="T" />
    ///     that satisfy the specified predicate, or an empty collection if no entities match the criteria.
    /// </returns>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     A task representing the asynchronous operation of adding the entity.
    /// </returns>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates an existing entity of type <typeparamref name="T" /> in the repository.
    /// </summary>
    /// <param name="entity">The entity of type <typeparamref name="T" /> to</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     A task representing the asynchronous operation, with the result being the updated entity of type
    ///     <typeparamref name="T" />.
    /// </returns>
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Checks whether an entity of type <typeparamref name="T" /> with the specified identifier exists in the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to check for</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     A task representing the asynchronous operation, with the result being true if an entity with the specified
    ///     identifier exists,
    ///     or false otherwise.
    /// </returns>
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The result indicates whether the entity was successfully deleted.
    /// </returns>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
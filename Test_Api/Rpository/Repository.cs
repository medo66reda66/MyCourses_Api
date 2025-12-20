using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Test_Api.Datebase;
using Test_Api.Rpository.Irepository;

namespace Test_Api.Rpository
{
    public class Repository<T> : IRepository<T>  where T : class
    {
        ApplicationDBcontext _context;
        private DbSet<T> _db;
        public Repository(ApplicationDBcontext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }
        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _db.AddAsync(entity, cancellationToken);
            return entity;
        }
        public void Update(T entity)
        {
            _db.Update(entity);
        }
        public void Delete(T entity)
        {
            _db.Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? includes = null,
            bool tracked = true,
            CancellationToken cancellationToken = default)
        {var entities = _db.AsQueryable();
            
            if (expression is not null)
            {
               entities = entities.Where(expression);
            }
            if (includes is not null)
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include);
                }
            }
            if (!tracked)
            {
                entities = entities.AsNoTracking();
            }
            return await entities.ToListAsync(cancellationToken);
        }
        public async Task<T> GetOneAsync(
             Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? includes = null,
            bool tracked = true,
            CancellationToken cancellationToken = default
            ) 
        { 
            return (await GetAllAsync(expression, includes, tracked, cancellationToken)).FirstOrDefault()!;
        }
        public async Task ComitSaveAsync( CancellationToken cancellationToken)
        {
            try
            {
                await _context.SaveChangesAsync( cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error",ex.Message);
            }
        }
    }
}

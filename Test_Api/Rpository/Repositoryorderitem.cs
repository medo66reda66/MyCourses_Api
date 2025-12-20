using System.Threading.Tasks;
using Test_Api.Datebase;
using Test_Api.Models;
using Test_Api.Rpository.Irepository;


namespace Test_Api.Rpository
{
    public class Repositoryorderitem :Repository<Mycourse> , Irepositoruorderitem
    {
         private ApplicationDBcontext _context;
        public Repositoryorderitem(ApplicationDBcontext context) : base(context)
        {
            _context = context;
        }
        public async Task AddrangeAsunc(IEnumerable<Mycourse> Mycourses, CancellationToken cancellationToken=default)
        {
           await _context.AddRangeAsync(Mycourses, cancellationToken);
        }
        public void Removerange(IEnumerable<Mycourse> Mycourses)
        {
            _context.RemoveRange(Mycourses);
        }

    }
}

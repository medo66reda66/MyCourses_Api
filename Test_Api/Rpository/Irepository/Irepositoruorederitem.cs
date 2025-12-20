using Test_Api.Models;

namespace Test_Api.Rpository.Irepository
{
    public interface Irepositoruorderitem : IRepository<Mycourse>
    {
        Task AddrangeAsunc(IEnumerable<Mycourse> Mycourses, CancellationToken cancellationToken = default);
        void Removerange(IEnumerable<Mycourse> Mycourses);
    }
}

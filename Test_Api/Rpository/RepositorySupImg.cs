using System.Threading.Tasks;
using Test_Api.Datebase;
using Test_Api.Models;
using Test_Api.Rpository.Irepository;

namespace Test_Api.Rpository
{
    public class RepositorySupImg :Repository<CourseVideos> ,IrepositoruSupImg
    {
         private ApplicationDBcontext _context;
        public RepositorySupImg(ApplicationDBcontext context) : base(context)
        {
            _context = context;
        }
        public async Task AddrangeAsunc(IEnumerable<CourseVideos> courseSupImgs ,CancellationToken cancellationToken=default)
        {
           await _context.AddRangeAsync(courseSupImgs ,cancellationToken);
        }
        public void Removerange(IEnumerable<CourseVideos> courseSupImgs)
        {
            _context.RemoveRange( courseSupImgs);
        }

    }
}

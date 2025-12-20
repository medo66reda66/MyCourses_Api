using Test_Api.Models;

namespace Test_Api.Rpository.Irepository
{
    public interface IrepositoruSupImg : IRepository<CourseVideos>
    {
        Task AddrangeAsunc(IEnumerable<CourseVideos> courseSupImgs, CancellationToken cancellationToken = default);
        void Removerange(IEnumerable<CourseVideos> courseSupImgs);
    }
}

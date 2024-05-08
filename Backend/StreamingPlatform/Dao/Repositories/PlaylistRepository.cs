namespace StreamingPlatform.Dao.Repositories
{
    public class PlaylistRepository
    {
        private readonly StreamingDbContext _context;

        public PlaylistRepository(StreamingDbContext context)
        {
            _context = context;
        }
    }
}
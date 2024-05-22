using StreamingPlatform.Dao.Interfaces;
using StreamingPlatform.Dtos.Response;
using StreamingPlatform.Models;
using StreamingPlatform.Services.Interfaces;

namespace StreamingPlatform.Services
{
    public class SongService(IUnitOfWork unitOfWork): ISongService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;


        public async Task<SongResponseDto> GetSongById(string id)
        {
            IGenericRepository<Song> repository = this.unitOfWork.Repository<Song>();
            Song song = await repository.GetRecordByIdAsync(new Guid(id));
            return new SongResponseDto(song.Title, song.ArtistId.ToString(), song.Duration.ToString(), song.AlbumId.ToString());
        }
    }
}
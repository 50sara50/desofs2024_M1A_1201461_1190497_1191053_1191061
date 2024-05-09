using AutoMapper;
using StreamingPlatform.Dao.Helper;

namespace StreamingPlatform.Dtos.Mappers
{
    /// <summary>
    /// Automapper profile for PagedResponseOffsetDto
    /// </summary>
    public class PageResponseDTOMapper : Profile
    {
        public PageResponseDTOMapper()
        {
            this.CreateMap(typeof(PagedResponseOffset<>), typeof(PagedResponseOffset<>));
        }
    }
}

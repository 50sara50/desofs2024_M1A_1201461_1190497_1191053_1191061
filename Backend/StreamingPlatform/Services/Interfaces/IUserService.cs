namespace StreamingPlatform
{
    public interface IUserService
    {
        public Task<NewUserDto> GetById(Guid id);
    }
}
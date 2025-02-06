namespace HSOne.WebApp.Services
{
    public interface IMediaService
    {
        Task<byte[]> GetImageAsync(string filePath);
    }
}

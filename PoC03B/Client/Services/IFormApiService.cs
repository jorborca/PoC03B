using PoC03B.Shared.Models;

namespace PoC03B.Client.Services
{
    public interface IFormApiService
    {
        Task<List<FormHistory>?> GetHistory();
        Task PostHistory(Guid id, string name, string description);

        Task<FormLayout> GetForm(string idForm);
        Task PostForm(FormLayout cleanTemplate);
    }
}
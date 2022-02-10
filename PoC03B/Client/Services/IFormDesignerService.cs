using PoC03B.Shared.Models;

namespace PoC03B.Client.Services
{
    public interface IFormDesignerService
    {
        Task<List<FormHistory>?> GetHistory();
        Task PostHistory(Guid id, string name, string description);

        Task<FormDesigner> GetForm(string idForm);
        Task PostForm(FormDesigner cleanTemplate);
    }
}
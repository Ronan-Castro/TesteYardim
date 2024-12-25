using Microsoft.AspNetCore.Components.Forms;

namespace DesafioYardim.Service
{
    public interface IConversorService
    {
        Task<List<Dictionary<string, string>>> LerJson(IBrowserFile? arquivoDadosUpload);
        string GerarCsv(List<Dictionary<string, string>> dadosJson);
        List<Dictionary<string, string>> GerarTabela(List<Dictionary<string, string>> dadosJson);
    }
}
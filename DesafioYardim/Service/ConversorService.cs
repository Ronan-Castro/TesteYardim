using Microsoft.AspNetCore.Components.Forms;
using System.Text;
using System.Text.Json;

namespace DesafioYardim.Service
{
    public class ConversorService : IConversorService
    {

        public async Task<List<Dictionary<string, string>>> LerJson(IBrowserFile? arquivoDadosUpload)
        {
            var arquivoStream = arquivoDadosUpload.OpenReadStream();
            using (var reader = new System.IO.StreamReader(arquivoStream))
            {
                var jsonContent = await reader.ReadToEndAsync();

                // Corrige manualmente o JSON para lidar com chaves duplicadas
                var linhas = new List<Dictionary<string, string>>();

                try
                {
                    var jsonDocument = System.Text.Json.JsonDocument.Parse(jsonContent);
                    var root = jsonDocument.RootElement;

                    if (root.ValueKind == System.Text.Json.JsonValueKind.Object)
                    {
                        var tempData = new Dictionary<string, List<string>>();

                        //Monta o cabeçalho
                        foreach (var property in root.EnumerateObject())
                        {
                            if (!tempData.ContainsKey(property.Name))
                            {
                                tempData[property.Name] = new List<string>();
                            }
                            string valueAsString = property.Value.ValueKind == JsonValueKind.String
                                                        ? property.Value.GetString()
                                                        : property.Value.GetRawText();
                            tempData[property.Name].Add(valueAsString);

                        }

                        // Amarra a linha aos cabeçalhos na lista de dicionários
                        var maxRows = tempData.Values.Max(list => list.Count);
                        for (int i = 0; i < maxRows; i++)
                        {
                            var row = new Dictionary<string, string>();
                            foreach (var key in tempData.Keys)
                            {
                                row[key] = i < tempData[key].Count ? tempData[key][i] : string.Empty;
                            }
                            linhas.Add(row);
                        }
                    }

                    return linhas;
                }
                catch (Exception ex)
                {
                    // Caso o JSON esteja mal formatado
                    return null;
                }
            }
        }

        // Gera o CSV a partir de um Dictionary<string, object>
        public string GerarCsv(List<Dictionary<string, string>> dadosJson)
        {
            var csv = new StringBuilder();

            if (dadosJson == null || dadosJson.Count == 0)
                return string.Empty;

            // Adicionar cabeçalho do CSV (chaves do primeiro dicionário, pois irão se repetir)
            var cabecalho = string.Join(",", dadosJson.First().Keys);
            csv.AppendLine(cabecalho);

            // Para cada linha amarrada ao cabeçalho no dicionário, criar uma nova linha no csv
            foreach (var linha in dadosJson)
            {
                var valores = string.Join(",", linha.Values.Select(v => v?.Replace(",", "\\,") ?? string.Empty)); // Escapa vírgulas
                csv.AppendLine(valores);
            }

            return csv.ToString();
        }
        public List<Dictionary<string, string>> GerarTabela(List<Dictionary<string, string>> dadosJson)
        {
            if (dadosJson == null || dadosJson.Count == 0)
                return new List<Dictionary<string, string>>();

            // Apenas retorna os dados no formato já existente
            return dadosJson.Select(linha => new Dictionary<string, string>(linha)).ToList();
        }

    }
}

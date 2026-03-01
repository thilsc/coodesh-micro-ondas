using System.Text.Json;

namespace MicroondasDigital.Models;

public static class CustomProgramRepository
{
    private static readonly string FileName = "customPrograms.json";
    private static string FilePath => Path.Combine(Directory.GetCurrentDirectory(), FileName);

    public static List<AquecimentoCustomizadoModel> GetAll()
    {
        if (!File.Exists(FilePath))
            return new List<AquecimentoCustomizadoModel>();

        try
        {
            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<AquecimentoCustomizadoModel>>(json)
                   ?? new List<AquecimentoCustomizadoModel>();
        }
        catch
        {
            // se o arquivo estiver corrompido, descartamos e recomeçamos
            return new List<AquecimentoCustomizadoModel>();
        }
    }

    public static void SaveAll(IEnumerable<AquecimentoCustomizadoModel> programs)
    {
        var opts = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(programs, opts);
        File.WriteAllText(FilePath, json);
    }

    public static void Add(AquecimentoCustomizadoModel program)
    {
        if (string.IsNullOrEmpty(program.Id))
        {
            program.Id = Guid.NewGuid().ToString();
        }
        var list = GetAll();
        list.Add(program);
        SaveAll(list);
    }

    public static void Update(AquecimentoCustomizadoModel program)
    {
        var list = GetAll();
        var idx = list.FindIndex(p => p.Id == program.Id);
        if (idx >= 0)
        {
            list[idx] = program;
            SaveAll(list);
        }
    }

    public static void Delete(string id)
    {
        var list = GetAll();
        list.RemoveAll(p => p.Id == id);
        SaveAll(list);
    }
}

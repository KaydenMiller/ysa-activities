using System.Text.Json;
using LaytonYSAClerk.Cli;

namespace LaytonYSAClerk.Data;

public class RecordDataService
{
    public IEnumerable<MergedRecord> GetRecords()
    {
        var file = File.ReadAllText("./merged-records.json");
        var mergeRecords = JsonSerializer.Deserialize<List<MergedRecord>>(file)!;

        var sortedRecords = mergeRecords
           .OrderBy(x => x.Fullname)
           .ToList();

        return sortedRecords;
    }
}
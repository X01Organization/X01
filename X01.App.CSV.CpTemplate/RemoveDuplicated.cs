namespace X01.App.CSV.CpTemplate;
internal class RemoveDuplicated
{
    public async Task ExecuteAsync(CancellationToken token)
    {
        string[] lines = await File.ReadAllLinesAsync("C:\\workroot\\test\\CostPositionFilterReport.csv", token);
        string[] ss = 
        lines.Select(x => x.Split(';')).Select(x => new
        {
            Status = x[0],
            ServiceType = x[1],
            CostPositionDescription = x[3],
            TemplateId = x[4],
            TemplateDescription = x[5],
            ComponentId = x[6],
            ComponentName = x[7],
        }).GroupBy(x => new
        {
            x.Status,
            x.ServiceType,
            x.CostPositionDescription,
            x.TemplateId,
            x.TemplateDescription,
            x.ComponentId,
            x.ComponentName,
        })
        .Select(x=>  new string[] {
            x.Key.Status,
            x.Key.ServiceType,
            x.Key.CostPositionDescription,
            x.Key.TemplateId,
            x.Key.TemplateDescription,
            x.Key.ComponentId,
            x.Key.ComponentName,
        })
        .Select(x=> string.Join(';', x))
        .ToArray();
       await File.WriteAllLinesAsync("C:\\workroot\\test\\CostPositionFilterReport1.csv", ss);
Console.WriteLine("==>");
    }
}

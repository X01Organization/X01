// See https://aka.ms/new-console-template for more information

using createsql;

var tabledict = new Dictionary<string, long>()
{
    {"tourop_trip_cost_flight",236237733 },
    {"tourop_trip_cost_carrental",56948462 },
    {"tourop_trip_cost_accommodation",794769887 },
    {"tourop_company_cost_accommodation",286428972 },
};

foreach (var item in tabledict)
{
    string table = item.Key;
    long count  = item.Value;
    string sql = "UPDATE " + table
                           + @" SET ""BookingInformation"" = ""Remarks"" WHERE ""BookingInformation"" IS NOT NULL AND ""Remarks"" IS NOT NULL";
    int chunk = 100000;
    var sss = new Class1().Split(count, chunk, sql).ToArray();
    File.WriteAllLines(@"c:\\temp\\" + table + ".sql", sss);
}
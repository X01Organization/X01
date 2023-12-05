using System.Text;

namespace Translation;
public class Generation
{
    public void Generate()
    {
        var lines = File.ReadAllLines("C:\\workroot\\1.txt");
        var test = lines.Where(x=> !string.IsNullOrWhiteSpace(x)).Select(x=> x.Split('|'))
            .Select(x=>new Info(){ english = x[0].Trim(), german=x[1].Trim(), header = "transport",})
            .ToList();

writeFile(test);
        int a =0;
    }

    public void Generate1()
    {
        var lines = File.ReadAllLines("C:\\workroot\\1.txt");
        var headers = new[] { "hotel", "room", "parking", };

        List<Info> all = new List<Info>();
        foreach (var x in lines.Skip(2))
        {
            Console.WriteLine("");
            Console.WriteLine("=============>");
            Console.WriteLine("");
            Console.WriteLine(x);
            all.AddRange(generate(headers, x));
        }

        writeFile(all);
    }

    private void writeFile(List<Info> all)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var x in all.DistinctBy(x => new
        {
            x.header,
            x.german,
            x.english,
            x.dutch,
        })
            .Where(x => !(x.german == x.english && x.german == x.dutch)).Select(x =>
        {
            x.header = $"contentful.amenity.{x.header}.{x.english}";
            return x;
        })
            .OrderBy(x => x.header).ThenBy(x=> x.order).DistinctBy(x=> x.header))
        {
            sb.Append("  \"");
            sb.Append(x.header);
            sb.Append("\": {");
            sb.AppendLine();

            sb.Append("    \"de-DE\": \"");
            sb.Append(x.german);
            sb.Append("\",");
            sb.AppendLine();

            sb.Append("    \"en-GB\": \"");
            sb.Append(x.english);
            sb.Append("\",");
            sb.AppendLine();

            sb.Append("    \"nl-NL\": \"");
            sb.Append(x.dutch);
            sb.Append("\"");
            sb.AppendLine();

            sb.Append("  },");
            sb.AppendLine();
        }

        File.WriteAllText("c:/workroot/test11.json", sb.ToString());
    }
    private IEnumerable<Info> generate(string[] headers, string line)
    {
        var columns = line.Split('\t').Select(x => x.Trim()).ToArray();
        if (18 != columns.Length)
        {
            throw new Exception("bug");
        }
        var chs = columns.Chunk(3).ToArray();
        var g1 = chs[0].Concat(chs[3]).ToArray();
        var g2 = chs[1].Concat(chs[4]).ToArray();
        var g3 = chs[2].Concat(chs[5]).ToArray();
        foreach (var z1 in generate(headers[0], g1))
        {
            yield return z1;
        }
        foreach (var z2 in generate(headers[1], g2))
        {
            yield return z2;
        }
        foreach (var z3 in generate(headers[2], g3))
        {
            yield return z3;
        }
    }

    private IEnumerable<Info> generate(string header, string[] columns)
    {
        int priority = 1;
        foreach (var x in columns.Chunk(3))
        {
            if (x.Length != 3)
            {
                throw new Exception("bug");
            }
            if (!x.All(y => string.IsNullOrWhiteSpace(y)))
            {
                if (x.Any(y => string.IsNullOrWhiteSpace(y)))
                {
                    throw new Exception("bug");
                }
                yield return new Info()
                {
                    order = priority++,
                    header = header,
                    german = x[0],
                    english = x[1],
                    dutch = x[2],
                };
            }
        }
    }
    private class Info
    {
        public int order { get; set; } = 0;
        public string header { get; set; }
        public string german { get; set; }
        public string english { get; set; }
        public string dutch { get; set; }
    }
}

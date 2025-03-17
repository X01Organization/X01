using System.Text;

namespace Translation;
public class Generation
{
    public void Generate()
    {
Generate1();
Generate2();
    }
 public void Generate3()
    {
        string[] lines = File.ReadAllLines("C:\\workroot\\3.txt");
        List<Info> test = lines.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Split('|'))
            .Select(x => new Info() { english = x[0].Trim(), german = x[1].Trim(), header = "transport", })
            .ToList();
        writeFile("c:/workroot/translation3.json", test, false);
        int a = 0;
    }

    public void Generate2()
    {
        string[] lines = File.ReadAllLines("C:\\workroot\\2.txt");
        List<Info> test = lines.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Split('|'))
            .Select(x => new Info() { english = x[0].Trim(), german = x[1].Trim(), header = "transport", })
            .ToList();
        writeFile("c:/workroot/translation2.json", test, false);
    }

    public void Generate1()
    {
        string[] lines = File.ReadAllLines("C:\\workroot\\1.txt");
        string[] headers = new[] { "accommodation.hotel", "accommodation.room", "accommodation.parking", };

        List<Info> all = new();
        foreach (string? x in lines.Skip(2))
        {
            Console.WriteLine("");
            Console.WriteLine("=============>");
            Console.WriteLine("");
            Console.WriteLine(x);
            all.AddRange(generate(headers, x));
        }

        writeFile("c:/workroot/translation1.json", all, true);
    }

    private void writeFile(string file,List<Info> all, bool germanKey)
    {
        StringBuilder sb = new();
        foreach (Info? x in all.DistinctBy(x => new
        {
            x.header,
            x.german,
            x.english,
            x.dutch,
        })
            //.Where(x => !(x.german == x.english && x.german == x.dutch))
            .Select(x =>
        {
            x.header = $"contentful.{x.header}.{(germanKey ? x.german : x.english)}";
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

        File.WriteAllText(file, sb.ToString());
    }
    private IEnumerable<Info> generate(string[] headers, string line)
    {
        string[] columns = line.Split('\t').Select(x => x.Trim()).ToArray();
        if (18 != columns.Length)
        {
            throw new Exception("bug");
        }
        string[][] chs = columns.Chunk(3).ToArray();
        string[] g1 = chs[0].Concat(chs[3]).ToArray();
        string[] g2 = chs[1].Concat(chs[4]).ToArray();
        string[] g3 = chs[2].Concat(chs[5]).ToArray();
        foreach (Info z1 in generate(headers[0], g1))
        {
            yield return z1;
        }
        foreach (Info z2 in generate(headers[1], g2))
        {
            yield return z2;
        }
        foreach (Info z3 in generate(headers[2], g3))
        {
            yield return z3;
        }
    }

    private IEnumerable<Info> generate(string header, string[] columns)
    {
        int priority = 1;
        foreach (string[] x in columns.Chunk(3))
        {
            if (x.Length != 3)
            {
                throw new Exception("bug");
            }
            if (!x.All(string.IsNullOrWhiteSpace))
            {
                if (x.Any(string.IsNullOrWhiteSpace))
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

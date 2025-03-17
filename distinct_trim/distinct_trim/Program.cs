// See https://aka.ms/new-console-template for more information

using distinct_trim;

Parser.Default
      .ParseArguments<Options>(args)
      .WithParsed(o =>
       {
           if (text.Length != o.Len)
           {
               Console.WriteLine($"[WARN]: decoded length {text.Length}");
           }

           Console.WriteLine(text);
       });
File.ReadAllLines(args[0]).Select(line => line.Trim()).Where(line => !string.IsNullOrEmpty(line)).Distinct();

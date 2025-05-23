﻿using System.Text.Json;
using System.Text.Json.Nodes;
using CommandLine;
using JsonFormatter;

Parser.Default
      .ParseArguments<Options>(args)
      .WithParsed(o =>
       {
           JsonObject? outputJsonString = null;
           using (FileStream s = File.OpenRead(o.Input))
           {
               outputJsonString = JsonSerializer.Deserialize<JsonObject>(s);
           }

           string output = string.IsNullOrWhiteSpace(o.Output) ? o.Input : o.Output;
           File.WriteAllText(output, JsonSerializer.Serialize(outputJsonString, new JsonSerializerOptions()
           {
               WriteIndented = true,
           }));
       });
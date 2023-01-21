using BlowFishDecoder;
using CommandLine;
using CompuTop.Core.Crypto;

Parser.Default
      .ParseArguments<Options>(args)
      .WithParsed(o =>
       {
           string text = new BlowFish(o.Key, new HexCoding()).DecryptECB(o.Data, o.Len);
           if (text.Length != o.Len)
           {
               Console.WriteLine($"[WARN]: decoded length {text.Length}");
           }

           Console.WriteLine(text);
       });
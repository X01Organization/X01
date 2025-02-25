using X01.App.Json.Deserializer;
using X01.CmdLine;

var ioCmdLineOption = new CmdLineArgsParser().Parse<Option>(args);
await new JsonDeserializer().DeserializeAsync(ioCmdLineOption, default);

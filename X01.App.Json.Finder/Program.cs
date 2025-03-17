using X01.App.Json.Deserializer;
using X01.CmdLine;

Option ioCmdLineOption = new CmdLineArgsParser().Parse<Option>(args);
await new JsonDeserializer().DeserializeAsync(ioCmdLineOption, default);

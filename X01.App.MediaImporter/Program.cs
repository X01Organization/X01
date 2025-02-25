using X01.App.MediaImporter;
using X01.CmdLine;

var option = new CmdLineArgsParser().Parse<Option>(args);
await new MediaImporter().ImportAsync(option, default);

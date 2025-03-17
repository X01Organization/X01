using X01.App.MediaImporter;
using X01.CmdLine;

Option option = new CmdLineArgsParser().Parse<Option>(args);
await new MediaImporter().ImportAsync(option, default);

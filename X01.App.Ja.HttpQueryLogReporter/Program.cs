using X01.App.Ja.HttpQueryLogReporter;
using X01.CmdLine;

Option option = new CmdLineArgsParser().Parse<Option>(args);

await new Reporter(option).ReportAsync(default);

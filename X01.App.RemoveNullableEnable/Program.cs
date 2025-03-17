// See https://aka.ms/new-console-template for more information
using X01.App.RemoveNullableEnable;
using X01.CmdLine;

Option option = new CmdLineArgsParser().Parse<Option>(args);
await new NullableRemover().RemoveAsync(option, default);

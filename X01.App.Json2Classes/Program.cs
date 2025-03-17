using Json2Class;
using Json2Classes;
using X01.CmdLine;

Option option = new CmdLineArgsParser().Parse<Option>(args);
   new Json2ClassConverter(option).DoJobAsync().GetAwaiter().GetResult(); 
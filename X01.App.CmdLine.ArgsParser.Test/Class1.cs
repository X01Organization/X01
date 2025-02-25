using System.Diagnostics;

namespace X01.App.CmdLine.ArgsParser.Test;

internal class Class1
{

        public  decimal RoundTo99( decimal value)
    {
        value =Math.Ceiling(value); 
        return value - (value % 100) + 99;
    }





    public void Test()
    {
        int i =0;
        while(i < 1000000)
        { 
            Test1();
            Test2();
            ++i;}
        Thread.Sleep(1000000000);
    }

    public void Test1()
    { 
        Task.Run(()=>{
        Debug.WriteLine("entering 1");

        int a = 3 + 4;

        lock(a as object)
        {
            Thread.Sleep(1000000000);
            Debug.WriteLine("entered 1");
        }

        Debug.WriteLine("exit 1");
        });
    }

    public void Test2()
    { 
        Task.Run(()=>{
        Debug.WriteLine("entering 2");

        int a = 1 + 6;

        lock(a as object)
        {
            Debug.WriteLine("entered 2");
        }

        Debug.WriteLine("exit 2");
        });
    }

}

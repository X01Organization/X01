using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace createsql;
public class Class2
{
    public async Task<int> TestAsync(){

        Task t = Test1Async();
        Task<Task> t1 = t.ContinueWith(async x=>  await Test2Async());
        await t1;
        await t;
        return 1;
    }

    private async Task Test1Async(){ 
        throw new Exception("test1");
    }

    private async Task Test2Async(){ 
        throw new Exception("test2");
    }

}

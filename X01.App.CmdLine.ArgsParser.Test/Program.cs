// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using X01.App.CmdLine.ArgsParser.Test;
using X01.CmdLine;


  IEnumerable<int> numbers = [  1, 2, 3, 4, 5 ];

        // Convert IEnumerable to IQueryable
        IQueryable<int> queryableNumbers = numbers.AsQueryable();

// Use IQueryable (e.g., for LINQ queries)
IQueryable<int> evenNumbers = queryableNumbers.Where(n => n % 2 == 0);

        // Output the results
        foreach (int number in evenNumbers)
        {
            Console.WriteLine(number);
        }

decimal a11 = 0m;
bool ssss341= false;

switch (a11)
{
    case 0:
    case < 0 when ssss341:
        Console.WriteLine("11111");
        break;
    case > 0:
        Console.WriteLine("2222");
        break;
}


double? n = null;
bool tt1 = n > 4;
bool tt2 = 4 > n;
bool tt3 = n > n;

string ttt123 = await File.ReadAllTextAsync("C:\\workroot\\json\\0.json");
JsonNode? jsonNode = JsonSerializer.Deserialize<JsonNode>(ttt123);
foreach (JsonNode? x1 in jsonNode!.AsArray())
{
    x1.GetPropertyName();
}

int ii = 0;
while (ii < int.MaxValue)
{
    //var sa123 = new testStream(@"C:\workroot\data\downloads\Dev Meeting-20240202_103246-Meeting Recording.mp4", FileMode.Open, FileAccess.Read, FileShare.Read);
    JsonContent sf1 = new JsonContent("{}");
    GC.Collect();
    GC.Collect(3);
    Console.WriteLine($"=>>>> {ii++}");
}





int a111 = 3;
uint s = (uint) a111 - 4;

string[] lines1 = await File.ReadAllLinesAsync("C:\\workroot\\project\\dotnetcore-tools\\Nirvana\\bin\\Debug\\net8.0\\log\\error.txt");
var tttest =
lines1.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => new { line = x, match = Regex.Match(x, @".*\((.+)\).+\((.+)\).*") }).Select(x =>
{
    if (!x.match.Success)
    {
        throw new Exception();
    }

    return new
    {
        travelcode = x.match.Groups[1].ToString(),
        day = x.match.Groups[2].ToString(),
    };
})
//.Where(x=> !x.travelcode.StartsWith("nl") && !x.travelcode.StartsWith("en"))
.Select(x => new { travelcode = x.travelcode.Substring(6), x.day, })
.Distinct()
.OrderBy(x => x.travelcode)
.ToArray();
string csv = string.Join(Environment.NewLine, tttest.Select(x => $"{x.travelcode};{x.day}").Prepend("TravelCode;Day"));

string ttt = await File.ReadAllTextAsync("C:\\workroot\\data\\downloads\\test9.json");
JsonNode? ss = JsonSerializer.Deserialize<JsonNode>(ttt);

string[] ttt1 = ss.AsArray().Select(x => x.AsObject()["line"]).Select(x => new
{
    type = x.GetType(),
    value = x.GetValue<string>(),
    json = JsonSerializer.Deserialize<JsonNode>(x.GetValue<string>()),
    x
}).Select(x => x.json["message"].GetValue<string>()).ToArray();
new Class1().Test();


string timePeriodsString = " 8~10 10:30~12 13~17:25 18:10~19:05 18:00~10 ";

Regex matchRegex = new Regex(@"\s*?(?<Delimiter>[\s,|;])\s*(?<StartHour>\d{1,2})(?::(?<StartMinute>\d{1,2}))?\s*~\s*(?<EndHour>\d{1,2})(?::(?<EndMinute>\d{1,2}))?\s*?");
//var matchRegex = new Regex(@"\b(?<StartHour>\d{1,2})(?::(?<StartMinute>\d{1,2}))?\s*~\s*(?<EndHour>\d{1,2})(?::(?<EndMinute>\d{1,2}))?\b"); 
MatchCollection matches = matchRegex.Matches(" " + timePeriodsString);



string[] lines = await File.ReadAllLinesAsync("C:\\workroot\\1233425647");
string[] ttes = lines.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Split('\t').Take(3).ToArray())
    .Select(x => "{\"" + x[0].Trim() + "\", " + x[2] + "}" + " // " + x[1])
    .ToArray();
string tt = string.Join(Environment.NewLine, ttes);
//using Adyen;

//	using Adyen;
//	using Adyen.Model.Checkout;
//	using Adyen.Service;
//	using Environment = Adyen.Model.Environment;
//using Adyen.Service.Checkout;


//https://docs.adyen.com/development-resources/libraries/


//// 创建支付请求对象
//var paymentRequest = new PaymentRequest
//{
//    Amount = new Amount
//    {
//        Currency = "EUR",
//        Value = 1000 // 金额，以最小货币单位表示（如欧元的分）
//    },
//    Reference = "YOUR_ORDER_REFERENCE",
//    PaymentMethod = new PaymentMethod
//    {
//        Type = PaymentMethodType.Scheme,
//        EncryptedCard = "YOUR_ENCRYPTED_CARD_DATA" // 加密的信用卡信息
//    },
//    MerchantAccount = "YOUR_MERCHANT_ACCOUNT",
//    ReturnUrl = "https://yourwebsite.com/checkout/confirmation" // 完成支付后返回的 URL
//};
//var client = new Client(config);
//var checkout = new PaymentsService(client);
//// 发起支付请求
//var paymentResponse = checkout.Payments(paymentRequest);

//// 获取支付链接
//var paymentLink = paymentResponse.PaymentLink;

//var test = new [] 
//{"0.5 hours","1 hour","1.5 hours","2 hours","2.5 hours","3 hours","3.5 hours","4 hours","4.5 hours","5 hours","5.5 hours","6 hours","6.5 hours","7 hours","7.5 hours","8 hours","8.5 hours","9 hours","9.5 hours","10 hours","10.5 hours","11 hours","11.5 hours","12 hours","half day","full day","two days","three days" } ;

//var te1 = test.OrderBy(x=> x)
//    .Select(x=> 
//    {var left =  "  \"contentful.activity.Duration."+ x +"\": {" + Environment.NewLine +
//                 "    \"de"

//        return left ;
//    }).ToArray();

Regex ss1 = new Regex(@"^\s*((\s(?<Hour>\d{1,2})(?::(?<Minute>\d{1,2}))?\s*~\s*(\d{1,2})(?::(\d{1,2}))?)*)\s*$");
Match m = ss1.Match("   8:01~10 10:30~12 13~17:25 18:10~19:05  ");

Group ss11 = m.Groups["Hour"];
Group ss22 = m.Groups["Minute"];




Console.WriteLine(string.Join(Environment.NewLine, args.Select(x => $"<{x}>")));

new CmdLineArgsParser().Parse<Option>(args);


//X01.App.CmdLine.ArgsParser.Test --help --version --input=/c/434 --input=/c/577 --output=/c/test/5667 --no-trim -x --env dev -o no-compile -t "-333"


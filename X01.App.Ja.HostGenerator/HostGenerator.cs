namespace X01.App.Ja.HostGenerator;
public class HostGenerator
{
    public async Task GenerateAsync(CancellationToken token)
    { 
        var hostLines = await   File.ReadAllLinesAsync("C:\\workroot\\env\\doc\\journaway\\hosts", token);
        var hostInfos = 
        hostLines.Select(x=> x.Split('\t', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Where(x=> x.Length == 4)
            .Select(x=> new { AliseName = x[1], Name = GetName(x[1]) , Domain = x[2], Ip = x[3],})
            .OrderBy(x=> x.Name)
            .ToArray();

        var hosts = string.Join('\n',  hostInfos.Select(x=> $"{x.Ip.PadRight(3*4+3+1)} {x.Domain}"));
        hosts = @"127.0.0.1 localhost
127.0.1.1 x01

# The following lines are desirable for IPv6 capable hosts
::1     ip6-localhost ip6-loopback
fe00::0 ip6-localnet
ff00::0 ip6-mcastprefix
ff02::1 ip6-allnodes
ff02::2 ip6-allrouters

# journaway
" + hosts;
        hosts = hosts.ReplaceLineEndings("\n");
        await File.WriteAllTextAsync("C:\\workroot\\env\\sys\\linux\\xhome\\hosts", hosts);


        var names = string.Join(Environment.NewLine, hostInfos.Select(x=>"{" + $"\"{x.Name}\",".PadRight(20)+" \"\"" + "},").ToList());

        var config = string.Join('\n',  hostInfos.Select(x=> $@"Host {x.Name}
    HostName {x.Domain}
    ServerAliveInterval 60
    User zxiang 
    PubKeyAuthentication yes
    IdentityFile /c/workroot/env/auth/rsa/journaway/db/id_rsa_no_pwd
"));
        config += @"
###################################################################################
Host synology
    HostName  journaway.synology.me
    Port 2222
    ServerAliveInterval 60
    PasswordAuthentication yes
    User journaway
    #Password handelshafen1! 
    #ProxyCommand sshpass -p 'handelshafen1!' ssh -q -o StrictHostKeyChecking=no %h -l %r
#####################################################################################
";
        config = config.ReplaceLineEndings("\n").Trim();
        await File.WriteAllTextAsync("C:\\workroot\\env\\sys\\linux\\xhome\\.ssh\\config", config);
    }

    private string GetName(string name)
    { 
        var dict = new Dictionary<string, string>(){ 
{"TEST",              "test_svc"}             ,
{"Product",           "product"}             ,
{"Grafana",           "grafana"}                     ,
{"Loki",              "loki"}                        ,
{"Prometheus",        "prometheus"}                  ,
{"Dotnetsvc UK",      "live_gb_svc"}                 ,
{"B2B UK",            "live_gb_b2b"}                 ,
{"Nirvana Test",      "test_nirvana"}                ,
{"Nirvana",           "live_nirvana"}                ,
{"B2B NL",            "live_nl_b2b"}                 ,
{"Dotnetsvc NL",      "live_nl_svc"}                 ,
{"Proget",            "proget"}                      ,
{"Flightsvc",         "live_de_flight_svc"}             ,
{"B2B",               "live_b2b_de"}                 ,
{"Flightsvc Test",    "test_flight_svc"}             ,
{"Redis Test",        "test_redis"}                  ,
{"Redis",             "live_de_redis"}               ,
{"Redis NL",          "live_nl_redis"}               ,
{"Expedia Test",      "test_expedia"}                ,
{"Postgres",          "live_de_postgres"}            ,
{"Postgres UK",       "live_gb_postgres"}            ,
{"Postgres NL",       "live_nl_postgres"}            ,
{"Postgres Flights",  "live_de_flight_postgres"}     ,
{"B2B Test",          "test_b2b"}                    ,
{"Postgres Test",     "test_postgres"}               ,
{"NGINX",             "nginx"}                       ,
{"Dotnetsvc",         "live_de_svc"}                 ,
{"NGINX Test",        "test_nginx"}                  ,
        };

        var test= dict.ToDictionary(x=> x.Value, x=> x.Key);
        return dict[ name];
    }
}

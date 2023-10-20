using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Json2Class
{
    public class Json2ClassConverter
    {
        private readonly Options _options;

        public Json2ClassConverter(Options options)
        {
            _options = options;
        }

        public async Task DoJobAsync(CancellationToken token = default)
        {
            var outputDirectoryInfo = new DirectoryInfo(_options.OutputDirectory);
            if (outputDirectoryInfo.Exists)
            {
                outputDirectoryInfo.Delete(true);
            }

            outputDirectoryInfo.Create();

#region TEST

#if true
            await foreach (var json in GetJsonsAsync(false, token))
            {
            }

#endif

#endregion

            Dictionary<string, ClassInfo> classInfoDictionary = new Dictionary<string, ClassInfo>();
            try
            {
                await foreach (var json in GetJsonsAsync(true, token))
                {
                    await using var ms = new MemoryStream();
                    using var sw = new StreamWriter(ms);
                    sw.Write(json);
                    sw.Flush();
                    ms.Position = 0;
                    using var jsonDocument = await JsonDocument.ParseAsync(ms, cancellationToken: token);

                    await ConvertAsync(classInfoDictionary, GetClassName(_options.ClassName), jsonDocument.RootElement,
                        token);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            foreach (var x in classInfoDictionary.Values)
            {
                token.ThrowIfCancellationRequested();
                await WriteClassAsync(x, token);
            }
        }

        private async IAsyncEnumerable<string> GetJsonsAsync(bool fromFile,
            [EnumeratorCancellation] CancellationToken token)
        {
            if (fromFile)
            {
                yield return await File.ReadAllTextAsync(_options.InputFile, token);
                yield break;
            }

            using var client = new HttpClient();
            var tripIds =
                "46397,46573,46751,46924,47153,47188,47383,48530,48657,48684,48738,48753,48802,48844,48877,48905,48907,48947,49010,49014,49071,49139,49265,49417,49571,49572,49607,49621,49622,49642,49673,49676,49685,49717,49751,49752,49757,49760,49787,50230,50241,50246,50254,50299,50321,50328,50698,50747,50756,51619,51620,51621,51632,51743,51744,51745,51779,51780,51811,51825,51826,51865,52065,52196,52212,52272,52334,52335,52344,52425,52519,52557,52644,52645,52726,52740,52774,52810,52824,52895,52923,52965,53013,53094,53109,53147,53182,53191,53231,53256,53306,53318,53483,53484,53505,53509,53585,53645,53806,53863,53871,53910,53927,53932,53947,53948,53958,53959,53960,53961,53962,53974,53976,54011,54013,54032,54033,54034,54055,54075,54082,54083,54144,54149,54162,54203,54270,54288,54697,54735,54751,54778,54792,54793,54794,54888,54889,55196,55246,55435,55441,55456,55457,55460,55466,55468,55475,55481,55503,55516,55541,55546,55547,55585,55608,55609,55616,55683,55737,55751,55754,55780,55786,55787,55804,55911,55914,55921,55930,55937,55946,55947,55948,55982,56000,56031,56058,56066,56069,56075,56091,56138,56173,56174,56194,56195,56210,56220,56237,56243,56244,56290,56291,56304,56308,56311,56323,56345,56425,56426,56453,56480,56495,56507,56508,56521,56522,56549,56554,56563,56569,56574,56579,56580,56611,56612,56613,56623,56624,56632,56643,56694,56716,56754,56755,56761,56762,56770,56781,56791,56830,57001,57004,57005,57034,57053,57054,57091,57129,57165,57188,57265,57278,57300,57322,57349,57350,57455,57456,57457,57478,57479,57481,57549,57570,57571,57575,57576,57657,57659,57680,57724,57725,57726,57731,57732,57733,57749,57889,58108,58122,58166,58191,58197,58208,58233,58250,58254,58268,58304,58367,58383,58384,58385,58410,58411,58428,58429,58434,58435,58480,58481,58482,58506,58507,58551,58571,58597,58598,58632,58642,58664,58676,58677,58678,58687,58694,58696,58722,58748,58749,58750,58960,58981,58982,58983,59040,59043,59044,59045,59062,59063,59078,59079,59080,59103,59123,59124,59170,59187,59188,59254,59259,59268,59308,59309,59310,59311,59312,59318,59319,59334,59335,59336,59375,59380,59397,59398,59399,59403,59414,59415,59416,59432,59434,59435,59436,59437,59441,59442,59444,59445,59464,59465,59468,59469,59470,59471,59477,59478,59479,59480,59524,59542,59576,59577,59578,59579,59580,59584,59594,59595,59621,59624,59631,59633,59634,59635,59636,59637,59638,59652,59654,59655,59659,59665,59688,59689,59694,59717,59721,59722,59723,59735,59746,59748,59749,59763,59764,59765,59766,59767,59990,60014,60025,60026,60027,60157,60173,60174,60175,60176,60177,60184,60185,60186,60198,60211,60234,60235,60273,60539,60540,60570,60578,60579,60597,60599,60600,60703,60723,60741,60742,60749,60750,60751,60752,60753,60772,60773,60790,60791,60792,60793,60799,60840,60841,60843,60844,60845,60846,60856,60910,60926,60927,60928,60929,60943,60944,60958,60967,60975,61026,61027,61028,61045,61047,61083,61133,61134,61145,61146,61147,61148,61149,61164,61261,61262,61276,61277,61292,61293,61352,61360,61361,61409,61410,61411,61412,61428,61429,61430,61431,61449,61450,61473,61497,61531,61541,61542,61543,61544,61545,61546,61565,61566,61567,61568,61569"
                   .Split(',')
                   .Distinct()
                   .OrderBy(x => x)
                   .ToArray();
            int i = 0;
            foreach (var tripId in tripIds)
            {
                var cacheFilename = $"c:/workroot/tmp/{tripId}.json";
                if (File.Exists(cacheFilename))
                {
                    ++i;
                    yield return File.ReadAllText(cacheFilename);
                }
                else
                {
                    var response =
                        await client.GetAsync($"https://localhost:1991/valhalla/getTrip?tripId={tripId}&culture=de-DE");
                    //var response = await client.GetAsync($"https://www.journaway.com/de/api/schema?trip={tripId}");
                    //var response = await client.GetAsync($"https://www.journaway.com/de/api/tripdayinfo?trip={tripId}");
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"{++i}/{tripIds.Length} ==> {tripId}");
                    await File.WriteAllTextAsync(cacheFilename, json, token);
                    yield return json;
                }
            }
        }

        private async Task ConvertAsync(Dictionary<string, ClassInfo> classInfoDictionary,
            string className,
            JsonElement jsonElement,
            CancellationToken token)
        {
            switch (jsonElement.ValueKind)
            {
                case JsonValueKind.Object:
                    await ConvertObjectAsync(classInfoDictionary, className, jsonElement, token);
                    return;
                case JsonValueKind.Array:
                    await ConvertArrayAsync(classInfoDictionary, className, jsonElement, token);
                    return;
            }

            throw new ArgumentOutOfRangeException($"JsonValueKind.({jsonElement.ValueKind})");
        }

        private async Task ConvertObjectAsync(Dictionary<string, ClassInfo> classInfoDictionary,
            string className,
            JsonElement jsonElement,
            CancellationToken token)
        {
            if (JsonValueKind.Object != jsonElement.ValueKind)
            {
                throw new InvalidOperationException("must be JsonValueKind.Object");
            }

            if (!classInfoDictionary.TryGetValue(className, out var classInfo))
            {
                classInfo = CreateClassInfo(className);
                classInfoDictionary.Add(className, classInfo);
            }

            foreach (var x in jsonElement.EnumerateObject())
            {
                token.ThrowIfCancellationRequested();
                var propertyInfo = new PropertyInfo1()
                {
                    Name = GetClassName(x.Name),
                    JsonName = x.Name,
                };

                if (JsonValueKind.Array == x.Value.ValueKind)
                {
                    if (x.Value.EnumerateArray().Any())
                    {
                        var elementsValueKind = x.Value.EnumerateArray()
                                                 .Select(y => y.ValueKind)
                                                 .Distinct()
                                                 .Single();
                        if (GetBasePropertyTypeName(elementsValueKind) is string basePropertyTypeName1)
                        {
                            propertyInfo.TypeName = basePropertyTypeName1 + "[]";
                        }
                        else
                        {
                            var propertyClassName = className + propertyInfo.Name;
                            propertyInfo.TypeName = propertyClassName + "[]";
                            await ConvertAsync(classInfoDictionary, propertyClassName, x.Value, token);
                        }
                    }
                    else
                    {
                        propertyInfo.TypeName = "[]";
                    }
                }

                if (string.IsNullOrEmpty(propertyInfo.TypeName))
                {
                    if (GetBasePropertyTypeName(x.Value.ValueKind) is string basePropertyTypeName)
                    {
                        propertyInfo.TypeName = basePropertyTypeName;
                    }
                    else
                    {
                        propertyInfo.TypeName = className + propertyInfo.Name;
                        await ConvertAsync(classInfoDictionary, propertyInfo.TypeName, x.Value, token);
                    }
                }

                classInfo.Properties.Add(propertyInfo);
            }
        }

        private async Task ConvertArrayAsync(Dictionary<string, ClassInfo> classInfoDictionary,
            string className,
            JsonElement jsonElement,
            CancellationToken token)
        {
            if (JsonValueKind.Array != jsonElement.ValueKind)
            {
                throw new InvalidOperationException("must be JsonValueKind.Array");
            }

            var elements = jsonElement.EnumerateArray().ToArray();
            var elementsValueKind = elements.Select(x => x.ValueKind).Distinct().Single();

            if (GetBasePropertyTypeName(elements[0].ValueKind) is string)
            {
                throw new InvalidOperationException("should be handled");
            }

            if (!classInfoDictionary.TryGetValue(className, out var classInfo))
            {
                classInfo = CreateClassInfo(className);
                classInfoDictionary.Add(className, classInfo);
            }

            foreach (var x in elements)
            {
                token.ThrowIfCancellationRequested();

                await ConvertAsync(classInfoDictionary, className, x, token);
            }
        }

        private ClassInfo CreateClassInfo(string className)
        {
            var classInfo = new ClassInfo()
            {
                Name = className,
                NameSpace = _options.Namespace,
            };
            classInfo.Usings.Add("System.Text.Json.Serialization");
            return classInfo;
        }

        private string GetBasePropertyTypeName(JsonValueKind jsonValueKind)
        {
            switch (jsonValueKind)
            {
                case JsonValueKind.String:
                    return "string";
                case JsonValueKind.Number:
                    return "decimal";
                case JsonValueKind.True:
                    return "bool";
                case JsonValueKind.False:
                    return "bool";
                case JsonValueKind.Null:
                    return "null";
                case JsonValueKind.Undefined:
                    throw new ArgumentOutOfRangeException($"JsonValueKind.({jsonValueKind})");
            }

            return null;
        }

        private async Task WriteClassAsync(ClassInfo classInfo, CancellationToken token)
        {
            if (string.IsNullOrEmpty(classInfo.NameSpace))
            {
                throw new InvalidOperationException("Missing namespace");
            }

            if (string.IsNullOrEmpty(classInfo.Name))
            {
                throw new InvalidOperationException("Missing classname");
            }

            if (1 > classInfo.Properties.Count)
            {
                throw new InvalidOperationException("Missing property");
            }

            var invalidProperties = classInfo.Properties
                                             .Where(x => string.IsNullOrWhiteSpace(x.Name) ||
                                                         string.IsNullOrWhiteSpace(x.JsonName) ||
                                                         string.IsNullOrWhiteSpace(x.TypeName))
                                             .ToArray();
            if (0 < invalidProperties.Length)
            {
                throw new InvalidOperationException("Wrong property");
            }

            var classFileName = Path.Combine(_options.OutputDirectory, classInfo.Name + ".cs");
            await using var fs = File.OpenWrite(classFileName);
            await using var sw = new StreamWriter(fs);
            foreach (var x in classInfo.Usings.Distinct().OrderBy(x => x))
            {
                token.ThrowIfCancellationRequested();
                await sw.WriteAsync("using " + x + ";");
                await sw.WriteLineAsync();
            }

            await sw.WriteLineAsync();
            await sw.WriteAsync("namespace " + classInfo.NameSpace + ";");
            await sw.WriteLineAsync();
            await sw.WriteLineAsync();

            await sw.WriteAsync("public class " + classInfo.Name);
            await sw.WriteLineAsync();
            await sw.WriteAsync("{");

            var allProperties = classInfo.Properties
                                         .DistinctBy(x => $"{x.Name}<>{x.JsonName}<>{x.TypeName}")
                                         .ToArray();

            foreach (var x in allProperties.Where(x => "null" == x.TypeName))
            {
                var typeNames = allProperties.Where(y => y.Name == x.Name &&
                                                         "null" != y.TypeName)
                                             .Select(x => x.TypeName)
                                             .Distinct()
                                             .ToArray();
                if (0 < typeNames.Length)
                {
                    x.TypeName = typeNames[0];
                }
                else
                {
                    x.TypeName = "string";
                }
            }

            foreach (var x in allProperties.Where(x => "[]" == x.TypeName))
            {
                x.TypeName = allProperties.FirstOrDefault(y => y.Name == x.Name && "[]" != y.TypeName)?.TypeName
                          ?? "string[]";
            }

            allProperties = allProperties
                           .DistinctBy(x => $"{x.Name}<>{x.JsonName}<>{x.TypeName}")
                           .ToArray();

            var notUniqueNames = allProperties.GroupBy(x => x.Name).Where(x => x.Skip(1).Any()).ToArray();
            if (0 < notUniqueNames.Length)
            {
                //throw new InvalidOperationException("Name must be unique");
            }

            var notUniqueJsonNames = allProperties.GroupBy(x => x.JsonName).Where(x => x.Skip(1).Any()).ToArray();
            if (0 < notUniqueJsonNames.Length)
            {
                //throw new InvalidOperationException("JsonName must be unique");
            }

            foreach (var x in allProperties.OrderBy(x => x.JsonName))
            {
                token.ThrowIfCancellationRequested();

                await sw.WriteLineAsync();
                await sw.WriteAsync("    [JsonPropertyName(\"" + x.JsonName + "\")]");
                await sw.WriteLineAsync();
                await sw.WriteAsync("    public " + x.TypeName + "? " + x.Name + " { get; set; }");
                await sw.WriteLineAsync();
            }

            await sw.WriteAsync("}");
        }

        private string GetClassName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("ClassName");
            }

            if (char.IsLower(name[0]))
            {
                return GetClassName(char.ToUpperInvariant(name[0]) + name.Substring(1));
            }

            if (name.Contains('-'))
            {
                return GetClassName(name.Replace("-", string.Empty));
            }

            if (name.Contains('@'))
            {
                return GetClassName(name.Replace("@", string.Empty));
            }

            return name;
        }
    }
}
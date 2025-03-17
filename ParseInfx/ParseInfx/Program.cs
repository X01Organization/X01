// See https://aka.ms/new-console-template for more information
string[] lines = File.ReadAllLines(@"C:\Temp\1.txt");
IEnumerable<string[]> infos = lines.Select(x=> x.Split(' ').Take(8).ToArray());
string names = string.Join(Environment.NewLine, infos.Select(x=> x[0]));
Dictionary<string, string> result = new() { 
{"Version","version"},
{"Marke","brand"},
{"Abflughafen_Hin","Departure_Airport"},
{"Zielflughafen_Hin","Destinationairport_hin"},
{"Abflughafen_Rück","Departure_Return"},
{"Zielflughafen_Rück","DestinationAirport_Return"},
{"Maschinenkennung","machineidentifier"},
{"Reiseart","Traveltype"},
{"Angebotstermin","offerdate"},
{"Angebotsdauer","offerduration"},
{"Leistungscodierung","powercoding"},
{"Zimmertyp","roomtype"},
{"Verpflegung","meals"},
{"Belegung","occupancy"},
{"Preis","Price"},
{"Airline","airline"},
{"Reiseart_lang","triptype_long"},
{"Hotelname","hotelname"},
{"Ort","location"},
{"Beschreibung","description"},
{"Kategorie","category"},
{"Katalogname","catalogname"},
{"Katalogseite","catalogpage"},
{"Restplätze","remainingplaces"},
{"Bildname","imagename"},
{"Reisetyp","traveltype"},
{"Anreise","gettingthere"},
{"Belegung_Min","occupancy_min"},
{"Belegung_Max","Occupancy_Max"},
{"Vollzahler_Min","fullpayer_min"},
{"Vollzahler_Max","fullpayer_max"},
{"Währung","currency"},
};
string ss = string.Join(Environment.NewLine , infos.Select(x=> $"/// <summary>{Environment.NewLine}/// {x[7].ToLowerInvariant() } {x[0]}{Environment.NewLine}/// [{(Convert.ToInt32( x[1])-1)},{(Convert.ToInt32( x[2])-1)}] {(Convert.ToInt32( x[3]))} {Environment.NewLine}/// </summary> {Environment.NewLine} public  string  {result[x[0]].Substring(0,1).ToUpperInvariant()}{result[x[0]].Substring(1)} " + "{get;set;}" ) );
string ss1 = string.Join(Environment.NewLine , infos.Select(x=> $"infxModel.{result[x[0]]} = InterpretAt(line,{(Convert.ToInt32( x[1])-1)},{(Convert.ToInt32( x[3]))} );"));
int a =0;

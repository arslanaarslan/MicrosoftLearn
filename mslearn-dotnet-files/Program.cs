using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

// var salesFiles = FindFiles("stores");

// foreach (var file in salesFiles)
// {
//   Console.WriteLine(file);
// }

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");
// Create a directory to store the sales totals
var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);
var salesFiles = FindFiles(storesDirectory);
// Read and Parse from json file
var salesTotal = CalculateSalesTotal(salesFiles);
// Create a totals.txt file in the salesTotalDir directory and write an empty string to it
File.WriteAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

// Console.WriteLine(salesData?.Total);

IEnumerable<string> FindFiles(string folderName)
{
  List<string> salesFiles = new List<string>();

  var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

  foreach (var file in foundFiles)
  {
    var extension = Path.GetExtension(file);
    // The file name will contain the full path, so only check the end of it
    // if (file.EndsWith("sales.json"))
    if (extension == ".json")
    {
      salesFiles.Add(file);
    }
  }

  return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
  double salesTotal = 0;

  // Loop over each file path in salesFiles
  foreach (var file in salesFiles)
  {
    // Read the contents of the file
    string salesJson = File.ReadAllText(file);

    // Parse the contents as JSON
    SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

    // Add the amount found in the Total field to the salesTotal variable
    salesTotal += data?.Total ?? 0;
  }

  return salesTotal;
}

record SalesData(double Total);
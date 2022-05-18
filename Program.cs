using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZSI.services;

namespace ZSI
{
    class Program
    {
        public static TSPService service = new TSPService();
        static void Main(string[] args)
        {
            Console.WriteLine("Podaj ścieżkę pliku: ");
            string path = Console.ReadLine();
            
            if (String.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                Console.WriteLine("Nieprawidłowa ścieżka!");
                return;
            }

            int populationRange = 40;
            int numberOfBestFittingcontestants = 25;
            int probCrossing = 90;
            int probMutation = 5;

            int range = 0;
            StringBuilder errors = new StringBuilder();

            service.Cities = getDataFromFile(path, ref range, ref errors);
            if (errors.Length > 0)
            {
                Console.WriteLine("Wystąpiły błędy: " + errors.ToString());
            }

            int bestRating = 0;
            var bestContestant = service.TSP(populationRange, probCrossing, probMutation, numberOfBestFittingcontestants, out bestRating);
        }

        public static List<int[]> getDataFromFile(string filePath, ref int populationRange, ref StringBuilder errors)
        {
            try
            {
                List<int[]> population = new List<int[]>();

                if (!File.Exists(filePath))
                {
                    errors.AppendLine($"Nie znaleziono pliku o ścieżce {filePath}!");
                    return population;
                }

                string[] data = File.ReadAllLines(filePath);
                int range = 0;

                if (data != null && data.Length > 0)
                {
                    var result = int.TryParse(data[0].Trim(), out range);
                    if (!result)
                    {
                        errors.AppendLine("Błędny zakres!");
                        return population;
                    }

                    for(var i = 1; i < data.Length; i++)
                    {
                        string[] splitedLine = data[i].Split(" ").Where(x => !String.IsNullOrWhiteSpace(x)).ToArray();
                        List<int> intervals = new List<int>();

                        foreach(var interval in splitedLine)
                        {
                            result = int.TryParse(interval.Trim(), out int convertedValue);
                            if (result)
                                intervals.Add(convertedValue);
                        }

                        if (intervals.Count > 0)
                        {
                            population.Add(intervals.ToArray());
                        }
                        else
                        {
                            range--;
                        }
                    }
                }

                populationRange = range;

                return population;
            }
            catch(Exception ex)
            {
                errors.AppendLine("Błąd: " + ex.Message);
                return new List<int[]>();
            }
        }
    }
}

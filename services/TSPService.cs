using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZSI.services
{
    public class TSPService
    {
        public List<int[]> Cities { get; set; }
        public int CitiesCount => Cities.Count();

        public int[] TSP(int populationRange, int probCrossing, int probMutation, int numberOfBestFittingcontestants, out int bestRating)
        {
            bestRating = 0;
            try
            {
                Random rnd = new Random();
                int t = 0;
                var basePopulation = GenerateBasePopulation(populationRange, rnd);
                var ratings = RatePopulation(basePopulation);

                var bestContestant = GetBestContestant(basePopulation);
                bestRating = RatePopulation(new List<int[]> { bestContestant }).First();

                Console.WriteLine("\nNumery miast: " + string.Join(" - ", bestContestant));
                Console.WriteLine("Długość trasy: " + bestRating);

                while (t < 1000000) 
                {
                    basePopulation = RankSelection(basePopulation, ratings, 10);
                    basePopulation = CrossingOX(basePopulation, rnd, probCrossing);
                    basePopulation = MutationByInversion(basePopulation, probMutation, rnd);
                    ratings = RatePopulation(basePopulation);

                    var tempBestContestant = GetBestContestant(basePopulation);
                    var tempBestRating = RatePopulation(new List<int[]> { tempBestContestant }).First();
                    if (tempBestRating < bestRating)
                    {
                        bestContestant = tempBestContestant;
                        bestRating = tempBestRating;
                        Console.WriteLine("\nNumery miast: " + string.Join(" - ", bestContestant));
                        Console.WriteLine("Długość trasy: " + bestRating + $" (iteracja {t})");
                    }

                    t++;
                }

                return bestContestant;
            }
            catch(Exception ex)
            {
                return new int[] { };
            }
        }

        public List<int[]> GenerateBasePopulation(int populationRange, Random rnd)
        {
            try
            {
                List<int[]> basePopulation = new List<int[]>();
                List<int> citiesNumbers = new List<int>();

                for (int i = 0; i < Cities.Count; i++)
                    citiesNumbers.Add(i);

                for (int i = 0; i < populationRange; i++)
                {
                    var route = citiesNumbers.OrderBy<int, int>(x => rnd.Next());
                    basePopulation.Add(route.ToArray());
                }

                return basePopulation;
            }
            catch(Exception ex)
            {
                return new List<int[]>();
            }
        }

        public List<int> RatePopulation(List<int[]> population)
        {
            try
            {
                List<int> ratings = new List<int>();
                foreach(var route in population)
                {
                    var distances = new int[CitiesCount];
                    for (var j = 0; j < CitiesCount; j++)
                    {
                        var city = route[j];
                        var nextCity = CitiesCount - 1 > j ? route[j + 1] : route[0];

                        if (city > nextCity)
                            distances[j] = Cities[city][nextCity];
                        else
                            distances[j] = Cities[nextCity][city];
                    }
                    ratings.Add(distances.Sum());
                }

                return ratings;
            }
            catch(Exception ex)
            {
                return new List<int>();
            }
        }

        public int[] GetBestContestant(List<int[]> population)
        {
            try
            {
                var ratings = RatePopulation(population);
                var sortedPopulation = RankSelection(population, ratings, 20);
                return sortedPopulation.First();
            }
            catch(Exception ex)
            {
                return new int[] { };
            }
        }

        public List<int[]> RankSelection(List<int[]> population, List<int> ratings, int numberOfBestFittingcontestants)
        {
            try
            {
                var basePopCount = population.Count;
                List<int[]> outputPopulation = new List<int[]>();
                List<Tuple<int[], int>> populationWithRatings = new List<Tuple<int[], int>>();
                for (var i = 0; i < basePopCount; i++)
                {
                    populationWithRatings.Add(new Tuple<int[], int>(population[i], ratings[i]));
                }

                var bestContestants = populationWithRatings.OrderBy(x => x.Item2)
                    .Select(x => x.Item1).Take(numberOfBestFittingcontestants).ToList();

                var counter = basePopCount / numberOfBestFittingcontestants;
                if (basePopCount % numberOfBestFittingcontestants == 0)
                    counter++;

                for(var i = 0; i < counter; i++)
                {
                    outputPopulation.AddRange(bestContestants);
                }

                if (basePopCount % numberOfBestFittingcontestants == 0)
                    outputPopulation = outputPopulation.Take(basePopCount).ToList();

                return outputPopulation;
            }
            catch (Exception ex)
            {
                return new List<int[]>();
            }
        }

        public List<int[]> CrossingOX(List<int[]> population, Random random, int crossingProb)
        {
            try
            {
                var outputPopulation = new List<int[]>();
                var firstCrossingPoint = 0;
                var secondCrossingPoint = 0;
                var isCrossing = random.Next(1, 101);

                var n = population.Count % 2 == 0 ? population.Count : population.Count - 1;

                for (int i = 0; i < n; i += 2)
                {
                    var popLen = population[i].Length;
                    var rodzic1 = new int[popLen];
                    var rodzic2 = new int[popLen];
                    population[i].CopyTo(rodzic1, 0);
                    population[i + 1].CopyTo(rodzic2, 0);
                    firstCrossingPoint = random.Next(1, popLen / 2 - 1);
                    secondCrossingPoint = random.Next(firstCrossingPoint + 1, popLen - 1);

                    var potomek1 = new int[popLen];
                    var potomek2 = new int[popLen];

                    if (isCrossing <= crossingProb)
                    {
                        for(var j = firstCrossingPoint; j <= secondCrossingPoint; j++)
                        {
                            potomek1[j] = rodzic1[j];
                            potomek2[j] = rodzic2[j];
                        }

                        var rodzic2WithoutPotomek1 = rodzic2.Except(potomek1).ToList();
                        var rodzic1WithoutPotomek2 = rodzic1.Except(potomek2).ToList();
                        for (var j = 0; j < firstCrossingPoint; j++)
                        {
                            if (rodzic2WithoutPotomek1.Any())
                            {
                                potomek1[j] = rodzic2WithoutPotomek1.First();
                                rodzic2WithoutPotomek1 = rodzic2WithoutPotomek1.Except(potomek1).ToList();
                            }
                            if (rodzic1WithoutPotomek2.Any())
                            {
                                potomek2[j] = rodzic1WithoutPotomek2.First(); 
                                rodzic1WithoutPotomek2 = rodzic1WithoutPotomek2.Except(potomek2).ToList();
                            }
                        }
                        for (var j = secondCrossingPoint + 1; j < potomek1.Count(); j++)
                        {
                            if (rodzic2WithoutPotomek1.Any())
                            {
                                potomek1[j] = rodzic2WithoutPotomek1.First();
                                rodzic2WithoutPotomek1 = rodzic2WithoutPotomek1.Except(potomek1).ToList();
                            }
                            if (rodzic1WithoutPotomek2.Any())
                            {
                                potomek2[j] = rodzic1WithoutPotomek2.First();
                                rodzic1WithoutPotomek2 = rodzic1WithoutPotomek2.Except(potomek2).ToList();
                            }
                        }

                        outputPopulation.Add(potomek1);
                        outputPopulation.Add(potomek2);
                    }
                    else
                    {
                        outputPopulation.Add(rodzic1);
                        outputPopulation.Add(rodzic2);
                    }
                }

                return outputPopulation;
            }
            catch (Exception ex)
            {
                return new List<int[]>();
            }
        }

        public List<int[]> MutationByInversion(List<int[]> population, int mutationProb, Random random)
        {
            try
            {
                var isMutate = 0;
                var firstIndex = 0;
                var secondIndex = 0;

                foreach(var route in population)
                {
                    isMutate = random.Next(1, 101);
                    if (isMutate <= mutationProb)
                    {
                        firstIndex = random.Next(0, route.Length/2 - 1);
                        secondIndex = random.Next(firstIndex + 1, route.Length - 1);

                        Array.Reverse(route, firstIndex, (secondIndex - firstIndex + 1));
                    }
                }

                return population;
            }
            catch(Exception ex)
            {
                return new List<int[]>();
            }
        }
    }
}

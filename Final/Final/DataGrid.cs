﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final
{
    internal class DataGrid
    {
        public static double deltaT { get; set; } = 1;
        public double totalT { get; set; }
        public static double sumT = 0;
        public float k { get; set; }// fraction of infected group that will recover
        public float b { get; set; }
        public float deathRate { get; set;}// odds of an infected person dying after leaving the infected group (infectious, really)
        public  float borderTravelRate;
        public float airportTravelRate;
        public static List<GridUnit> activeGridUnits = new List<GridUnit>();
        public static List<GridUnit> airportGridUnits = new List<GridUnit>();
        public string startingCity = "Los Angeles";


        public static DataParser parser = new DataParser();
        public static int gridXLength = parser.width;
        public static int gridYLength = parser.height;
        public static GridUnit[,] array = new GridUnit[gridXLength, gridYLength];

        public static long totalInfected = 0;
        public static long totalDead = 0;
        public static long totalRecovered = 0;



        public DataGrid()
        {
            
        }
        
        public long getTotalInfected()
        {
            return totalInfected;
        }
        public long getTotalDead()
        {
            return totalDead;
        }
        public long getTotalRecovered()
        {
            return totalRecovered;
        }

        public DataGrid( double totalT, float k, float deathRate, float borderTravelRate, float airportTravelRate, string startingCity, float b)
        {
            this.b = b;
            this.totalT = totalT;
            this.k = k;
            this.deathRate = deathRate;
            this.borderTravelRate = borderTravelRate;
            this.airportTravelRate = airportTravelRate;
            this.startingCity = startingCity;

            
            //activeGridUnits.ElementAt(0).i = .01;

            parser.parseCSV("input.txt", "output.txt", array, airportGridUnits, activeGridUnits);
            if (startingCity == "New York")
            {
                activeGridUnits.Add(array[511, 153]);
            }
            else if (startingCity == "Los Angeles")
            {
                activeGridUnits.Add(array[67, 85]);
            }
            else if (startingCity == "Chicago")
            {
                activeGridUnits.Add(array[373, 164]);
            }

            activeGridUnits.ElementAt(0).i = .05;
        }

        public void runTimeStep()//DataGrid dataGrid)
        {
            for (int i = 0; i < activeGridUnits.Count; i++)
            {
                doIteration(activeGridUnits.ElementAt(i), deltaT, this.k, this.deathRate, this.b);
                //overWriteGrid(i, temp);
            }
            int current = activeGridUnits.Count;
            //for each grid, call calculateSpreadToNeighbors() on it
            for (int i = 0; i < current; i++)
            {
                calculateSpreadToNeighbors(activeGridUnits.ElementAt(i), this.borderTravelRate, this.airportTravelRate);
            }
        }

        public double[] getArray()
        {
            double[] longArray = new double[array.Length];

            for (int y = 0; y < gridYLength; y++)
            {
                    for (int x = 0; x < gridXLength; x++)
                {
                    int i = y * gridXLength + x;
                    longArray[i] = array[x, y].i;
                }
                
            }
            return longArray;
        }

        static void Run(DataGrid dataGrid)
        {

            // parse the data and fill the array
            parser.parseCSV("input.txt", "output.txt", array, airportGridUnits, activeGridUnits);



            //array[39, 249].i = Convert.ToDouble(1000.0 / array[39, 249].N);
            //activeGridUnits.Add(array[39, 249]);




            // figure out the beginning GridUnit and add it to activeGridUnits


            while (sumT < dataGrid.totalT)
            {
                //Console.WriteLine("-----------------------");
                //for (int i = 0; i < gridXLength; i++)
                //{
                //    for (int j = 0; j < gridYLength; j++)
                //    {
                //        if (array[i,j].I > 0)
                //        {
                //            Console.Write("X");
                //        }
                //        else
                //        {
                //            Console.Write(" ");
                //        }
                //    }
                //    Console.Write("\n");                    
                //}
                //Console.WriteLine("-----------------------");
                //for each grid, call doIteration if I is not 0;

                for (int i = 0; i < activeGridUnits.Count; i++)
                {
                    doIteration(activeGridUnits.ElementAt(i), deltaT, dataGrid.k, dataGrid.deathRate, dataGrid.b);
                    //overWriteGrid(i, temp);
                }
                int current = activeGridUnits.Count;
                //for each grid, call calculateSpreadToNeighbors() on it
                for (int i = 0; i < current; i++)
                {
                    calculateSpreadToNeighbors(activeGridUnits.ElementAt(i), dataGrid.borderTravelRate, dataGrid.airportTravelRate);
                }


                sumT += deltaT;
            }
            
        }

        public static void doIteration(GridUnit grid, double deltaT, float k, float deathRate , float b)
        {

            /*
            S = S(t)	is the number of susceptible individuals,
            I = I(t)	is the number of infected individuals, and
            R = R(t)	is the number of recovered individuals.
            s(t) = S(t)/N,	the susceptible fraction of the population,
            i(t) = I(t)/N,	the infected fraction of the population, and
            r(t) = R(t)/N,	the recovered fraction of the population.
            S' = -b s(t)I(t)
            s' = -b s(t)i(t)
            r' = k i(t)
            s' + i' + r' = 0
            i' = b s(t) i(t) - k i(t)
            */


            // calculate the rate of change for this iteration
            double sPrime = -(b) * grid.s * grid.i;
            double iPrime = b * grid.s * grid.i - (k * grid.i);
            double rPrime = k * grid.i;

            // apply the change to the population 
            grid.s += (sPrime * deltaT);
            grid.i += (iPrime * deltaT);
            totalInfected += Convert.ToInt64(iPrime * grid.N);
            grid.r += (rPrime * deltaT);
            totalRecovered += Convert.ToInt64(rPrime * grid.N);
            
            // calculate losses
            long theDead = Convert.ToInt64(rPrime * grid.N * deltaT * deathRate);
            grid.D += theDead;
            totalDead += theDead;
            grid.N -= theDead; // remove losses from total population, needs testing.]

            if (grid.I <= 0 || grid.N <= 0 || grid.R == grid.N)
            {
                activeGridUnits.Remove(grid);
            }
        }

        public static void calculateSpreadToNeighbors(GridUnit centerGrid, float borderTravelRate, float airportTravelRate) // spreads infection out, one way, from each infected grid
        {


            long infectedOut = Convert.ToInt64(centerGrid.i * borderTravelRate * centerGrid.N);
            long recoveredOut = Convert.ToInt64(centerGrid.r * borderTravelRate * centerGrid.N);
            long susceptableOut = Convert.ToInt64(centerGrid.s * borderTravelRate * centerGrid.N);

            // readjust populations due to people leaving

            // calculate each direction
            if (!(centerGrid.yCoord <= 0)) // go down
            {
                GridUnit target = array[centerGrid.xCoord, centerGrid.yCoord - 1];
                if (target.N > 0)
                {


                    target.N += infectedOut + recoveredOut + susceptableOut;
                    target.i += Convert.ToSingle(infectedOut) / target.N;
                    target.r += Convert.ToSingle(recoveredOut) / target.N;
                    target.s += Convert.ToSingle(susceptableOut) / target.N;

                    centerGrid.N -= infectedOut + recoveredOut + susceptableOut;
                    centerGrid.i -= Convert.ToSingle(infectedOut) / centerGrid.N;
                    centerGrid.r -= Convert.ToSingle(recoveredOut) / centerGrid.N;
                    centerGrid.s -= Convert.ToSingle(susceptableOut) / centerGrid.N;

                    if (target.i > 0 && (!activeGridUnits.Contains(target)))
                    {
                        activeGridUnits.Add(target);
                    }
                }
            }
            if (!(centerGrid.yCoord >= gridYLength - 1)) // go up
            {
                GridUnit target = array[centerGrid.xCoord, centerGrid.yCoord + 1];
                if (target.N > 0)
                {

                    target.N += infectedOut + recoveredOut + susceptableOut;
                    target.i += Convert.ToSingle(infectedOut) / target.N;
                    target.r += Convert.ToSingle(recoveredOut) / target.N;
                    target.s += Convert.ToSingle(susceptableOut) / target.N;

                    centerGrid.N -= infectedOut + recoveredOut + susceptableOut;
                    centerGrid.i -= Convert.ToSingle(infectedOut) / centerGrid.N;
                    centerGrid.r -= Convert.ToSingle(recoveredOut) / centerGrid.N;
                    centerGrid.s -= Convert.ToSingle(susceptableOut) / centerGrid.N;

                    if (target.i > 0 && (!activeGridUnits.Contains(target)))
                    {
                        activeGridUnits.Add(target);
                    }
                }
            }
            if (!(centerGrid.xCoord <= 0)) // go left
            {
                GridUnit target = array[centerGrid.xCoord - 1, centerGrid.yCoord];
                if (target.N > 0)
                {

                    target.N += infectedOut + recoveredOut + susceptableOut;
                    target.i += Convert.ToSingle(infectedOut) / target.N;
                    target.r += Convert.ToSingle(recoveredOut) / target.N;
                    target.s += Convert.ToSingle(susceptableOut) / target.N;

                    centerGrid.N -= infectedOut + recoveredOut + susceptableOut;
                    centerGrid.i -= Convert.ToSingle(infectedOut) / centerGrid.N;
                    centerGrid.r -= Convert.ToSingle(recoveredOut) / centerGrid.N;
                    centerGrid.s -= Convert.ToSingle(susceptableOut) / centerGrid.N;

                    if (target.i > 0 && (!activeGridUnits.Contains(target)))
                    {
                        activeGridUnits.Add(target);
                    }
                }
            }
            if (!(centerGrid.xCoord >= gridXLength - 1)) // go right
            {
                GridUnit target = array[centerGrid.xCoord + 1, centerGrid.yCoord];
                if (target.N > 0)
                {

                    target.N += infectedOut + recoveredOut + susceptableOut;
                    target.i += Convert.ToSingle(infectedOut) / target.N;
                    target.r += Convert.ToSingle(recoveredOut) / target.N;
                    target.s += Convert.ToSingle(susceptableOut) / target.N;

                    centerGrid.N -= infectedOut + recoveredOut + susceptableOut;
                    centerGrid.i -= Convert.ToSingle(infectedOut) / centerGrid.N;
                    centerGrid.r -= Convert.ToSingle(recoveredOut) / centerGrid.N;
                    centerGrid.s -= Convert.ToSingle(susceptableOut) / centerGrid.N;

                    if (target.i > 0 && (!activeGridUnits.Contains(target)))
                    {
                        activeGridUnits.Add(target);
                    }
                }
            }
            if (airportGridUnits.Contains(centerGrid))
            {
                foreach (GridUnit otherAirport in airportGridUnits)
                {
                    if (otherAirport != centerGrid)
                    {
                        otherAirport.N += Convert.ToInt64((infectedOut + recoveredOut + susceptableOut) * airportTravelRate);
                        otherAirport.i += Convert.ToSingle(infectedOut * airportTravelRate) / otherAirport.N;
                        otherAirport.r += Convert.ToSingle(recoveredOut * airportTravelRate) / otherAirport.N;
                        otherAirport.s += Convert.ToSingle(susceptableOut * airportTravelRate) / otherAirport.N;

                        if (otherAirport.i > 0 && (!activeGridUnits.Contains(otherAirport)))
                        {
                            activeGridUnits.Add(otherAirport);
                        }

                        centerGrid.N -= Convert.ToInt64((infectedOut + recoveredOut + susceptableOut) * airportTravelRate);
                        centerGrid.i -= Convert.ToSingle(infectedOut * airportTravelRate) / centerGrid.N;
                        centerGrid.r -= Convert.ToSingle(recoveredOut * airportTravelRate) / centerGrid.N;
                        centerGrid.s -= Convert.ToSingle(susceptableOut * airportTravelRate) / centerGrid.N;
                    }
                }
            }
        }
    }
}

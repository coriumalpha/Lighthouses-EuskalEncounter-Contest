using Entities;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Players.TestPlayerV2
{
    public class TestPlayerV2 : IPlayer
    {
        private const int MAX_CELL_ENERGY = 100;
        private const int MIN_ALTER_ROUTE_ENERGY_DIFFERENCE = 80;
        private const double MAX_ENERGY_CONSUMPTION_COEFFICIENT = 0.45;
        private const double ENERGY_INCREMENT = 0.15;

        private readonly double MAX_STEP_DISTANCE;

        #region Public properties
        public int Id { get; set; }
        public string Name { get; set; }
        public int PlayerCount { get; set; }
        public IMap Map { get; set; }
        public Vector2 Position { get; set; }
        public int Score { get; set; }
        public int Energy { get; set; }
        public int MaxEnergy { get; set; }
        public List<Vector2> Keys { get; set; }
        public List<Lighthouse> Lighthouses { get; set; }
        public List<Lighthouse> OwnLighthouses { get; set; }
        public Cell[] View { get; set; }
        #endregion

        private Random rand;
        private Vector2? destination = null;
        private Route route;
        private bool energyOptimization;

        public TestPlayerV2(string name = "", bool energyOptimization = true)
        {
            this.MAX_STEP_DISTANCE = Vector2.Distance(new Vector2(0, 0), new Vector2(1, 1));
            this.Name = name;
            this.rand = new Random();
            this.Keys = new List<Vector2>();
            this.Lighthouses = new List<Lighthouse>();
            this.energyOptimization = energyOptimization;
        }

        public void Setup(IPlayerConfig playerConfig)
        {
            this.Id = playerConfig.Id;
            this.PlayerCount = playerConfig.PlayerCount;
            this.Position = playerConfig.Position;
            this.Map = playerConfig.Map;
            this.Lighthouses = playerConfig.Lighthouses.Select(x => new Lighthouse() { Position = x }).ToList();
        }

        public IDecision Play(ITurnState state)
        {
            UpdatePlayerState(state);

            IDecision decision = new Decision();

            if (AttackLighthouse(decision))
            {
                return decision;
            }

            if (destination == null || state.Position == destination)
            {
                this.route = BestRoute(state.Position, out destination);
            }

            decision.Action = PlayerActions.Move;

            Vector2 targetStep = NextStep(state.Position, route, this.energyOptimization);

            decision.Target = targetStep;
                       
            return decision;
        }

        private bool AttackLighthouse(IDecision decision)
        {
            IEnumerable<ILighthouse> actualPositionLighthouses = Lighthouses.Where(x => x.Position == Position);
            if (actualPositionLighthouses.Any())
            {
                ILighthouse actualPositionLighthouse = actualPositionLighthouses.Single();
                if (actualPositionLighthouse.IdOwner != this.Id)
                {
                    double maxAllowedEnergy = (this.Energy * MAX_ENERGY_CONSUMPTION_COEFFICIENT);
                    if (actualPositionLighthouse.Energy > maxAllowedEnergy)
                    {
                        return false;
                    }

                    int loadedEnergy;
                    if (actualPositionLighthouse.Energy == 0)
                    {
                        double averageEnergy = Lighthouses.Average(x => x.Energy);
                        if (averageEnergy == 0)
                        {
                            loadedEnergy = (int)Math.Truncate(maxAllowedEnergy);
                        }
                        else
                        {
                            
                            loadedEnergy = (int)Math.Truncate(((averageEnergy * ENERGY_INCREMENT) + averageEnergy));
                        }                        
                    }
                    else
                    { 
                        loadedEnergy = (actualPositionLighthouse.Energy * 2) + (int)Math.Truncate((actualPositionLighthouse.Energy * ENERGY_INCREMENT));
                    }

                    if (loadedEnergy > maxAllowedEnergy)
                    {
                        return false;
                    }

                    decision.Action = PlayerActions.Attack;
                    decision.Energy = loadedEnergy;

                    return true;
                }
            }

            return false;
        }

        private void UpdatePlayerState(ITurnState state)
        {
            this.Position = state.Position;
            this.Lighthouses = state.Lighthouses.Select(x => new Lighthouse() { Position = x.Position, Energy = x.Energy, IdOwner = x.IdOwner }).ToList();
            this.Energy = state.Energy;
        }

        private Route BestRoute(Vector2 origin, out Vector2? destination)
        {
            List<Route> posibleRoutes = new List<Route>();

            foreach (Lighthouse lighthouse in Lighthouses.Where(x => x.Position != origin && x.IdOwner != this.Id))
            {
                posibleRoutes.Add(TraceRoute(origin, lighthouse.Position));
            }

            if (!posibleRoutes.Any())
            {
                posibleRoutes.Add(TraceRoute(origin, Lighthouses.First().Position));
            }

            Route bestRoute = posibleRoutes.OrderByDescending(x => x.Way.Sum(w => w.Energy)).First();
            destination = bestRoute.Destination.Position;

            return bestRoute;
        }

        private Route TraceRoute(Vector2 originVector, Vector2 destinationVector)
        {
            List<Step> steps = new List<Step>();
            List<ICell> visitedCells = new List<ICell>();

            ICell destination = FromVector2(destinationVector);
            ICell origin = FromVector2(originVector);

            Step firstStep = new Step(0);
            firstStep.Discoveries.Add(new Discovery() { Cell = origin });
            steps.Add(firstStep);

            int counter = 1;
            while (true)
            {
                //Es imprescindible acceder al elemento antes de agregar uno nuevo
                Step lastStep = steps.OrderByDescending(x => x.StepNumber).First();
                Step actualStep = new Step(counter);
                steps.Add(actualStep);


                foreach (Discovery discovery in lastStep.Discoveries)
                { 
                    IEnumerable<ICell> discoveredCells = DiscoverCloseCells(discovery.Cell, visitedCells);

                    foreach (ICell cellToVisit in discoveredCells)
                    {
                        actualStep.Discoveries.Add(new Discovery() { Cell = cellToVisit, Discoverer = discovery.Cell });
                        visitedCells.Add(cellToVisit);

                        if (cellToVisit == destination)
                        {
                            goto floodFinished;
                        }
                    }
                }               

                counter++;
            }

        floodFinished:
            Route route = new Route(origin, destination);

            ICell searchCell = destination;
            route.Way.Add(searchCell);

            foreach (Step step in steps.OrderByDescending(x => x.StepNumber))
            {
                Discovery discovery = step.Discoveries.Where(x => x.Cell == searchCell).Single();
                if (discovery.Discoverer == null)
                {
                    route.Way.Reverse();
                    return route;
                }

                searchCell = discovery.Discoverer;
                route.Way.Add(searchCell);
            }

            route.Way.Reverse();
            return route;
        }

        private IEnumerable<ICell> DiscoverCloseCells(ICell cell, List<ICell> visited)
        {
            IEnumerable<ICell> closeCells = this.Map.Grid.Where(x => Vector2.Distance(cell.Position, x.Position) <= MAX_STEP_DISTANCE && x.IsPlayable && !visited.Contains(x));
            return closeCells;
        }

        private ICell FromVector2(Vector2 vector)
        {
            return this.Map.Grid.Where(x => x.Position.Equals(vector)).Single();
        }

        private Vector2 NextStep(Vector2 origin, Route route, bool energyPonderated = false)
        {
            ICell nextWaypoint = route.RemainingWay.First();
            
            if (energyPonderated)
            {
                nextWaypoint = EnergyPonderatedDestination(origin, nextWaypoint, route);
            }

            Vector2 step = Step(origin, nextWaypoint.Position);

            if (!GameLogic.IsValidStep(origin, step, this.Map.Grid))
            {
                throw new Exception("Invalid movement request");
            }

            RegisterStep(nextWaypoint);

            return step;
        }

        private ICell EnergyPonderatedDestination(Vector2 origin, ICell defaultNextWaypoing, Route route)
        {
            IEnumerable<ICell> closeCells = this.Map.Grid.Where(x => Vector2.Distance(origin, x.Position) <= MAX_STEP_DISTANCE && x.IsPlayable);
            ICell maxEnergy = closeCells.OrderByDescending(x => x.Energy).First();

            if (defaultNextWaypoing.Position == maxEnergy.Position)
            {
                return defaultNextWaypoing;
            }

            double energyDifference = (maxEnergy.Energy - defaultNextWaypoing.Energy);
            if (energyDifference > MIN_ALTER_ROUTE_ENERGY_DIFFERENCE)
            {
                this.route = TraceRoute(maxEnergy.Position, route.Destination.Position);

                return maxEnergy;
            }

            return defaultNextWaypoing;
        }

        private void RegisterStep(ICell stepped)
        {
            this.route.TraveledWay.Add(stepped);
        }

        private Vector2 Step(Vector2 origin, Vector2 destination)
        {
            Vector2 trayectory = Vector2.Subtract(destination, origin);

            Vector2 step = new Vector2();
            step.X = (float)Math.Truncate(Math.Clamp(trayectory.X, -1, 1));
            step.Y = (float)Math.Truncate(Math.Clamp(trayectory.Y, -1, 1));

            return step;
        }
    }
}

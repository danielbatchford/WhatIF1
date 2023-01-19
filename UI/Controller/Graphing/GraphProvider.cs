using System;
using System.Collections.Generic;
using System.Linq;
using WhatIfF1.Modelling.Events.Drivers;
using WhatIfF1.Util;

namespace WhatIfF1.UI.Controller.Graphing
{
    public class GraphProvider : NotifyPropertyChangedWrapper
    {
        // TODO - could remove this class completely

        private readonly EventController _parentController;

        public IEnumerable<GraphType> GraphTypes { get; }

        private XYGraph _graph;

        public XYGraph Graph
        {
            get => _graph;
            set
            {
                _graph = value;
                OnPropertyChanged();
            }
        }

        public GraphProvider(EventController parentController, GraphType graphType)
        {
            _parentController = parentController;

            GraphTypes = Enum.GetValues(typeof(GraphType)).Cast<GraphType>();

            Graph = GetGraphFromType(parentController, graphType);
        }

        public void UpdateGraph()
        {
            int leaderLap = _parentController.CurrentLap;

            if (leaderLap < 1)
            {
                throw new GraphException("Cannot set lap to lap < 1");
            }
            if (leaderLap > _parentController.Model.NoOfLaps)
            {
                throw new GraphException($"Cannot set lap as it exceeded the number of laps in the parent model (Got {leaderLap} laps, model has {_parentController.Model.NoOfLaps} laps)");
            }

            Graph.UpdateGraph();
        }

        public void UpdateCurrentDriver(Driver driver)
        {
            Graph.TargetDriver = driver;
        }

        public void RemoveCurrentDriver()
        {
            Graph.Clear();
        }

        private XYGraph GetGraphFromType(EventController parentController, GraphType graphType)
        {
            switch (graphType)
            {
                case GraphType.VELOCITY_TIME:
                    return new VelocityLapGraph(parentController);
                default:
                    throw new GraphException($"Could not find a graph implementation for the provided {nameof(graphType)} {graphType}");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;
using WhatIfF1.UI.Controller.Graphing.Interfaces;
using WhatIfF1.UI.Controller.Interfaces;
using WhatIfF1.Util;

namespace WhatIfF1.UI.Controller.Graphing
{
    public class GraphProvider : NotifyPropertyChangedWrapper, IGraphProvider
    {
        private readonly IEventController _parentController;

        public IEnumerable<GraphType> GraphTypes { get; }

        private IXYGraph _graph;

        public IXYGraph Graph
        {
            get => _graph;
            set
            {
                _graph = value;
                OnPropertyChanged();
            }
        }

        private IDriver _targetDriver;

        public IDriver TargetDriver
        {
            get => _targetDriver;
            set
            {
                if (_targetDriver?.Equals(value) == true)
                {
                    return;
                }

                _targetDriver = value;

                if (value is null)
                {
                    Graph.Clear();
                }
                else
                {
                    Graph.TargetDriver = value;
                }

                OnPropertyChanged();
            }
        }

        // TODO - could migrate graph logic to this class

        public GraphProvider(IEventController parentController, GraphType graphType)
        {
            _parentController = parentController;

            GraphTypes = Enum.GetValues(typeof(GraphType)).Cast<GraphType>();

            Graph = GetGraphFromType(parentController, graphType);
        }

        public void UpdateProvider()
        {
            if (TargetDriver == null)
            {
                return;
            }

            Graph.UpdateGraph();
        }

        private IXYGraph GetGraphFromType(IEventController parentController, GraphType graphType)
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

using System;
using System.Collections.Generic;
using System.Linq;
using ReactGraph.Internals.Notification;

namespace ReactGraph.Internals.NodeInfo
{
    class NodeRepository
    {
        private readonly Dictionary<Tuple<object, string>, INodeInfo> nodeLookup;
        private readonly List<INotificationStrategy> notificationStrategies;

        public NodeRepository(DependencyEngine dependencyEngine)
        {
            nodeLookup = new Dictionary<Tuple<object, string>, INodeInfo>();
            notificationStrategies = new List<INotificationStrategy>
            {
                new NotifyPropertyChangedStrategy(dependencyEngine)
            };
        }

        public INotificationStrategy[] GetStrategies(Type type)
        {
            return notificationStrategies
                .Where(notificationStrategy => notificationStrategy.AppliesTo(type))
                .ToArray();
        }

        public bool Contains(object instance, string key)
        {
            return nodeLookup.ContainsKey(Tuple.Create(instance, key));
        }

        public INodeInfo Get(object instance, string key)
        {
            return nodeLookup[Tuple.Create(instance, key)];
        }

        public void RemoveLookup(object instance, string key)
        {
            var tuple = Tuple.Create(instance, key);
            if (nodeLookup.ContainsKey(tuple))
                nodeLookup.Remove(tuple);
        }

        public void AddLookup(object instance, string key, INodeInfo nodeInfo)
        {
            var tuple = Tuple.Create(instance, key);
            if (!nodeLookup.ContainsKey(tuple))
                nodeLookup.Add(tuple, nodeInfo);
        }
    }
}
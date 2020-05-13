using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelEngine.Utilities
{
    internal class MachineState<T>
    {
        public string Name;
        public object Value;

        private List<Connection<T>> connections;

        public MachineState(string name, object value, params Connection<T>[] connections)
        {
            Name = name;
            Value = value;
            this.connections = connections.ToList();
        }

        public object FindNextState()
        {
            foreach (Connection<T> connection in connections)
            {
                if (connection.AllConditionsTrue())
                {
                    if (connection.HasExitTime)
                        return connection.ConnectedTo;
                    else if (!connection.HasExitTime)
                        return connection.ConnectedTo;
                }
            }

            return null;
        }
    }
}

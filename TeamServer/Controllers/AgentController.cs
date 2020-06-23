using AgentCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamServer.Models;

namespace TeamServer.Controllers
{
    public class AgentController
    {
        public List<AgentSessionData> ConnectedAgents { get; set; } = new List<AgentSessionData>();
        public void UpdateSession(Metadata metadata)
        {
            if (!ConnectedAgents.Where(a => a.Metadata.AgentId.Equals(metadata.AgentId, StringComparison.OrdinalIgnoreCase)).Any())
            {
                CreateSession(metadata);
            }
            else
            {
                ConnectedAgents.Where(a => a.Metadata.AgentId.Equals(metadata.AgentId, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault().LastSeen = DateTime.UtcNow;
            }
        }

        private void CreateSession(Metadata metadata)
        {
            ConnectedAgents.Add(new AgentSessionData
            {
                Metadata = metadata,
                FirstSeen = DateTime.UtcNow,
                LastSeen = DateTime.UtcNow
            });
        }

        public IEnumerable<AgentSessionData> GetSessions()
        {
            return ConnectedAgents;
        }

    }
}

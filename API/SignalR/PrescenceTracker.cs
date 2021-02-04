using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class PrescenceTracker
    {
        private static readonly Dictionary<string, List<string>> OnlineUsers = 
            new Dictionary<string, List<string>>();

        public Task UserConnected(string useremail, string connectionId)
        {
            lock (OnlineUsers)
            {
                if(OnlineUsers.ContainsKey(useremail))
                {
                    OnlineUsers[useremail].Add(connectionId);
                }
                else 
                {
                    OnlineUsers.Add(useremail,new List<string>{connectionId});
                }
            }
            return Task.CompletedTask;
        }
        public Task UserDisonnected(string useremail, string connectionId)
        {
            lock (OnlineUsers)
            {
                if(!OnlineUsers.ContainsKey(useremail))
                {
                    return Task.CompletedTask;

                    

                }
                OnlineUsers[useremail].Remove(connectionId);
                if(OnlineUsers[useremail].Count == 0)
                {
                    OnlineUsers.Remove(useremail);
                }
            }
            return Task.CompletedTask;
            
        }

        public Task<string[]> GetOnlineUsers()
        {
            string [] onlineUsers;
            lock(OnlineUsers)
            {
                onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }

            return Task.FromResult(onlineUsers);
        }

        public Task<List<string>> GetConnectionsForUser(string useremail)
        {
            List<string> connectionIds;
            lock(OnlineUsers)
            {
                connectionIds = OnlineUsers.GetValueOrDefault(useremail);
            }

            return Task.FromResult(connectionIds);
        }

    
    }
}
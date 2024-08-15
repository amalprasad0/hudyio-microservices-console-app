using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace ChatService.Utilities
{
    public class UserConnectionManager
    {
        private readonly ILogger<UserConnectionManager> _logger;
        private static readonly ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();

        public UserConnectionManager(ILogger<UserConnectionManager> logger)
        {
            _logger = logger;
        }

        public void AddConnection(string userId, string connectionId)
        {
            _connections[userId] = connectionId;
            _logger.LogInformation("User connected: {UserId} with ConnectionId: {ConnectionId}", userId, connectionId);
        }

        public void RemoveConnection(string userId)
        {
            _connections.TryRemove(userId, out _);
            _logger.LogInformation("User disconnected: {UserId}", userId);
        }

        public string GetConnectionId(string userId)
        {
            _connections.TryGetValue(userId, out var connectionId);
            return connectionId;
        }
    }
}

using Network;

namespace Oxide.Plugins
{
    [Info("AlreadyFix", "Mevent", "0.0.1")]
    public class AlreadyFix : RustPlugin
    {
        void OnClientAuth(Connection connection)
        {
            for (var i = BasePlayer.activePlayerList.Count - 1; i >= 0; i--)
            {
                var player = BasePlayer.activePlayerList[i];
                if (player.userID == connection.userid)
                {
                    BasePlayer.activePlayerList.RemoveAt(i);
                    player?.Kick("");
                }
            }
            for (var i = ConnectionAuth.m_AuthConnection.Count - 1; i >= 0; i--)
            {
                var player = ConnectionAuth.m_AuthConnection[i];
                if (player.userid == connection.userid)
                    ConnectionAuth.m_AuthConnection.RemoveAt(i);
            }
        }
    }
}
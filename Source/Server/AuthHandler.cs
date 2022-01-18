using System;
using System.Net;

namespace CommService
{
    /// <summary>
    /// Default auth handler. Accepts user "anonymous", password empty.
    /// Allows all connections.
    /// </summary>
    public class AuthHandler : IAuthHandler
    {
        private IPEndPoint peer;
        private bool allowAnyDataPeer;
        private string rootUser;
        private string rootPass;

        private AuthHandler(
            IPEndPoint peer, bool allowAnyDataPeer, string user, string pass)
        {
            this.peer = peer;
            this.allowAnyDataPeer = allowAnyDataPeer;
            this.rootUser = user;
            this.rootPass = pass;
        }

        public AuthHandler(bool allowAnyDataPeer, string user, string pass) :
            this(null, allowAnyDataPeer, user, pass)
        {
        }

        public AuthHandler() : this(false, "anonymous", string.Empty)
        {
        }

        public IAuthHandler Clone(IPEndPoint newPeer)
        {
            return new AuthHandler(newPeer, allowAnyDataPeer, rootUser, rootPass);
        }

        public bool AllowLogin(string user, string pass)
        {
            return (user == rootUser && pass == rootPass);
        }

        public bool AllowControlConnection()
        {
            return true;
        }

        public bool AllowActiveDataConnection(IPEndPoint port)
        {
            // allow any peer or only allow active connections to the same peer as the control connection
            return (allowAnyDataPeer || peer.Address.Equals(port.Address));
        }
    }
}

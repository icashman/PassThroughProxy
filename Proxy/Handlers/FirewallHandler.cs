﻿using System.Linq;
using System.Threading.Tasks;
using Proxy.Configurations;
using Proxy.Sessions;

namespace Proxy.Handlers
{
    public class FirewallHandler : IHandler
    {
        private static readonly FirewallHandler Self = new FirewallHandler();

        private FirewallHandler()
        {
        }

        public Task<HandlerResult> Run(SessionContext context)
        {
            if (!Configuration.Get().Firewall.Enabled)
            {
                return Task.FromResult(HandlerResult.NewHostConnectionRequired);
            }

            return IsAllowed(context)
                ? Task.FromResult(HandlerResult.NewHostConnectionRequired)
                : Task.FromResult(HandlerResult.Terminated);
        }

        public static FirewallHandler Instance()
        {
            return Self;
        }

        private static bool IsAllowed(SessionContext context)
        {
            return !Configuration.Get().Firewall.Rules
                .Any(r => r.Pattern.Match(context.Header.Host.Hostname).Success &&
                          r.Action == ActionEnum.Deny);
        }
    }
}
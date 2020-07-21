﻿using Microsoft.Extensions.Hosting;
using OpenMod.API;
using OpenMod.Core.Eventing;

namespace OpenMod.Core.Ioc
{
    public sealed class OpenModInitializedEvent : Event
    {
        public IOpenModHost Host { get; }

        public OpenModInitializedEvent(IOpenModHost host)
        {
            Host = host;
        }
    }
}
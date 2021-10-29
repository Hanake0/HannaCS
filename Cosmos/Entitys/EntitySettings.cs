using System;
using System.Collections.Generic;

namespace Hanna.Cosmos.Entitys {
    public class EntitySettings {
        public ulong Id { get; init; }
        public Dictionary<string, Object> Configs { get; init; }
    }
}
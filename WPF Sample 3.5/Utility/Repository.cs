using SampleWPF.Core.Interfaces;
using SampleWPF.Models;
using SampleWPF.Utility.Interfaces;

namespace SampleWPF.Utility
{
    public static class Repository
    {
        public static IClientCache ClientCache { get; set; }
        public static IGlobalCache GlobalCache { get; set; }
        public static SessionData Session { get; set; }
    }
}

using System.Collections.Generic;

namespace SampleWPF.Utility.Interfaces
{
    public interface ICacheAdmin
    {
        List<string> GetKeys();
        object GetItem(string key);
    }
}

using Plus.AutoApi.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace Plus.AutoApi.Sample.Service
{
    [AutoApi(Disabled = true)]
    public class HelloApplicationService : IAutoApi
    {
        public IEnumerable<int> Get()
        {
            return Enumerable.Range(0, 10);
        }
    }
}
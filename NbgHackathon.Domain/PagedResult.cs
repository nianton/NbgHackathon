using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbgHackathon.Domain
{
    public class PagedResult<T>
    {
        public IList<T> Items { get; set; }
        public string ContinuationToken { get; set; }
        public bool HasMoreResults => !string.IsNullOrEmpty(ContinuationToken);
    }
}

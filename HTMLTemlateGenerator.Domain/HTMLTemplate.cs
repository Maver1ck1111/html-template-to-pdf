using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLTemlateGenerator.Domain
{
    public class HTMLTemplate
    {
        public Guid Id { get; set; } 
        public string Name { get; set; } = null!;
        public string HTMLContent { get; set; } = null!;
    }
}

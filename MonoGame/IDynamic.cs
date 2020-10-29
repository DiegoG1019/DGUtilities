using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.MonoGame
{
    public interface IDynamic
    {
        ID ID { get; set; }
        void Destroy();
        event Action<object> IDChanged;
    }
}

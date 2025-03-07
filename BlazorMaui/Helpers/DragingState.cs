using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMaui.Helpers
{
    static public class DragingState
    {
        static public bool IsDraging { get; set; }

        static public void SetIsDraging() => IsDraging = !IsDraging;
        static public bool GetIsDraging() => IsDraging;
    }
}

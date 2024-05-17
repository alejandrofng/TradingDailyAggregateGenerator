using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxpoAsignacion.Services.TimeZoneService
{
    public interface ITimeZoneService
    {
        public TimeSpan getOffSet(DateTime date);
    }
}

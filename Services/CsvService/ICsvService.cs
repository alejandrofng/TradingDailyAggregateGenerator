using Axpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxpoAsignacion.Services.CsvService
{
    public interface ICsvService
    {
        public void SaveCsv(List<PowerPeriod> data, DateTime date, string filePath);
    }
}

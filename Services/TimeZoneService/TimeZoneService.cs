using Serilog;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace AxpoAsignacion.Services.TimeZoneService
{
    public class TimeZoneService: ITimeZoneService
    {
        public TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");
        public TimeZoneService(string? TimeZoneId)
        {
            TimeZoneId = TimeZoneId ?? "Europe/Berlin";
            try 
            { 
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            }
            catch(Exception ex) 
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public TimeSpan getOffSet(DateTime date)
        {
            TimeSpan variable = TimeZone.GetUtcOffset(date);
            return variable;
        }
    }
}

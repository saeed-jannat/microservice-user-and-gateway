using identity.Core.Application.Interfaces;
using System.Globalization;

namespace identity.Core.Application.Services
{
    public class DateService : IDateService
    {

        public string ConvertToShamsi(DateTime? date)
        {
            if (date != null)
            {
                var persianCalendar = new PersianCalendar();
                return $"{persianCalendar.GetYear((DateTime)date):0000}/{persianCalendar.GetMonth((DateTime)date):00}/{persianCalendar.GetDayOfMonth((DateTime)date):00}";
            }
            else return null;
        }
    }
}

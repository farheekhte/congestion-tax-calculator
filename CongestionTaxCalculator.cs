using System;
using System.Collections.Generic;
using System.Linq;

namespace congestion.calculator
{
    public class CongestionTaxCalculator
    {
        private readonly List<DateTime> _tollFreeDates = new List<DateTime>
        {
            // تاریخ‌های معاف از مالیات ترافیکی را اضافه کنید
            new DateTime(2013, 1, 1),
            new DateTime(2013, 3, 28),
            new DateTime(2013, 3, 29),
            // ... Add more dates here ...
        };

        private readonly Dictionary<TimeSpan, int> _tollFees = new Dictionary<TimeSpan, int>
        {
            // محاسبه قیمت‌های مالیات ترافیکی بر اساس بازه زمانی
            { new TimeSpan(6, 0, 0), 8 },
            { new TimeSpan(6, 30, 0), 13 },
            { new TimeSpan(7, 0, 0), 18 },
            { new TimeSpan(8, 0, 0), 13 },
            { new TimeSpan(15, 0, 0), 13 },
            { new TimeSpan(15, 30, 0), 18 },
            { new TimeSpan(17, 0, 0), 13 },
            { new TimeSpan(18, 0, 0), 8 },
            { TimeSpan.FromDays(1), 0 } // برای اوقاتی که تعریف نشده‌اند
        };

        private readonly Dictionary<string, Dictionary<TimeSpan, int>> _cityTollFees = new Dictionary<string, Dictionary<TimeSpan, int>>
        {
            {
                "Gothenburg", new Dictionary<TimeSpan, int>
                {
                    // محاسبه قیمت‌های مالیات ترافیکی بر اساس بازه زمانی برای Gothenburg
                    { new TimeSpan(6, 0, 0), 8 },
                    { new TimeSpan(6, 30, 0), 13 },
                    { new TimeSpan(7, 0, 0), 18 },
                    { new TimeSpan(8, 0, 0), 13 },
                    { new TimeSpan(15, 0, 0), 13 },
                    { new TimeSpan(15, 30, 0), 18 },
                    { new TimeSpan(17, 0, 0), 13 },
                    { new TimeSpan(18, 0, 0), 8 },
                    { TimeSpan.FromDays(1), 0 } // برای اوقاتی که تعریف نشده‌اند
                }
            },
            {
                "Stockholm", new Dictionary<TimeSpan, int>
                {
                    // محاسبه قیمت‌های مالیات ترافیکی بر اساس بازه زمانی برای Stockholm
                    { new TimeSpan(6, 0, 0), 10 },
                    { new TimeSpan(6, 30, 0), 15 },
                    { new TimeSpan(7, 0, 0), 20 },
                    { new TimeSpan(8, 0, 0), 15 },
                    { new TimeSpan(15, 0, 0), 15 },
                    { new TimeSpan(15, 30, 0), 20 },
                    { new TimeSpan(17, 0, 0), 15 },
                    { new TimeSpan(18, 0, 0), 10 },
                    { TimeSpan.FromDays(1), 0 } // برای اوقاتی که تعریف نشده‌اند
                }
            },
            // اضافه کردن شهرهای دیگر و قوانین مالیات ترافیکی آن‌ها اینجا...
        };

        public int CalculateTotalTax(Vehicle vehicle, List<DateTime> dates, string city)
        {
            // بررسی اگر خودرو معاف از مالیات ترافیکی باشد یا شهر نامعتبر باشد
            if (IsTollFreeVehicle(vehicle) || !IsValidCity(city))
                return 0;

            int totalTax = 0;
            DateTime intervalStart = dates[0];
            var tollFees = GetTollFeesForCity(city);

            foreach (DateTime date in dates)
            {
                int nextTax = GetTollFee(date.TimeOfDay, tollFees);
                int tempTax = GetTollFee(intervalStart.TimeOfDay, tollFees);

                if (IsWithinSameHour(date, intervalStart))
                    totalTax = CalculateHourlyTax(totalTax, tempTax, nextTax);
                else
                    totalTax += nextTax;

                intervalStart = date;
            }

            // اطمینان حاصل شود که مجموع مالیات به حداکثر 60 SEK نرسد
            return Math.Min(totalTax, 60);
        }

        private bool IsWithinSameHour(DateTime date1, DateTime date2)
        {
            TimeSpan timeDifference = date1 - date2;
            return timeDifference.TotalMinutes <= 60;
        }

        private int CalculateHourlyTax(int totalTax, int tempTax, int nextTax)
        {
            if (totalTax > 0) totalTax -= tempTax;
            if (nextTax >= tempTax) tempTax = nextTax;
            return totalTax + tempTax;
        }

        private bool IsTollFreeVehicle(Vehicle vehicle)
        {
            if (vehicle == null) return false;
            string vehicleType = vehicle.GetVehicleType();
            return IsTollFreeVehicleType(vehicleType);
        }

        private bool IsTollFreeVehicleType(string vehicleType)
        {
            return Enum.GetNames(typeof(TollFreeVehicles)).Contains(vehicleType);
        }

        private bool IsValidCity(string city)
        {
            return _cityTollFees.ContainsKey(city);
        }

        private Dictionary<TimeSpan, int> GetTollFeesForCity(string city)
        {
            return _cityTollFees[city];
        }

        private int GetTollFee(TimeSpan timeOfDay, Dictionary<TimeSpan, int> tollFees)
        {
            if (IsTollFreeTime(timeOfDay)) return 0;

            var fee = tollFees.FirstOrDefault(f => timeOfDay <= f.Key);
            return fee.Value;
        }

        private bool IsTollFreeTime(TimeSpan timeOfDay)
        {
            return timeOfDay >= new TimeSpan(18, 30, 0) || timeOfDay < new TimeSpan(6, 0, 0);
        }

        private enum TollFreeVehicles
        {
            Motorcycle,
            Tractor,
            Emergency,
            Diplomat,
            Foreign,
            Military
        }
    }
}
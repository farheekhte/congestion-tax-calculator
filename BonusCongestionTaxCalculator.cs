using System;
using System.Collections.Generic;
using System.Linq;
using congestion.calculator;

public class BonusCongestionTaxCalculator
{
    /**
         * Calculate the total toll fee for one day
         *
         * @param vehicle - the vehicle
         * @param dates   - date and time of all passes on one day
         * @return - the total congestion tax for that day
         */

    // List of dates that are toll-free (e.g., public holidays, weekends)
    private readonly List<DateTime> _tollFreeDates = new List<DateTime>
    {
        // Add dates exempt from congestion tax
        new DateTime(2013, 1, 1),
        new DateTime(2013, 3, 28),
        new DateTime(2013, 3, 29),
        // ... Add more dates here ...
    };

    // Default toll fees based on time intervals
    private readonly Dictionary<TimeSpan, int> _defaultTollFees = new Dictionary<TimeSpan, int>
    {
        // Define toll fees based on time intervals
        { new TimeSpan(6, 0, 0), 8 },
        { new TimeSpan(6, 30, 0), 13 },
        { new TimeSpan(7, 0, 0), 18 },
        { new TimeSpan(8, 0, 0), 13 },
        { new TimeSpan(15, 0, 0), 13 },
        { new TimeSpan(15, 30, 0), 18 },
        { new TimeSpan(17, 0, 0), 13 },
        { new TimeSpan(18, 0, 0), 8 },
        { TimeSpan.FromDays(1), 0 } // For unspecified times
    };

    // Toll fees for different cities
    private readonly Dictionary<string, Dictionary<TimeSpan, int>> _cityTollFees = new Dictionary<string, Dictionary<TimeSpan, int>>
    {
        {
            "Gothenburg", new Dictionary<TimeSpan, int>
            {
                // Define toll fees for Gothenburg based on time intervals
                { new TimeSpan(6, 0, 0), 8 },
                { new TimeSpan(6, 30, 0), 13 },
                { new TimeSpan(7, 0, 0), 18 },
                { new TimeSpan(8, 0, 0), 13 },
                { new TimeSpan(15, 0, 0), 13 },
                { new TimeSpan(15, 30, 0), 18 },
                { new TimeSpan(17, 0, 0), 13 },
                { new TimeSpan(18, 0, 0), 8 },
                { TimeSpan.FromDays(1), 0 } // For unspecified times
            }
        },
        {
            "Stockholm", new Dictionary<TimeSpan, int>
            {
                // Define toll fees for Stockholm based on time intervals
                { new TimeSpan(6, 0, 0), 10 },
                { new TimeSpan(6, 30, 0), 15 },
                { new TimeSpan(7, 0, 0), 20 },
                { new TimeSpan(8, 0, 0), 15 },
                { new TimeSpan(15, 0, 0), 15 },
                { new TimeSpan(15, 30, 0), 20 },
                { new TimeSpan(17, 0, 0), 15 },
                { new TimeSpan(18, 0, 0), 10 },
                { TimeSpan.FromDays(1), 0 } // For unspecified times
            }
        },
        // Add more cities and their toll fees here...
    };

    // Method to calculate the total congestion tax for a vehicle for a list of dates in a specific city
    public int CalculateTotalTax(Vehicle vehicle, List<DateTime> dates, string city)
    {
        // Check if the vehicle is toll-free or if the city is invalid
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

        // Ensure the total tax does not exceed the maximum of 60 SEK
        return Math.Min(totalTax, 60);
    }

    // Check if two DateTime instances are within the same hour
    private bool IsWithinSameHour(DateTime date1, DateTime date2)
    {
        TimeSpan timeDifference = date1 - date2;
        return timeDifference.TotalMinutes <= 60;
    }

    // Calculate the total tax when within the same hour
    private int CalculateHourlyTax(int totalTax, int tempTax, int nextTax)
    {
        if (totalTax > 0) totalTax -= tempTax;
        if (nextTax >= tempTax) tempTax = nextTax;
        return totalTax + tempTax;
    }

    // Check if the vehicle is toll-free
    private bool IsTollFreeVehicle(Vehicle vehicle)
    {
        if (vehicle == null) return false;
        string vehicleType = vehicle.GetVehicleType();
        return IsTollFreeVehicleType(vehicleType);
    }

    // Check if a vehicle type string is toll-free
    private bool IsTollFreeVehicleType(string vehicleType)
    {
        return Enum.GetNames(typeof(TollFreeVehicles)).Contains(vehicleType);
    }

    // Check if the city is valid
    private bool IsValidCity(string city)
    {
        return _cityTollFees.ContainsKey(city);
    }

    // Get toll fees for a specific city
    private Dictionary<TimeSpan, int> GetTollFeesForCity(string city)
    {
        return _cityTollFees[city];
    }

    // Get the toll fee based on the time of day and toll fees dictionary
    private int GetTollFee(TimeSpan timeOfDay, Dictionary<TimeSpan, int> tollFees)
    {
        if (IsTollFreeTime(timeOfDay)) return 0;

        var fee = tollFees.FirstOrDefault(f => timeOfDay <= f.Key);
        return fee.Value;
    }

    // Check if the time of day is toll-free
    private bool IsTollFreeTime(TimeSpan timeOfDay)
    {
        return timeOfDay >= new TimeSpan(18, 30, 0) || timeOfDay < new TimeSpan(6, 0, 0);
    }

    // Enum for toll-free vehicle types
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
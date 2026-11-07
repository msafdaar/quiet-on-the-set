using System;

namespace QuietOnTheSetUI
{
    public class HourlySchedule
    {
        public const int HoursPerDay = 24;
        public const int UseMasterLimit = -1;

        public bool Enabled { get; set; }

        public int[] Limits { get; set; }

        public HourlySchedule()
        {
            Limits = CreateDefaultLimits();
        }

        public static int[] CreateDefaultLimits()
        {
            int[] limits = new int[HoursPerDay];
            for (int hour = 0; hour < limits.Length; hour++)
            {
                limits[hour] = UseMasterLimit;
            }
            return limits;
        }

        public int GetLimit(int hour)
        {
            if (Limits == null || hour < 0 || hour >= HoursPerDay)
            {
                return UseMasterLimit;
            }

            int limit = Limits[hour];
            if (limit < 0 || limit > 100)
            {
                return UseMasterLimit;
            }

            return limit;
        }

        public void Normalize()
        {
            int[] normalized = CreateDefaultLimits();
            if (Limits != null)
            {
                int count = Math.Min(HoursPerDay, Limits.Length);
                for (int hour = 0; hour < count; hour++)
                {
                    int limit = Limits[hour];
                    if (limit >= 0 && limit <= 100)
                    {
                        normalized[hour] = limit;
                    }
                }
            }
            Limits = normalized;
        }

        public HourlySchedule Clone()
        {
            var clone = new HourlySchedule
            {
                Enabled = Enabled,
                Limits = Limits == null ? CreateDefaultLimits() : (int[])Limits.Clone()
            };
            return clone;
        }
    }
}

using System;
using System.IO;
using System.Xml.Serialization;

namespace QuietOnTheSetUI
{
    public static class HourlyScheduleStore
    {
        private static string FolderPath
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "QuietOnTheSet");
            }
        }

        public static string FilePath
        {
            get { return Path.Combine(FolderPath, "schedule.xml"); }
        }

        public static HourlySchedule Load()
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    return new HourlySchedule();
                }

                var serializer = new XmlSerializer(typeof(HourlySchedule));
                using (var stream = File.OpenRead(FilePath))
                {
                    var schedule = serializer.Deserialize(stream) as HourlySchedule;
                    if (schedule == null)
                    {
                        return new HourlySchedule();
                    }

                    schedule.Normalize();
                    return schedule;
                }
            }
            catch
            {
                return new HourlySchedule();
            }
        }

        public static void Save(HourlySchedule schedule)
        {
            if (schedule == null)
            {
                return;
            }

            Directory.CreateDirectory(FolderPath);
            var serializer = new XmlSerializer(typeof(HourlySchedule));
            using (var stream = File.Create(FilePath))
            {
                serializer.Serialize(stream, schedule);
            }
        }
    }
}

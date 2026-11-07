using System;
using System.Globalization;
using System.Windows.Forms;

namespace QuietOnTheSetUI
{
    public partial class HourlyScheduleForm : Form
    {
        private readonly HourlySchedule _schedule;

        public HourlySchedule Schedule
        {
            get { return _schedule; }
        }

        public HourlyScheduleForm(HourlySchedule schedule)
        {
            InitializeComponent();
            _schedule = schedule == null ? new HourlySchedule() : schedule;
            this.Icon = QuietOnTheSetUI.Properties.Resources.appicon;
            PopulateGrid();
        }

        private void PopulateGrid()
        {
            scheduleGrid.Rows.Clear();
            for (int hour = 0; hour < HourlySchedule.HoursPerDay; hour++)
            {
                int limit = _schedule.GetLimit(hour);
                string limitText = limit == HourlySchedule.UseMasterLimit
                    ? string.Empty
                    : limit.ToString(CultureInfo.InvariantCulture);

                scheduleGrid.Rows.Add(
                    hour.ToString("00", CultureInfo.InvariantCulture) + ":00",
                    limitText);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            scheduleGrid.EndEdit();

            int[] limits = new int[HourlySchedule.HoursPerDay];
            for (int hour = 0; hour < HourlySchedule.HoursPerDay; hour++)
            {
                string raw = Convert.ToString(scheduleGrid.Rows[hour].Cells[1].Value);
                string text = (raw ?? string.Empty).Trim();

                if (text.Length == 0)
                {
                    limits[hour] = HourlySchedule.UseMasterLimit;
                    continue;
                }

                int limit;
                if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out limit)
                    || limit < 0
                    || limit > 100)
                {
                    MessageBox.Show(
                        this,
                        string.Format(
                            CultureInfo.CurrentCulture,
                            "Enter a volume limit from 0 to 100 for {0}, or leave it blank to use the master limit.",
                            hour.ToString("00", CultureInfo.InvariantCulture) + ":00"),
                        "Invalid limit",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    scheduleGrid.CurrentCell = scheduleGrid.Rows[hour].Cells[1];
                    scheduleGrid.BeginEdit(true);
                    return;
                }

                limits[hour] = limit;
            }

            _schedule.Limits = limits;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}

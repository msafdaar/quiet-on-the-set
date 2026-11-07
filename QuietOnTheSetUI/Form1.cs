using Microsoft.Win32;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuietOnTheSetUI
{
    public partial class Form1 : Form
    {
        MMDeviceEnumerator MMDE = new MMDeviceEnumerator();
        MMDevice mmDevice;
        private bool _isLocked = false;
        private string _password;
        private int _masterMaxVolume;
        private bool _exitAllowed = false;
        private HourlySchedule _hourlySchedule;
        private readonly object _scheduleLock = new object();
        private bool _loadingSchedule;

        // Removes the app from Alt+Tab window if minimized
        protected override CreateParams CreateParams
        {
            get
            {
                var Params = base.CreateParams;
                if (FormWindowState.Minimized == this.WindowState)
                {
                    Params.ExStyle |= 0x80;
                }
                return Params;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                base.OnShown(e);
                Hide();
            }
        }

        public Form1()
        {
            InitializeComponent();
            //            Bitmap applicationIcon = QuietOnTheSetUI.Properties.Resources.appicon;

            try
            {
                checkBox1.Checked = Convert.ToBoolean(Properties.Settings.Default["StartAutomatically"]);
                checkBox2.Checked = Convert.ToBoolean(Properties.Settings.Default["StartMinimized"]);
            }
            catch (Exception)
            {
                checkBox1.Checked = false;
                checkBox2.Checked = false;
            }

            if (checkBox2.Checked)
            {
                //  Hides the app completely
                Form1_FormClosing(null, new FormClosingEventArgs(new CloseReason(), true));

                //  The volume is automatically locked if the app is minimized 
                Properties.Settings.Default["IsLocked"] = true;
                Properties.Settings.Default.Save();
            }

            this.Icon = QuietOnTheSetUI.Properties.Resources.appicon;
            mmDevice = MMDE.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            notifyIcon1.Icon = QuietOnTheSetUI.Properties.Resources.appicon;
            volumeTrackBar.ValueChanged += VolumeTrackBar_ValueChanged;
            mmDevice.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
            _masterMaxVolume = Convert.ToInt16(Properties.Settings.Default["MaxVolume"]);
            volumeTrackBar.Value = _masterMaxVolume;
            _isLocked = Convert.ToBoolean(Properties.Settings.Default["IsLocked"]);
            _hourlySchedule = HourlyScheduleStore.Load();
            _loadingSchedule = true;
            checkBoxSchedules.Checked = _hourlySchedule.Enabled;
            _loadingSchedule = false;
            _password = Properties.Settings.Default["UnlockCode"].ToString();
            currentVolumeLabel.Text = Convert.ToInt16(mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100).ToString();
            notifyIcon1.BalloonTipTitle = $"Quiet on the Set";
            if (_isLocked)
            {
                LockVolume(true);
            }
            else
            {
                UnlockVolume();
            }

            this.FormClosing += Form1_FormClosing;
            this.Resize += Form1_Resize;

            scheduleTimer.Start();
            UpdateFooter();
        }

        private void UpdateFooter()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime;

            footerLabel.Text = $"v{version} was built {buildDate.ToString("g")}";
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(500);
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                notifyIcon1.Visible = false;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_exitAllowed == false)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
            }
        }

        internal void LockVolume(bool initializing = false)
        {
            _isLocked = true;
            lockButton.Text = "Unlock";
            volumeTrackBar.Enabled = false;
            checkBoxSchedules.Enabled = false;
            editScheduleButton.Enabled = false;
            if (!initializing)
            {
                _masterMaxVolume = volumeTrackBar.Value;
                Properties.Settings.Default["MaxVolume"] = _masterMaxVolume.ToString();
                Properties.Settings.Default["IsLocked"] = true;
                Properties.Settings.Default["UnlockCode"] = passwordTextBox.Text;
                Properties.Settings.Default.Save();
            }
            _password = passwordTextBox.Text;
            passwordTextBox.Text = string.Empty;
            confirmPasswordTextBox.Text = string.Empty;
            notifyIcon1.BalloonTipText = _balloonTipText;
            notifyIcon1.Text = _balloonTipText;
            if (_password.Length > 0) { lockButton.Enabled = false; }
            exitButton.Visible = false;
            SetMaxVolume();
            UpdateMaxVolumeLabel();
        }
        internal void UnlockVolume()
        {
            _isLocked = false;
            lockButton.Text = "Lock";
            volumeTrackBar.Enabled = true;
            checkBoxSchedules.Enabled = true;
            editScheduleButton.Enabled = true;
            Properties.Settings.Default["IsLocked"] = false;
            Properties.Settings.Default["UnlockCode"] = string.Empty;
            Properties.Settings.Default.Save();
            passwordTextBox.Text = string.Empty;
            confirmPasswordTextBox.Text = string.Empty;
            notifyIcon1.BalloonTipText = _balloonTipText;
            notifyIcon1.Text = _balloonTipText;
            exitButton.Visible = true;
            _password = string.Empty;
            UpdateMaxVolumeLabel();
        }

        private string _balloonTipText
        {
            get
            {
                if (_isLocked)
                {
                    string scheduleSuffix = IsScheduleOverrideActive() ? " (time schedule)" : string.Empty;
                    return $"Maximum volume locked at {GetEffectiveMaxVolume()}{scheduleSuffix}";
                }
                else
                {
                    return $"No maximum volume is currently set";
                }
            }
        }

        private int GetEffectiveMaxVolume()
        {
            if (!_isLocked)
            {
                return _masterMaxVolume;
            }

            lock (_scheduleLock)
            {
                if (_hourlySchedule == null || !_hourlySchedule.Enabled)
                {
                    return _masterMaxVolume;
                }

                int hourLimit = _hourlySchedule.GetLimit(DateTime.Now.Hour);
                return hourLimit == HourlySchedule.UseMasterLimit ? _masterMaxVolume : hourLimit;
            }
        }

        private bool IsScheduleOverrideActive()
        {
            if (!_isLocked)
            {
                return false;
            }

            lock (_scheduleLock)
            {
                return _hourlySchedule != null
                    && _hourlySchedule.Enabled
                    && _hourlySchedule.GetLimit(DateTime.Now.Hour) != HourlySchedule.UseMasterLimit;
            }
        }

        private void SetMaxVolume()
        {
            int effectiveMaxVolume = GetEffectiveMaxVolume();
            if (mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar > (effectiveMaxVolume / 100f))
            {
                mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar = effectiveMaxVolume / 100f;
            }
        }

        private void UpdateMaxVolumeLabel()
        {
            maxVolumeLabel.Text = GetEffectiveMaxVolume().ToString();
        }

        private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            var newVolume = Convert.ToInt16(data.MasterVolume * 100);
            if (_isLocked && newVolume > GetEffectiveMaxVolume())
            {
                SetMaxVolume();
            }
            if (currentVolumeLabel.InvokeRequired)
            {
                currentVolumeLabel.Invoke(new MethodInvoker(delegate { currentVolumeLabel.Text = newVolume.ToString(); }));
            }
        }

        private void VolumeTrackBar_ValueChanged(object sender, EventArgs e)
        {
            UpdateMaxVolumeLabel();
        }

        private void volumeTrackBar_Scroll(object sender, EventArgs e)
        {
            UpdateMaxVolumeLabel();
        }

        private void scheduleTimer_Tick(object sender, EventArgs e)
        {
            RefreshScheduleState();
        }

        private void RefreshScheduleState()
        {
            UpdateMaxVolumeLabel();
            if (_isLocked)
            {
                SetMaxVolume();
            }

            notifyIcon1.BalloonTipText = _balloonTipText;
            notifyIcon1.Text = _balloonTipText;
        }

        private void checkBoxSchedules_CheckedChanged(object sender, EventArgs e)
        {
            if (_loadingSchedule || _hourlySchedule == null)
            {
                return;
            }

            HourlySchedule scheduleToSave;
            lock (_scheduleLock)
            {
                _hourlySchedule.Enabled = checkBoxSchedules.Checked;
                scheduleToSave = _hourlySchedule.Clone();
            }

            SaveSchedule(scheduleToSave);
            RefreshScheduleState();
        }

        private void editScheduleButton_Click(object sender, EventArgs e)
        {
            if (_isLocked)
            {
                return;
            }

            HourlySchedule workingCopy;
            lock (_scheduleLock)
            {
                workingCopy = _hourlySchedule.Clone();
            }

            using (var form = new HourlyScheduleForm(workingCopy))
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                HourlySchedule updated = form.Schedule.Clone();
                lock (_scheduleLock)
                {
                    _hourlySchedule = updated;
                }

                SaveSchedule(updated);
                RefreshScheduleState();
            }
        }

        private void SaveSchedule(HourlySchedule schedule)
        {
            try
            {
                HourlyScheduleStore.Save(schedule);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "Could not save the time schedule: " + ex.Message,
                    "Time Schedule",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void lockButton_Click(object sender, EventArgs e)
        {
            if (_isLocked)
            {
                UnlockVolume();
            }
            else
            {
                LockVolume();
            }
        }

        private void passwordTextBox_TextChanged(object sender, EventArgs e)
        {
            ValidatePasswords();
        }

        private void confirmPasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            ValidatePasswords();
        }

        internal void ValidatePasswords()
        {
            if (_isLocked)
            {
                lockButton.Enabled = passwordTextBox.Text.Equals(confirmPasswordTextBox.Text) && passwordTextBox.Text.Equals(_password);
            }
            else
            {
                lockButton.Enabled = passwordTextBox.Text.Equals(confirmPasswordTextBox.Text);
            }
        }

        private void showPasswordCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            var isChecked = ((CheckBox)sender).Checked;
            passwordTextBox.UseSystemPasswordChar = !isChecked;
            confirmPasswordTextBox.UseSystemPasswordChar = !isChecked;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Show must be called before setting WindowState,
            // otherwise the window loses its size and position
            this.Show();
            this.WindowState = FormWindowState.Normal;
            MaxmizedFromTray();
        }

        private void MaxmizedFromTray()
        {
            notifyIcon1.Visible = false;
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            var response = MessageBox.Show("This will completely shut down the volume control so users can set the volume as loud as they want. Are you sure you want to exit?", "Warning", MessageBoxButtons.YesNo);
            if (response == DialogResult.Yes)
            {
                _exitAllowed = true;
                Application.Exit();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (checkBox1.Checked)
            {
                rk.SetValue("QuietOnTheSet", Application.ExecutablePath.ToString());
            }
            else
            {
                rk.DeleteValue("QuietOnTheSet", false);
            }

            Properties.Settings.Default["StartAutomatically"] = checkBox1.Checked;
            Properties.Settings.Default.Save();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["StartMinimized"] = checkBox2.Checked;
            Properties.Settings.Default.Save();
        }
    }
}

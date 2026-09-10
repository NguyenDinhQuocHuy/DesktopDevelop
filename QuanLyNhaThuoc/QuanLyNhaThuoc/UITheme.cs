using System.Drawing;
using System.Windows.Forms;

namespace QuanLyNhaThuoc
{
    internal static class UITheme
    {
        // Base sizes and colors for a larger, easy-to-read, and soothing theme
        private static readonly Font BaseFont = new Font("Segoe UI", 10.5F);
        private static readonly Color FormBackColor = Color.FromArgb(249, 252, 253); // very light
        private static readonly Color PanelHeaderColor = Color.FromArgb(220, 235, 245); // soft blue
        private static readonly Color ButtonColor = Color.FromArgb(76, 175, 80); // soft green
        private static readonly Color ButtonForeColor = Color.White;

        public static void ApplyToForm(Form form)
        {
            if (form == null) return;

            form.Font = BaseFont;
            form.BackColor = FormBackColor;

            // Apply to all controls recursively
            ApplyToControls(form.Controls);

            // If form has a top panel named pnlHeader, give it a soft header color and larger height
            var header = form.Controls["pnlHeader"] as Panel;
            if (header != null)
            {
                header.BackColor = PanelHeaderColor;
                header.Height = Math.Max(header.Height, 56);
                // try to enlarge header label
                if (header.Controls.Count > 0 && header.Controls[0] is Label lbl)
                {
                    lbl.Font = new Font(lbl.Font.FontFamily, 14F, FontStyle.Bold);
                }
            }
        }

        private static void ApplyToControls(Control.ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                // Increase font for most controls
                try { c.Font = BaseFont; } catch { }

                // Specific tweaks per control type
                switch (c)
                {
                    case Button b:
                        b.BackColor = ButtonColor;
                        b.ForeColor = ButtonForeColor;
                        b.FlatStyle = FlatStyle.Flat;
                        b.Height = Math.Max(b.Height, 34);
                        b.Padding = new Padding(6);
                        break;
                    case Label l:
                        l.ForeColor = Color.FromArgb(40, 40, 40);
                        break;
                    case TextBox t:
                        t.Height = Math.Max(t.Height, 28);
                        break;
                    case ComboBox cb:
                        cb.Height = Math.Max(cb.Height, 28);
                        break;
                    case NumericUpDown n:
                        n.Height = Math.Max(n.Height, 28);
                        break;
                    case DataGridView dgv:
                        dgv.EnableHeadersVisualStyles = false;
                        dgv.BackgroundColor = Color.White;
                        dgv.RowTemplate.Height = 30;
                        dgv.ColumnHeadersDefaultCellStyle.Font = BaseFont;
                        dgv.DefaultCellStyle.Font = BaseFont;
                        dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(235, 240, 245);
                        dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(30, 30, 30);
                        dgv.GridColor = Color.FromArgb(220, 230, 235);
                        break;
                    case GroupBox g:
                        g.Padding = new Padding(8);
                        break;
                    case TabControl tc:
                        tc.ItemSize = new Size(Math.Max(tc.ItemSize.Width, 120), Math.Max(tc.ItemSize.Height, 30));
                        break;
                }

                // Recurse into child controls
                if (c.HasChildren)
                    ApplyToControls(c.Controls);
            }
        }
    }
}

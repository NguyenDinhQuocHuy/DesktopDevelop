using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLyDichVuKhachSan.Helpers
{
    public enum InputValidationMode
    {
        None,
        NameOnly,   // Không cho phép nhập chữ số (chỉ cho phép chữ cái và khoảng trắng)
        PhoneOnly,  // Chỉ cho phép nhập chữ số (0-9)
        DecimalOnly // Chỉ cho phép nhập số thực/thập phân (0-9 và dấu chấm/phẩy)
    }

    /// <summary>
    /// Attached Property hỗ trợ kiểm soát nhập liệu thời gian thực trên TextBox (chặn gõ phím & chặn dán nội dung sai định dạng)
    /// </summary>
    public static class InputHelper
    {
        public static readonly DependencyProperty ValidationModeProperty =
            DependencyProperty.RegisterAttached(
                "ValidationMode",
                typeof(InputValidationMode),
                typeof(InputHelper),
                new PropertyMetadata(InputValidationMode.None, OnValidationModeChanged));

        public static InputValidationMode GetValidationMode(DependencyObject obj)
        {
            return (InputValidationMode)obj.GetValue(ValidationModeProperty);
        }

        public static void SetValidationMode(DependencyObject obj, InputValidationMode value)
        {
            obj.SetValue(ValidationModeProperty, value);
        }

        private static void OnValidationModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.PreviewTextInput -= TextBox_PreviewTextInput;
                DataObject.RemovePastingHandler(textBox, TextBox_Pasting);

                var mode = (InputValidationMode)e.NewValue;
                if (mode != InputValidationMode.None)
                {
                    textBox.PreviewTextInput += TextBox_PreviewTextInput;
                    DataObject.AddPastingHandler(textBox, TextBox_Pasting);
                }
            }
        }

        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is not TextBox textBox) return;
            var mode = GetValidationMode(textBox);

            if (mode == InputValidationMode.NameOnly)
            {
                // Không cho phép chữ số
                if (e.Text.Any(char.IsDigit))
                {
                    e.Handled = true;
                }
            }
            else if (mode == InputValidationMode.PhoneOnly)
            {
                // Chỉ cho phép chữ số (0-9)
                if (e.Text.Any(c => !char.IsDigit(c)))
                {
                    e.Handled = true;
                }
            }
            else if (mode == InputValidationMode.DecimalOnly)
            {
                // Chỉ cho phép số thập phân dương (0-9 và dấu . hoặc ,)
                int selectionStart = textBox.SelectionStart;
                int selectionLength = textBox.SelectionLength;
                string currentText = textBox.Text ?? "";
                string newText = currentText.Remove(selectionStart, selectionLength).Insert(selectionStart, e.Text);

                if (!Regex.IsMatch(newText, @"^\d*([.,]\d*)?$"))
                {
                    e.Handled = true;
                }
            }
        }

        private static void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (sender is not TextBox textBox) return;
            var mode = GetValidationMode(textBox);

            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string text = (string)e.DataObject.GetData(DataFormats.Text);
                if (mode == InputValidationMode.NameOnly)
                {
                    if (text.Any(char.IsDigit))
                    {
                        e.CancelCommand();
                    }
                }
                else if (mode == InputValidationMode.PhoneOnly)
                {
                    if (text.Any(c => !char.IsDigit(c)))
                    {
                        e.CancelCommand();
                    }
                }
                else if (mode == InputValidationMode.DecimalOnly)
                {
                    int selectionStart = textBox.SelectionStart;
                    int selectionLength = textBox.SelectionLength;
                    string currentText = textBox.Text ?? "";
                    string newText = currentText.Remove(selectionStart, selectionLength).Insert(selectionStart, text);

                    if (!Regex.IsMatch(newText, @"^\d*([.,]\d*)?$"))
                    {
                        e.CancelCommand();
                    }
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        // ==================== CÁC HÀM VALIDATION DỮ LIỆU DÙNG CHUNG ====================

        public static bool ContainsDigits(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            return text.Any(char.IsDigit);
        }

        public static bool ContainsNonDigits(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            return text.Any(c => !char.IsDigit(c));
        }

        public static string FilterOutDigits(string? text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return new string(text.Where(c => !char.IsDigit(c)).ToArray());
        }

        public static string FilterOnlyDigits(string? text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return new string(text.Where(char.IsDigit).ToArray());
        }
    }
}

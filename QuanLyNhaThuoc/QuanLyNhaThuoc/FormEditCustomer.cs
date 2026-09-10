using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace QuanLyNhaThuoc
{
    public partial class FormEditCustomer : Form
    {
        private Customer _customer;
        private List<Customer> _allCustomers;

        public FormEditCustomer(Customer customer, List<Customer> allCustomers)
        {
            InitializeComponent();
            _customer = customer;
            _allCustomers = allCustomers;

            txtPhone.Text = _customer.NumPhone;
            txtName.Text = _customer.Name;
            nudPoints.Value = _customer.Point;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string newPhone = txtPhone.Text.Trim();
            string newName = txtName.Text.Trim();
            int newPoints = (int)nudPoints.Value;

            if (string.IsNullOrWhiteSpace(newPhone))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhone.Focus();
                return;
            }

            if (!Regex.IsMatch(newPhone, @"^\d{9,11}$"))
            {
                MessageBox.Show("Số điện thoại không hợp lệ! (Phải từ 9 - 11 chữ số)", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhone.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(newName))
            {
                MessageBox.Show("Vui lòng nhập họ và tên khách hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            // Check duplicate phone if phone number changed
            if (!newPhone.Equals(_customer.NumPhone, StringComparison.OrdinalIgnoreCase))
            {
                if (_allCustomers.Any(x => x.NumPhone.Equals(newPhone, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("Số điện thoại này đã thuộc về khách hàng khác!", "Lỗi Trùng Lặp", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPhone.Focus();
                    return;
                }
            }

            _customer.NumPhone = newPhone;
            _customer.Name = newName;
            _customer.Point = newPoints;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}

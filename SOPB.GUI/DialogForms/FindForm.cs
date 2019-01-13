using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOPB.GUI.DialogForms
{
    public partial class FindForm : Form
    {
        public string LastName { get; private set; }
        public string[]  OtherFields { get; private set; }
        public FindForm(string name, params string[] other)
        {
            InitializeComponent();
            label1.Text = name;
            this.Text = "Поиск: " + name;
            if (other.Length > 0)
            {
                labelOther.Visible = true;
                labelOther.Text = other[0];
                textBoxOtherField.Visible = true;
                OtherFields = other;
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxLastName.Text))
            {
                this.DialogResult = MessageBox.Show(@"Вы не ввели текст для поиска! Повторить ввод?", @"Пустая Строка", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                ClearTextBox();
                if (this.DialogResult == DialogResult.No)
                {
                    LastName = String.Empty;
                    OtherFields[0] = String.Empty;
                    this.Close();
                }

            }
            else
            {
                 LastName = textBoxLastName.Text;
                if (OtherFields != null ) OtherFields[0] = textBoxOtherField.Text;
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        private void ClearTextBox()
        {
            textBoxLastName.Clear();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if ((DialogResult == DialogResult.Yes)) e.Cancel = true;
        }
    }
}

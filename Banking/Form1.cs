using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BankingModel;
using System.Threading;

namespace Banking
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SavingsAccount acc = new SavingsAccount();
        Customer cust = new Customer();

        List<Customer> searchResultCustomer = new List<Customer>();
        finalProjectDBEntities context = new finalProjectDBEntities();

        public static int listIndex;

        private void Form1_Load(object sender, EventArgs e)
        {
            this.savingsAccountsTableAdapter.Fill(this.finalProjectDBDataSet.SavingsAccounts);
            this.customersTableAdapter.Fill(this.finalProjectDBDataSet.Customers);

            txtName.Clear();
            txtAddress.Clear();
            txtPhoneNumber.Clear();

            txtAccount.Clear();
            txtBalance.Clear();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            acc.AccountId = int.Parse(txtAccount.Text);
            acc.Balance = 0;
            acc.Interest = 0.01M;

            cust.Name = txtName.Text;
            cust.Address = txtAddress.Text;
            cust.Phone = txtPhoneNumber.Text;
            cust.AccountId = int.Parse(txtAccount.Text);

            Validate();

            try
            {
                try
                {
                    this.savingsAccountsTableAdapter.Insert(acc.AccountId, acc.Balance, acc.Interest);
                }
                catch
                {
                    Console.WriteLine("savings account already exists");
                }

                this.customersTableAdapter.Insert(cust.Name, cust.Address, cust.Phone, cust.AccountId);

            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
            }

        }

        public void GetAccount()
        {

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            //will update acct everytime, but won't update customer
            acc.AccountId = int.Parse(txtAccount.Text);
            acc.Balance = decimal.Parse(txtBalance.Text);

            var accRow = finalProjectDBDataSet.SavingsAccounts.FindByAccountId(acc.AccountId);

            accRow.Balance = acc.Balance;
            accRow.AccountId = acc.AccountId;

            cust.Address = txtAddress.Text;
            cust.Phone = txtPhoneNumber.Text;
            cust.AccountId = int.Parse(txtAccount.Text);

            var custRow = finalProjectDBDataSet.Customers.FindByCustomerId(cust.CustomerId);
            custRow.SetModified();

            custRow.Name = txtName.Text;
            custRow.Address = cust.Address;
            custRow.Phone = cust.Phone;
            custRow.AccountId = cust.AccountId;

            

            try
            {
                this.Validate();
                this.customersTableAdapter.Update(custRow);

                this.savingsAccountsTableAdapter.Update(accRow);

                cust.Name = txtName.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show("update failed");
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //can't find newly made changes until stopped and restarted
            

            List<Customer> customerSearch = new List<Customer>();
            List<SavingsAccount> accountSearch = new List<SavingsAccount>();
            var search = txtName.Text;

           

            using (finalProjectDBEntities context = new finalProjectDBEntities())
            {
                var query =
                context.Customers
                .Where(cust => cust.Name == search);

                customerSearch.AddRange(query);
            }

            foreach (var element in customerSearch)
            {
                searchResultCustomer.Add(element);
            }

            if (searchResultCustomer.Count > 1)
            {
                btnNext.Enabled = true;
                listIndex = 0;
            }

            try
            {
                var custInfo = finalProjectDBDataSet.Customers.FindByCustomerId(searchResultCustomer[0].CustomerId);

                txtName.Text = custInfo.Name.ToString();
                txtAddress.Text = custInfo.Address.ToString();
                txtPhoneNumber.Text = custInfo.Phone.ToString();

                cust.Name = custInfo.Name.ToString();
                cust.Address = custInfo.Address.ToString();
                cust.Phone = custInfo.Phone.ToString();
                cust.AccountId = custInfo.AccountId;
                cust.CustomerId = searchResultCustomer[0].CustomerId;

                var accInfo = finalProjectDBDataSet.SavingsAccounts.FindByAccountId(custInfo.AccountId);

                txtBalance.Text = accInfo.Balance.ToString();
                txtAccount.Text = accInfo.AccountId.ToString();

                acc.Balance = accInfo.Balance;
                acc.AccountId = accInfo.AccountId;
                acc.Interest = accInfo.Interest;
            }
            catch
            {
                MessageBox.Show("Error: Customer does not exist");
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            List<SavingsAccount> accountSearch = new List<SavingsAccount>();
            Customer preItem = new Customer();

            if (listIndex > 0)
            {
                preItem = searchResultCustomer[listIndex - 1];
                var custInfo = finalProjectDBDataSet.Customers.FindByCustomerId(preItem.CustomerId);

                listIndex--;

                txtName.Text = custInfo.Name;
                txtAddress.Text = custInfo.Address;
                txtPhoneNumber.Text = custInfo.Phone;

                cust.Name = custInfo.Name.ToString();
                cust.Address = custInfo.Address.ToString();
                cust.Phone = custInfo.Phone.ToString();
                cust.AccountId = custInfo.AccountId;
                cust.CustomerId = preItem.CustomerId;

                var accInfo = finalProjectDBDataSet.SavingsAccounts.FindByAccountId(custInfo.AccountId);

                txtBalance.Text = accInfo.Balance.ToString();
                txtAccount.Text = accInfo.AccountId.ToString();

                acc.Balance = accInfo.Balance;
                acc.AccountId = accInfo.AccountId;
                acc.Interest = accInfo.Interest;

                btnNext.Enabled = true;
            }

            if (listIndex == 0)
            {
                btnPrevious.Enabled = false;
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            List<SavingsAccount> accountSearch = new List<SavingsAccount>();
            
            Customer nextItem = new Customer();

            if (listIndex < searchResultCustomer.Count - 1)
            {
                nextItem = searchResultCustomer[listIndex + 1];
                var custInfo = finalProjectDBDataSet.Customers.FindByCustomerId(nextItem.CustomerId);

                listIndex++;

                txtName.Text = custInfo.Name;
                txtAddress.Text = custInfo.Address;
                txtPhoneNumber.Text = custInfo.Phone;

                cust.Name = custInfo.Name.ToString();
                cust.Address = custInfo.Address.ToString();
                cust.Phone = custInfo.Phone.ToString();
                cust.AccountId = custInfo.AccountId;
                cust.CustomerId = nextItem.CustomerId;

                var accInfo = finalProjectDBDataSet.SavingsAccounts.FindByAccountId(custInfo.AccountId);

                txtBalance.Text = accInfo.Balance.ToString();
                txtAccount.Text = accInfo.AccountId.ToString();

                acc.Balance = accInfo.Balance;
                acc.AccountId = accInfo.AccountId;
                acc.Interest = accInfo.Interest;

                btnPrevious.Enabled = true;
            }


            if (listIndex == searchResultCustomer.Count - 1)
            {
                btnNext.Enabled = false;
            }
        }

        private void btnDeposit_Click(object sender, EventArgs e)
        {
            acc.Balance += int.Parse(txtWithdrawDeposit.Text);
            txtBalance.Text = acc.Balance.ToString();
        }

        private void btnWithdraw_Click(object sender, EventArgs e)
        {
            acc.Balance -= int.Parse(txtWithdrawDeposit.Text);
            txtBalance.Text = acc.Balance.ToString();
        }

        private void btnInterest_Click(object sender, EventArgs e)
        {
            var calc = acc.Interest * int.Parse(txtIntMonth.Text);
            txtCalcInterest.Text = calc.ToString();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtName.Clear();
            txtAddress.Clear();
            txtPhoneNumber.Clear();

            txtAccount.Clear();
            txtBalance.Clear();
            txtWithdrawDeposit.Clear();

            txtIntMonth.Clear();
            txtCalcInterest.Clear();

            searchResultCustomer.Clear();

            btnPrevious.Enabled = false;
            btnNext.Enabled = false;
        }
    }
}
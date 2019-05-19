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
        List<SavingsAccount> searchResultAccount = new List<SavingsAccount>();

        public static int listIndex;

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'finalProjectDBDataSet.SavingsAccounts' table. You can move, or remove it, as needed.
            this.savingsAccountsTableAdapter.Fill(this.finalProjectDBDataSet.SavingsAccounts);
            // TODO: This line of code loads data into the 'finalProjectDBDataSet.Customers' table. You can move, or remove it, as needed.
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

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            this.savingsAccountsBindingSource.EndEdit();
            var accRow = finalProjectDBDataSet.SavingsAccounts.FindByAccountId(acc.AccountId);

            accRow.AccountId = int.Parse(txtAccount.Text);
            accRow.Balance = decimal.Parse(txtBalance.Text);

            this.customersBindingSource.EndEdit();
            var custRow = finalProjectDBDataSet.Customers.FindByCustomerId(cust.CustomerId);

            custRow.Name = txtName.Text;
            custRow.Address = txtAddress.Text;
            custRow.Phone = txtPhoneNumber.Text;
            custRow.AccountId = int.Parse(txtAccount.Text);

            try
            {
                this.Validate();
                this.savingsAccountsTableAdapter.Update(accRow);

                this.customersTableAdapter.Update(custRow);
            }
            catch (Exception ex)
            {
                MessageBox.Show("update failed");
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //TODO: Not finding the last added people
            var search = txtName.Text;
            finalProjectDBEntities dbcontext = new finalProjectDBEntities();


            dbcontext.Customers.Load();
            customersBindingSource.DataSource = dbcontext.Customers.Local;

            dbcontext.SavingsAccounts.Load();
            savingsAccountsBindingSource.DataSource = dbcontext.SavingsAccounts.Local;

            var customerSearch =
               dbcontext.Customers.Local
               .Where(cust => cust.Name == search);

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
                txtName.Text = searchResultCustomer[0].Name.ToString();
                txtAddress.Text = searchResultCustomer[0].Address.ToString();
                txtPhoneNumber.Text = searchResultCustomer[0].Phone.ToString();

                cust.Name = searchResultCustomer[0].Name.ToString();
                cust.Address = searchResultCustomer[0].Address.ToString();
                cust.Phone = searchResultCustomer[0].Phone.ToString();
                cust.AccountId = searchResultCustomer[0].AccountId;
                cust.CustomerId = searchResultCustomer[0].CustomerId;

                txtBalance.Text = searchResultCustomer[0].SavingsAccount.Balance.ToString();
                txtAccount.Text = searchResultCustomer[0].SavingsAccount.AccountId.ToString();

                acc.Balance = searchResultCustomer[0].SavingsAccount.Balance;
                acc.AccountId = searchResultCustomer[0].SavingsAccount.AccountId;
                acc.Interest = searchResultCustomer[0].SavingsAccount.Interest;
            }
            catch
            {
                MessageBox.Show("Error: Customer does not exist");
            }
        }

        //previous is working
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            Customer preItem = new Customer();

            if (listIndex > 0)
            {
                preItem = searchResultCustomer[listIndex - 1];

                listIndex--;

                txtName.Text = preItem.Name;
                txtAddress.Text = preItem.Address;
                txtPhoneNumber.Text = preItem.Phone;

                cust.Name = preItem.Name.ToString();
                cust.Address = preItem.Address.ToString();
                cust.Phone = preItem.Phone.ToString();
                cust.AccountId = preItem.AccountId;
                cust.CustomerId = preItem.CustomerId;

                txtBalance.Text = preItem.SavingsAccount.Balance.ToString();
                txtAccount.Text = preItem.SavingsAccount.AccountId.ToString();

                acc.Balance = preItem.SavingsAccount.Balance;
                acc.AccountId = preItem.SavingsAccount.AccountId;
                acc.Interest = preItem.SavingsAccount.Interest;

                btnNext.Enabled = true;
            }

            if (listIndex == 0)
            {
                btnPrevious.Enabled = false;
            }
        }

        //next is working
        private void btnNext_Click(object sender, EventArgs e)
        {
            Customer nextItem = new Customer();
            if (listIndex < searchResultCustomer.Count - 1)
            {
                nextItem = searchResultCustomer[listIndex + 1];

                listIndex++;

                txtName.Text = nextItem.Name;
                txtAddress.Text = nextItem.Address;
                txtPhoneNumber.Text = nextItem.Phone;

                cust.Name = nextItem.Name.ToString();
                cust.Address = nextItem.Address.ToString();
                cust.Phone = nextItem.Phone.ToString();
                cust.AccountId = nextItem.AccountId;
                cust.CustomerId = nextItem.CustomerId;

                txtBalance.Text = nextItem.SavingsAccount.Balance.ToString();
                txtAccount.Text = nextItem.SavingsAccount.AccountId.ToString();

                acc.Balance = nextItem.SavingsAccount.Balance;
                acc.AccountId = nextItem.SavingsAccount.AccountId;
                acc.Interest = nextItem.SavingsAccount.Interest;

                btnPrevious.Enabled = true;
            }


            if (listIndex == searchResultCustomer.Count - 1)
            {
                btnNext.Enabled = false;
            }
        }

        //Deposit is working
        private void btnDeposit_Click(object sender, EventArgs e)
        {
            acc.Balance += int.Parse(txtWithdrawDeposit.Text);
            txtBalance.Text = acc.Balance.ToString();
        }

        //Withdraw is working
        private void btnWithdraw_Click(object sender, EventArgs e)
        {
            acc.Balance -= int.Parse(txtWithdrawDeposit.Text);
            txtBalance.Text = acc.Balance.ToString();
        }

        //Interst is working
        private void btnInterest_Click(object sender, EventArgs e)
        {
            var calc = acc.Interest * int.Parse(txtIntMonth.Text);
            txtCalcInterest.Text = calc.ToString();
        }

        //clear is working
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
            searchResultAccount.Clear();

            btnPrevious.Enabled = false;
            btnNext.Enabled = false;
        }
    }
}
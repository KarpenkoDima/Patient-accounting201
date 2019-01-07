using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SOPB.Accounting.DAL.ConnectionManager;
using SOPB.Accounting.DAL.TableAdapters.CustomerTableAdapter;

namespace SOPB.DAL.TEST
{
    [TestClass]
    public class TableAdapter
    {
        [TestInitialize]
        public void Init()
        {
            ConnectionManager.SetConnection(UserSettings.UserName, UserSettings.Password);
        }
        [TestMethod]
        public void TableAdapterConnection_TestMethod1()
        {
            Init();
            CustomerTableAdapter customerTableAdapter = new CustomerTableAdapter();
            customerTableAdapter.Connection = ConnectionManager.Connection;
            Assert.IsTrue(customerTableAdapter.Connection.State == System.Data.ConnectionState.Closed);
            
            customerTableAdapter.Connection.Open();
            Assert.IsTrue(customerTableAdapter.Connection.State == System.Data.ConnectionState.Open);
        }
        [TestMethod]
        public void CustomerTableAdapterFill_TestMethod()
        {
            Init();
            CustomerTableAdapter customerTableAdapter = new CustomerTableAdapter();
            customerTableAdapter.Connection = ConnectionManager.Connection;
            Assert.IsTrue(customerTableAdapter.Connection.State == System.Data.ConnectionState.Closed);

            customerTableAdapter.Connection.Open();
            Assert.IsTrue(customerTableAdapter.Connection.State == System.Data.ConnectionState.Open);
            DataTable dataTable = new DataTable("Customer");
            customerTableAdapter.Fill(dataTable);
            Assert.IsTrue(dataTable.Rows.Count > 0);
        }
        [TestMethod]
        public void AddressTableAdapterFill_TestMethod()
        {
            Init();
            AddressTableAdapter tableAdapter = new AddressTableAdapter();
            tableAdapter.Connection = ConnectionManager.Connection;
            Assert.IsTrue(tableAdapter.Connection.State == System.Data.ConnectionState.Closed);

            tableAdapter.Connection.Open();
            Assert.IsTrue(tableAdapter.Connection.State == System.Data.ConnectionState.Open);
            DataTable dataTable = new DataTable("Address");
            tableAdapter.Fill(dataTable);
            Assert.IsTrue(dataTable.Rows.Count > 0);
        }
        [TestMethod]
        public void InvalidTableAdapterFill_TestMethod()
        {
            Init();
            InvalidTableAdapter tableAdapter = new InvalidTableAdapter();
            tableAdapter.Connection = ConnectionManager.Connection;
            Assert.IsTrue(tableAdapter.Connection.State == System.Data.ConnectionState.Closed);

            tableAdapter.Connection.Open();
            Assert.IsTrue(tableAdapter.Connection.State == System.Data.ConnectionState.Open);
            DataTable dataTable = new DataTable("Invalid");
            tableAdapter.Fill(dataTable);
            Assert.IsTrue(dataTable.Rows.Count > 0);
        }
        [TestMethod]
        public void RegisterTableAdapterFill_TestMethod()
        {
            Init();
            RegisterTableAdapter tableAdapter = new RegisterTableAdapter();
            tableAdapter.Connection = ConnectionManager.Connection;
            Assert.IsTrue(tableAdapter.Connection.State == System.Data.ConnectionState.Closed);

            tableAdapter.Connection.Open();
            Assert.IsTrue(tableAdapter.Connection.State == System.Data.ConnectionState.Open);
            DataTable dataTable = new DataTable("Register");
            tableAdapter.Fill(dataTable);
            Assert.IsTrue(dataTable.Rows.Count > 0);
        }
    }

    static class UserSettings
    {
        public static string UserName = "Катя";
        public static string Password = "1";

        public static SqlConnection InitConnection()
        {
            Accounting.DAL.ConnectionManager.ConnectionManager.SetConnection(UserName, Password);
            return Accounting.DAL.ConnectionManager.ConnectionManager.Connection;
        }
    }
}

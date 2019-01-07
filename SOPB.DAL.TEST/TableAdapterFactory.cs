using System;
using System.Data;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SOPB.Accounting.DAL.ConnectionManager;
using SOPB.Accounting.DAL.LoadData;
using SOPB.Accounting.DAL.TableAdapters;

namespace SOPB.DAL.TEST
{
    [TestClass]
    public class TableAdapterFactoryTest
    {
        [TestInitialize]
        public void Init()
        {
            ConnectionManager.SetConnection(UserSettings.UserName, UserSettings.Password);
        }
        [TestMethod]
        public void BaseTableAdapterFillThrouthFactory_TestMethod1()
        {
            Init();
            string[] tablesName = new string[] { "Customer","Address","Invalid","Register"};
            DataTable dataTable;
            foreach (var item in tablesName)
            {
                dataTable = new DataTable(item);
                BaseTableAdapter baseTableAdapter = TableAdapterFactory.AdapterFactory(item);
                baseTableAdapter.Connection = ConnectionManager.Connection;
                if(baseTableAdapter.Connection.State != ConnectionState.Open)
                                    baseTableAdapter.Connection.Open();
                baseTableAdapter.Fill(dataTable);
                Assert.IsTrue(dataTable.Rows.Count>0);
                dataTable.Clear();
                dataTable.Dispose();
            }
        }
        [TestMethod]
        public void BaseTableAdapterFillThrouthFactoryAllTable_TestMethod1()
        {
            Init();
            string[] tablesName = new string[] {  "CUSTOMER", "ADDRESS" , "REGISTER", "APPPTPR",  "ADMINDIVISION", "TYPESTREET", "LAND", "WHYDEREGISTER", "REGISTERTYPE", "CHIPERRECEPT", "DISABILITYGROUP", "BENEFITSCATEGORY", "INVALID", "INVALIDBENEFITSCATEGORY", "GENDER"
};
            DataTable dataTable;
            foreach (var item in tablesName)
            {
                dataTable = new DataTable(item);
                BaseTableAdapter baseTableAdapter = TableAdapterFactory.AdapterFactory(item);
                baseTableAdapter.Connection = ConnectionManager.Connection;
                if (baseTableAdapter.Connection.State != ConnectionState.Open)
                    baseTableAdapter.Connection.Open();
                baseTableAdapter.Fill(dataTable);
                Assert.IsTrue(dataTable.Rows.Count > 0);
                dataTable.Clear();
                dataTable.Dispose();
            }
        }
        [TestMethod]
        public void Transaction_ReadData_TestMethod1()
        {
            Init();
            string[] tablesName = new string[] {  "CUSTOMER", "ADDRESS" , "REGISTER", "APPPTPR",  "ADMINDIVISION", "TYPESTREET", "LAND", "WHYDEREGISTER", "REGISTERTYPE", "CHIPERRECEPT", "DISABILITYGROUP", "BENEFITSCATEGORY", "INVALID", "INVALIDBENEFITSCATEGORY", "GENDER"
};
            DataTable dataTable;
            TransactionWork transaction;
            foreach (var item in tablesName)
            {
                dataTable = new DataTable(item);              

                using (transaction = new TransactionWork())
                {
                    transaction.ReadData(dataTable);
                    transaction.Commit();
                }
                Assert.IsTrue(dataTable.Rows.Count > 0);
                dataTable.Clear();
                dataTable.Dispose();
            }
        }
        [TestMethod]
        public void Transaction_UpdateMethod_TestMethod()
        {
            Init();
            string[] tablesName = new string[] {  "CUSTOMER", "ADDRESS" , "REGISTER", "APPPTPR",  "ADMINDIVISION", "TYPESTREET", "LAND", "WHYDEREGISTER", "REGISTERTYPE", "CHIPERRECEPT", "DISABILITYGROUP", "BENEFITSCATEGORY", "INVALID", "INVALIDBENEFITSCATEGORY", "GENDER"
};
            DataTable dataTable;
            TransactionWork transaction;
            Random randomRows = new Random();
            Random randomColumns = new Random();
            int rows;
            int columns;
            foreach (var item in tablesName)
            {
                dataTable = new DataTable(item);
               
                using (transaction = new TransactionWork())
                {
                    transaction.ReadData(dataTable);
                    rows = randomRows.Next(1, dataTable.Rows.Count - 1);
                    columns = randomColumns.Next(1, dataTable.Columns.Count - 1);
                    transaction.Commit();
                }
                for (int i = 1; i < rows; i++)
                {
                    for (int j = 1; j < columns; j++)
                    {
                        dataTable.Rows[i][j] = dataTable.Rows[i-1][j];
                        Debug.WriteLine(dataTable.Rows[i][j]);
                    }
                }
                using (transaction = new TransactionWork())
                {

                    transaction.UpdateData(dataTable);
                    transaction.Commit();
                }
                Assert.IsTrue(true);
                dataTable.Clear();
                dataTable.Dispose();
            }
        }
    }
}
                 
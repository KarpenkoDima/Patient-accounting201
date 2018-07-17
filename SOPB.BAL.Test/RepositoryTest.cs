using System;
using System.Data;
using System.Data.SqlClient;
using BAL.ORM.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SOPB.Accounting.DAL.ConnectionManager;

namespace SOPB.BAL.Test
{
    [TestClass]
    public class RepositoryTest
    {
        [TestInitialize]
        public void Init()
        {
            ConnectionManager.SetConnection(UserSettings.UserName, UserSettings.Password);
        }

        [TestMethod]
        // [DataRow("LastName", "Иванов")]
        [DataRow("Address", "славянск", "Ленина")]
        [DataRow("Birthday", "01/01/1999", "17/07/2018")]
        public void FinBy_test(string criteria, string value, string value2)
        {
            CustomRepository<string> repo = new CustomRepository<string>();
            DateTime date1, date2;
            DataSet ds;
            if (DateTime.TryParse(value, out date1) && DateTime.TryParse(value2, out date2))
            {
                ds = (DataSet)repo.FindBy(criteria, date1, date2, "МЕЖДУ");

            }
            else ds = (DataSet)repo.FindBy(criteria, new string[] { value, value2 });
            Assert.IsTrue(ds.Tables["Customer"].Rows.Count > 0);
        }
        [TestMethod]
        [DataRow(1, "Land")]
        [DataRow(1, "Gender")]
        [DataRow(2, "ApppTpr")]
        [DataRow(1, "RegisterType")]
        [DataRow(2, "RegisterType")]
        [DataRow(3, "RegisterType")]
        [DataRow(1, "SecondRegisterType")]
        [DataRow(3, "WhyDeRegister")]
        [DataRow(1, "WhySecondDeRegister")]
        public void FinByGlossary_test(int id, string criteria)
        {
            CustomRepository<string> repo = new CustomRepository<string>();
            DataSet ds = (DataSet)repo.FindByGlossary(id, criteria);
            Assert.IsTrue(ds.Tables["Customer"].Rows.Count > 0);
        }
        [TestMethod]
        [DataRow(1)]
        [DataRow(123)]
        [DataRow(2)]
        public void FinById_test(int id)
        {
            CustomRepository<string> repo = new CustomRepository<string>();
            DataSet ds = (DataSet)repo.FindByID(id);
            Assert.IsTrue(ds.Tables["Customer"].Rows.Count > 0);
        }
        [TestMethod]      
        public void Validation_Test()
        {
            CustomRepository<string> repo = new CustomRepository<string>();
            DataSet ds =(DataSet)repo.FillAll();
            repo.Validation();
            Assert.IsTrue(ds.Tables["Error"].Rows.Count > 0);
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

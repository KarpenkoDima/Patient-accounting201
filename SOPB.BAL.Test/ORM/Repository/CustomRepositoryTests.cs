using Microsoft.VisualStudio.TestTools.UnitTesting;
using BAL.ORM.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOPB.Accounting.DAL.ConnectionManager;
using SOPB.BAL.Test;
using System.Data;
using System.Diagnostics;

namespace BAL.ORM.Repository.Tests
{
    [TestClass()]
    public class CustomRepositoryTests
    {
        [TestInitialize]
        public void Init()
        {
            ConnectionManager.SetConnection(UserSettings.UserName, UserSettings.Password);
        }
        [TestMethod()]
        public void ValidatedTest()
        {
            Init();
            CustomRepository<string> repo = new CustomRepository<string>();
            DataSet ds = (DataSet)repo.FillAll();

            DataRowView row = ds.Tables["Register"].DefaultView.AddNew();           
            row[0] = ds.Tables["Register"].Rows[0][0];
            row["CustomerID"] = ds.Tables["Register"].Rows[0]["CustomerID"];
           
            row["FirstRegister"] = new Nullable<DateTime>(new DateTime(2015,1,10));
            row["FirstDeRegister"] = new Nullable<DateTime>(new DateTime(2011,1,1));
            row["SecondRegister"] = new Nullable<DateTime>(new DateTime(2009,1,1));
            row["SecondDeRegister"] = new Nullable<DateTime>(new DateTime(1999,1,1));
            repo.Validated(row);
            foreach(DataRow r in  ds.Tables["Error"].Rows)
                Debug.WriteLine(r[1] + " " + r[2].ToString()); 
            Assert.IsTrue(ds.Tables["Error"].Rows.Count > 0);
        }
    }
}
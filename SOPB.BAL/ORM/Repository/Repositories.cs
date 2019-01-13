using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using BAL.AccessData;
using SOPB.Accounting.DAL.LoadData;

namespace BAL.ORM.Repository
{
    public class CustomRepository<T> : RepositoryBase, IMutableRepository<T> where T : IComparable<T>
    {
        public bool IsCorrertError { get; set; } = false;
        private string _errorText = string.Empty;
        DateTime _maxDateTime = DateTime.Now;
        public CustomRepository()
        {
            
#if DEBUG
            IsCorrertError = false;
#endif        
        }
        public object FindBy(string criteria, params object[] values)
        {
            CustomerAccess.GetDataByCriteria(criteria, values);
            return CustomerAccess.GetData();
        }        
        public object FindByGlossary(int id, string name)
        {
            CustomerAccess.GetCustomersByGlossary(name, id);
            return CustomerAccess.GetData();
        }
        #region Interface and BaseClass Methods

        protected override string GetBaseQuery()
        {
            throw new NotImplementedException();
        }

        protected override string GetBaseWhereClause()
        {
            throw new NotImplementedException();
        }

        protected override string GetEntityName()
        {
            return "Customer";
        }

        protected override string GetKeyFieldName()
        {
            return "CustomerID";
        }

        protected override void BuildChildCallbacks()
        {
            throw new NotImplementedException();
        }

        
      
        public object FindByID(int id)
        {
            CustomerAccess.GetCustomersByID(id);
            return CustomerAccess.GetData();
        }

        public object FillAll()
        {
            CustomerAccess.FillDictionary();
            CustomerAccess.FillCustomerData();
            return CustomerAccess.GetData();
        }
        public void Update(IList<T> list)
        {
            CustomerAccess.Update();
        }
        #endregion

        public object GetEmpty()
        {
            return null;
        }

        public void ExportToExcel(string[] columns)
        {
            CustomerAccess.ExportToExcel();
        }

        internal void Validated()
        {
            CustomerAccess.Vapidated();
        }

        void IMutableRepository<T>.Update(IList<T> list)
        {
            throw new NotImplementedException();
        }

        object IRepository<T>.FindBy(string criteria, params T[] value)
        {
            return FindBy(criteria, value);
        }

        object IRepository<T>.FillAll()
        {
            return this.FillAll();
        }

        object IRepository<T>.FindByID(int id)
        {
            throw new NotImplementedException();
        }

        public void Validation()
        {
            CustomerAccess.Vapidated();
        }
        public void Validated(DataRowView row)
        {
            CustomerAccess.Vapidated(row);
        }
    }

    public class GlossaryRepository : RepositoryBase, IMutableRepository<Int32>
    {

        #region Override Methods

        protected override string GetBaseQuery()
        {
            throw new NotImplementedException();
        }

        protected override string GetBaseWhereClause()
        {
            throw new NotImplementedException();
        }

        protected override string GetEntityName()
        {
            return String.Empty;
        }

        protected override string GetKeyFieldName()
        {
            return String.Empty;
        }

        protected override void BuildChildCallbacks()
        {
            throw new NotImplementedException();
        }
        
        public object FindByID(int id)
        {
            return null;
        }
        public object FillAll()
        {
            CustomerAccess.FillDictionary();
            return CustomerAccess.GetData();
        }

        public void Update(IList<int> list)
        {
            CustomerAccess.Update();
        }

        #endregion

        public object GetGlossaryByName(string name)
        {
            return CustomerAccess.GetByEntityName(name);
        }
        public void SaveGlossary(string nameGlossary)
        {
            CustomerAccess.SaveEntity(nameGlossary);
        }

        public object FindBy(string criteria, params int[] value)
        {
            CustomRepository<int> repo = new CustomRepository<int>();
            repo.FindByGlossary(value[0], criteria);
            return CustomerAccess.GetData();
        }
    }
}

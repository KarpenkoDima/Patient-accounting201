﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using BAL.DataTables;
using SOPB.Accounting.DAL.LoadData;
using SOPB.Accounting.DAL.TableAdapters;
using SOPB.Accounting.DAL.TableAdapters.CustomerTableAdapter;
using SOPB.Accounting.DAL.TableAdapters.Glossary;

namespace BAL.AccessData
{
    
    public class CustomerAccess
    {
        private static readonly Tables _tables = new Tables();

        private static string whereClasure = String.Empty;
        private static IConvertible value;
        private static string dbType=String.Empty;
        public static Dictionary<string, string> ParametersForUsp = new Dictionary<string, string>()
        { {Utilites.QueryCriteria.Bithday, "@BirthOfDay:;@BirthOfDayEnd:;@predicate:10"},
            {Utilites.QueryCriteria.Address, "@City:100;@NameStreet:100"},
            {Utilites.QueryCriteria.LastName, "@LastName:100"}
        };
       

        public static DbType DbTypeConversionKey(string fullName)
        {

            Dictionary<string, DbType> conversionKey = new Dictionary<string, DbType>
            { {"System.Int64", DbType.Int64},
                {"System.Byte[]",DbType.Binary},
                {"System.Boolean",DbType.Boolean},
                {"System.String",DbType.String},
                {"System.DateTime",DbType.DateTime},
                {"System.Decimal",DbType.Decimal},
                {"System.Double",DbType.Double},
                {"System.Int32", DbType.Int32},
                {"System.Int16", DbType.Int16},
                {"System.Byte", DbType.Byte},
                {"System.Guid", DbType.Guid},
                {"System.Object", DbType.Object},
                {"System.Single", DbType.Single},
                {"System.Char", DbType.AnsiStringFixedLength}};
            if (conversionKey.ContainsKey(fullName))
            {
                return conversionKey[fullName];
            }
            return DbType.Object;
        }
        public static SqlDbType SqlTypeConversionKey(string fullName)
        {

            var conversionKey = new Dictionary<string, SqlDbType>
            { {"System.Int64", SqlDbType.BigInt},
                {"System.Byte[]",SqlDbType.Binary},
                {"System.Boolean",SqlDbType.Bit},
                {"System.String",SqlDbType.VarChar},
                {"System.DateTime",SqlDbType.DateTime},
                {"System.Decimal",SqlDbType.Decimal},
                {"System.Double",SqlDbType.Float},
                {"System.Int32", SqlDbType.Int},
                {"System.Int16", SqlDbType.SmallInt},
                {"System.Byte", SqlDbType.TinyInt},
                {"System.Guid", SqlDbType.UniqueIdentifier},
                {"System.Object", SqlDbType.Variant},
                {"System.Single", SqlDbType.Real},
                {"System.Char", SqlDbType.NChar}};
            if (conversionKey.ContainsKey(fullName))
            {
                return conversionKey[fullName];
            }
            return SqlDbType.Variant;
        }
        static CustomerAccess()
        {
            FillDictionary();
        }
        public static object GetData()
        {
            return _tables.DispancerDataSet;
        }
        public static void FillDictionary()
        {
            ClearData();
            TransactionWork transactionWork = null;
            try
            {
                using (transactionWork = (TransactionWork) TransactionFactory.Create())
                {
                    transactionWork.ReadData(_tables.DispancerDataSet.Tables["ApppTpr"]);
                    transactionWork.ReadData(_tables.DispancerDataSet.Tables["Gender"]);
                    transactionWork.ReadData(_tables.DispancerDataSet.Tables["AdminDivision"]);
                    transactionWork.ReadData(_tables.DispancerDataSet.Tables["TypeStreet"]);
                    transactionWork.ReadData(_tables.DispancerDataSet.Tables["ChiperRecept"]);
                    transactionWork.ReadData(_tables.DispancerDataSet.Tables["BenefitsCategory"]);
                    transactionWork.ReadData(_tables.DispancerDataSet.Tables["DisabilityGroup"]);
                    transactionWork.ReadData(_tables.DispancerDataSet.Tables["Land"]);
                    transactionWork.ReadData(_tables.DispancerDataSet.Tables["RegisterType"]);
                    transactionWork.ReadData(_tables.DispancerDataSet.Tables["WhyDeRegister"]);
                    transactionWork.Commit();
                }
            }
            catch (Exception)
            {
                transactionWork?.Rollback();
                throw;
            }
        }


        public static void FillCustomerData()
        {
            ClearData();
            TransactionWork transactionWork = null;
            try
            {
                using (transactionWork = (TransactionWork) TransactionFactory.Create())
                {
                    transactionWork.ReadData(_tables.DispancerDataSet.Tables["Customer"]);
                    transactionWork.ReadData(_tables.DispancerDataSet.Tables["Address"]);
                    transactionWork.ReadData(_tables.DispancerDataSet.Tables["Invalid"]);
                    transactionWork.ReadData(_tables.DispancerDataSet.Tables["InvalidBenefitsCategory"]);
                    transactionWork.ReadData(_tables.DispancerDataSet.Tables["Register"]);
                    transactionWork.Commit();
                }
                whereClasure = String.Empty;
                value = null;
                dbType = string.Empty;
            }
            catch (Exception)
            {
                transactionWork?.Rollback();
                throw;
            }
        }

        internal static void Vapidated()
        {
            var err = from customer in _tables.CustomerDataTable.AsEnumerable()
                      where customer.Field<DateTime?>("Birthday").HasValue && customer.Field<DateTime?>("Birthday") >= DateTime.Now
                      select new
                      {
                          CustomerID = customer.Field<int>("CustomerID"),
                          LastName = customer.Field<string>("LastName"),
                          FirstName = customer.Field<string>("FirstName"),
                          MiddleName = customer.Field<string>("MiddleName"),
                          Error = "Дата рождения выходит за диапазон"
                      };
            _tables.ErrorDataTable.Clear();
            _tables.ErrorDataTable.Dispose();
            foreach (var item in err)
            {
                _tables.ErrorDataTable.Rows.Add(item.CustomerID, item.LastName + " " + item.FirstName, item.Error);
            }

            var err2 = from customer in _tables.RegisterDataTable.AsEnumerable()
                       where customer.Field<DateTime?>("FirstRegister").HasValue && customer.GetParentRow("FK_Register_Customer_CustomerID").Field<DateTime?>("Birthday").HasValue
                       && customer.Field<DateTime?>("FirstRegister") <= customer.GetParentRow("FK_Register_Customer_CustomerID").Field<DateTime?>("Birthday")
                       select new
                       {
                           CustomerID = customer.Field<int>("CustomerID"),
                           LastName = customer.GetParentRow("FK_Register_Customer_CustomerID").Field<string>("LastName"),
                           FirstName = customer.Field<DateTime?>("FirstRegister"),
                           MiddleName = customer.GetParentRow("FK_Register_Customer_CustomerID").Field<DateTime?>("Birthday"),
                           Error = "Дата первого раза взятия на учёт раньше даты рождения"
                       };
            foreach (var item in err2)
            {
                _tables.ErrorDataTable.Rows.Add(item.CustomerID, item.LastName + " " + item.FirstName, item.Error);
            }
            var err3 = from customer in _tables.RegisterDataTable.AsEnumerable()
                       where customer.Field<DateTime?>("FirstDeRegister").HasValue && customer.Field<DateTime?>("FirstRegister").HasValue
                       && customer.Field<DateTime?>("FirstRegister") >= customer.Field<DateTime?>("FirstDeRegister")
                       select new
                       {
                           CustomerID = customer.Field<int>("CustomerID"),
                           LastName = customer.GetParentRow("FK_Register_Customer_CustomerID").Field<string>("LastName"),
                           FirstName = customer.Field<DateTime?>("FirstRegister"),
                           MiddleName = customer.Field<DateTime?>("FirstDeRegister"),
                           Error = "Дата первого раза взятия на учёт позже даты снятия с учёта"
                       };
            foreach (var item in err3)
            {
                _tables.ErrorDataTable.Rows.Add(item.CustomerID, item.LastName + " " + item.FirstName, item.Error);
            }
        }
        internal static void Vapidated(DataRowView row)
        {
            DateTime? firstRegister = (DateTime?)row["FirstRegister"];
            DateTime? firstDeRegister = (DateTime?)row["FirstDeRegister"];
            DateTime? secondRegister = (DateTime?)row["SecondRegister"];
            DateTime? secondDeRegister = (DateTime?)row["SecondDeRegister"];
            int customerId = (int)row["CustomerID"];
           // string lastName = row.Row.GetParentRow("FK_Register_Customer_CustomerID").Field<string>("LastName");
            if (firstRegister.HasValue)
            {
                if(firstDeRegister.HasValue && (firstRegister >= firstDeRegister))
                {
                    _tables.ErrorDataTable.Rows.Add(customerId,
                        firstRegister.Value.ToShortDateString() + " >= " + firstDeRegister.Value.ToShortDateString(),
                        "Дата 1-го взятия на учёт позже даты 1-го снятия с учёта");
                }
                if (secondRegister.HasValue && (firstRegister >= secondRegister))
                {
                    _tables.ErrorDataTable.Rows.Add(
                            customerId,
                            firstRegister.Value.ToShortDateString() + " >= " + secondRegister.Value.ToShortDateString(),
                            "Дата 1-го взятия на учёт позже даты повторного взятия на учёта"
                        );
                }
                if (secondDeRegister.HasValue && (firstRegister >= secondDeRegister))
                {
                    _tables.ErrorDataTable.Rows.Add(customerId,
                        firstRegister.Value.ToShortDateString() + " >= " + secondDeRegister.Value.ToShortDateString(),
                        "Дата 1-го взятия на учёт позже даты повторного снятия с учёта");
                }
            }
            if (firstDeRegister.HasValue)
            {                
                if (secondRegister.HasValue && (firstDeRegister >= secondRegister))
                {
                    _tables.ErrorDataTable.Rows.Add(customerId,
                        firstDeRegister.Value.ToShortDateString() + " >= " + secondRegister.Value.ToShortDateString(),
                        "Дата 1-го снятия с учёта позже даты повторного взятия на учёт");
                }
                if (secondDeRegister.HasValue && (firstDeRegister >= secondDeRegister))
                {
                    _tables.ErrorDataTable.Rows.Add(customerId,
                        firstDeRegister.Value.ToShortDateString() + " >= " + secondDeRegister.Value.ToShortDateString(),
                        "Дата 1-го снятия с учёта позже даты повторного снятия с учёта");
                }
            }
            if (secondRegister.HasValue)
            {               
                if (secondDeRegister.HasValue && (secondRegister >= secondDeRegister))
                {
                    _tables.ErrorDataTable.Rows.Add(customerId,
                        secondRegister.Value.ToShortDateString() + " >= " + secondDeRegister.Value.ToShortDateString(),
                        "Дата повторногого взятия на учёт позже даты повторного снятия с учёта");
                }
            }
        }

        public static void GetCustomersByID(int idx)
        {
            ClearData();
            GetDataByCustomerID(idx);
        }

        private static void GetDataByCustomerID(int id)
        {
            TransactionWork transactionWork = null;

            String storageProcedureName = "uspGetCustomers";
            SqlParameter parameter = new SqlParameter();
            parameter.DbType = DbType.Int32;
            parameter.Size = 4;
            parameter.ParameterName = "@CustomerID";
            parameter.Value = id;
            SqlParameter parameterReg = new SqlParameter();
            parameterReg.DbType = DbType.Int32;
            parameterReg.Size = 4;
            parameterReg.ParameterName = "@CustomerID";
            parameterReg.Value = id;
            SqlParameter parameterInv = new SqlParameter();
            parameterInv.DbType = DbType.Int32;
            parameterInv.Size = 4;
            parameterInv.ParameterName = "@CustomerID";
            parameterInv.Value = id;
            SqlParameter parameterAddr = new SqlParameter();
            parameterAddr.DbType = DbType.Int32;
            parameterAddr.Size = 4;
            parameterAddr.ParameterName = "@CustomerID";
            parameterAddr.Value = id;
            try
            {
                using (transactionWork = (TransactionWork)TransactionFactory.Create())
                {
                    transactionWork.Execute(_tables.CustomerDataTable, storageProcedureName, parameter);
                    transactionWork.Execute(_tables.RegisterDataTable, "uspGetRegisterByCustomerID", parameterReg);
                    transactionWork.Execute(_tables.InvalidDataTable, "uspGetInvalidByCustomerID", parameterInv);
                    transactionWork.Execute(_tables.AddressDataTable, "uspGetAddressByCustomerID", parameterAddr);
                    transactionWork.Commit();
                }
                whereClasure = "where vgc.CustomerID = @param";
                value = id;
                dbType = "@param int";
            }
            catch (Exception)
            {
                transactionWork?.Rollback();
                throw;
            }
        }
       
        public static void GetCustomersByBirthdayBetween(DateTime fromDateTime, DateTime toDateTime)
        {
            ClearData();
            GetDataByCriteria("Birthday", new object[] {fromDateTime, toDateTime, "МЕЖДУ"}, "BETWEEN");
    }

        public static void GetCustomersByBirthOfDay(DateTime fromDateTime, string predicate = "=")
        {
            ClearData();
            GetDataByCriteria("Birthday", new object[] { fromDateTime, DateTime.Now, predicate}, predicate);
        }
        
        public static void GetCustomersByGlossary(string glossaryName, int id)
        {
            ClearData();
            GetDataByGlossary(glossaryName, id);
        }

        private static void GetDataByGlossary(string glossaryName, int id)
        {
            TransactionWork transactionWork = null;
            string storageProcedureName = string.Format("uspGet{0}By{1}", "Customer", glossaryName);
            SqlParameter parameter = new SqlParameter();
            parameter.DbType = DbType.Int32;
            parameter.ParameterName = "@ID";
            parameter.Value = id;
            string storageProcedureNameReg = string.Format("uspGet{0}By{1}", "Register", glossaryName);
            SqlParameter parameterReg = new SqlParameter();
            parameterReg.DbType = DbType.Int32;
            parameterReg.ParameterName = "@ID";
            parameterReg.Value = id;
            string storageProcedureNameInv = string.Format("uspGet{0}By{1}", "Invalid", glossaryName);
            SqlParameter parameterInv = new SqlParameter();
            parameterInv.DbType = DbType.Int32;
            parameterInv.ParameterName = "@ID";
            parameterInv.Value = id;

            string storageProcedureNameInvBenefits = string.Format("uspGet{0}By{1}", "InvalidBenefits", glossaryName);
            SqlParameter parameterInvBenefits = new SqlParameter();
            parameterInvBenefits.DbType = DbType.Int32;
            parameterInvBenefits.ParameterName = "@ID";
            parameterInvBenefits.Value = id;

            string storageProcedureNameAddr = string.Format("uspGet{0}By{1}", "Address", glossaryName);
            SqlParameter parameterAddr = new SqlParameter();
            parameterAddr.DbType = DbType.Int32;
            parameterAddr.ParameterName = "@ID";
            parameterAddr.Value = id;
            try
            {
                using (transactionWork = (TransactionWork)TransactionFactory.Create())
                {
                    transactionWork.Execute(_tables.CustomerDataTable, storageProcedureName, parameter);
                    transactionWork.Execute(_tables.RegisterDataTable, storageProcedureNameReg, parameterReg);
                    transactionWork.Execute(_tables.InvalidDataTable, storageProcedureNameInv, parameterInv);
                    transactionWork.Execute(_tables.InvalidBenefitsDataTable, storageProcedureNameInvBenefits, parameterInvBenefits);
                    transactionWork.Execute(_tables.AddressDataTable, storageProcedureNameAddr, parameterAddr);
                    transactionWork.Commit();
                }
                WhereClasure(glossaryName, new object[]{id}, "=");
                //whereClasure = "where " + glossaryName + "ID = @param";
                //value = id;
                //dbType = "@param int";
            }
            catch (Exception)
            {
                transactionWork?.Rollback();
                throw;
            }
        }

        public static void Update()
        {
            TransactionWork transactionWork = null;
            try
            {
                using (transactionWork = (TransactionWork)TransactionFactory.Create())
                {
                    for (int i = 0; i < _tables.DispancerDataSet.Tables.Count; i++)
                    {
                        if(_tables.DispancerDataSet.Tables[i].TableName != _tables.ErrorDataTable.TableName)
                            transactionWork.UpdateData(_tables.DispancerDataSet.Tables[i]);
                    }
                    _tables.ErrorDataTable.Clear();
                    transactionWork.Commit();
                }
            }
            catch (Exception)
            {
                transactionWork?.Rollback();
                throw;
            }
        }

        private static void ClearData()
        {
            _tables.AddressDataTable.Clear();
            _tables.AddressDataTable.Dispose();
            _tables.InvalidBenefitsDataTable.Clear();
            _tables.InvalidBenefitsDataTable.Dispose();
            _tables.InvalidDataTable.Clear();
            _tables.InvalidDataTable.Dispose();
            _tables.RegisterDataTable.Clear();
            _tables.RegisterDataTable.Dispose();
            _tables.ErrorDataTable.Clear();
            _tables.ErrorDataTable.Dispose();
            _tables.CustomerDataTable.Clear();
            _tables.CustomerDataTable.Dispose();

        }

        public static object GetByEntityName(string name)
        {
            return _tables.DispancerDataSet.Tables[name];
        }

        public static void SaveEntity(string entityName)
        {
            TransactionWork transactionWork = null;
            try
            {
                using (transactionWork = (TransactionWork)TransactionFactory.Create())
                {
                    for (int i = 0; i < _tables.DispancerDataSet.Tables.Count; i++)
                    {
                        if (_tables.DispancerDataSet.Tables[i].TableName == entityName)
                        {
                            transactionWork.UpdateData(_tables.DispancerDataSet.Tables[i]);
                            break;
                        }
                    }

                   transactionWork.Commit();
                }
            }
            catch (Exception)
            {
                transactionWork?.Rollback();
                throw;
            }
        }

        public static void ExportToExcel()
        {
            DataTable table = new DataTable();
            TransactionWork transactionWork = null;
            SqlParameter[] parameters = new SqlParameter[3];

            SqlParameter parameter = new SqlParameter();
            parameter.DbType = DbType.String;
            parameter.IsNullable = true;
            parameter.Size = 100;
            parameter.ParameterName = "@whereClasure";
            if (string.IsNullOrWhiteSpace(whereClasure) || string.IsNullOrEmpty(whereClasure))
            {
                parameter.Value = DBNull.Value;
            }
            else parameter.Value = whereClasure;
            parameters[0] = parameter;

            parameter = new SqlParameter();
            parameter.DbType = DbType.String;
            parameter.ParameterName = "@Param";
            parameter.Size = 100;
            if (value==null)
            {
                parameter.Value = DBNull.Value;
            }
            else parameter.Value = value;
            parameter.IsNullable = true;
            parameters[1] = parameter;

            parameter = new SqlParameter();
            parameter.DbType = DbType.String;        
            parameter.Size = 30;
            parameter.ParameterName = "@ParamType";
            if (string.IsNullOrWhiteSpace(dbType) || string.IsNullOrEmpty(dbType))
            {
                parameter.Value = DBNull.Value;
            }
            else parameter.Value = dbType;
            parameter.IsNullable = true;
            parameters[2] = parameter;

            try
            {
                using (transactionWork = (TransactionWork) TransactionFactory.Create())
                {
                    transactionWork.Execute(table, "uspDynamicQuery", parameters);
                    transactionWork.Commit();
                }
            }
            catch (Exception)
            {
                transactionWork?.Rollback();
                throw;
            }

            FastExportingMethod.ExportToExcel(table,
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "List"));
            FastExportingMethod.GemExportToExcel(table,
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "List2.xls"));
            table.Dispose();
            table = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

       
        private static SqlParameter[] CreateUSP(string criteria, object[] parameters)
        {
            string[] parametersName = ParametersForUsp[criteria].Split(';');
            SqlParameter[] sqlParameters=null;
            if (parametersName.Length == parameters.Length)
            {
                sqlParameters = new SqlParameter[parametersName.Length];
                for (int i = 0; i < parametersName.Length; i++)
                {
                    string[] parameterNameSize = parametersName[i].Split(':');
                    SqlParameter parameter = new SqlParameter();
                    parameter.DbType = DbTypeConversionKey(parameters[i].GetType().FullName);
                    parameter.ParameterName = parameterNameSize[0];
                    if (!string.IsNullOrEmpty(parameterNameSize[1]))
                    {
                        parameter.Size = Convert.ToInt32(parameterNameSize[1]);
                    }
                    parameter.Value = parameters[i];
                    sqlParameters[i] = parameter;
                }
            }

            return sqlParameters;
        }

        public static void GetDataByCriteria(string criteria, object[] parameters, string predicate = "=")
        {
            ClearData();
            GetDataByCriteriaTest(criteria, parameters, predicate);
        }
        private static void GetDataByCriteriaTest(string criteria, object[] parameters, string predicate ="=")
        {
            TransactionWork transactionWork = null;

            SqlParameter[] parameterCustomer = CreateUSP(criteria, parameters);
            SqlParameter[] parameterReg = CreateUSP(criteria, parameters);
            SqlParameter[] parameterInv = CreateUSP(criteria, parameters);
            SqlParameter[] parameterInvBenefits = CreateUSP(criteria, parameters);
            SqlParameter[] parameterAddr = CreateUSP(criteria, parameters);

            try
            {
                using (transactionWork = (TransactionWork)TransactionFactory.Create())
                {
                    transactionWork.Execute(_tables.CustomerDataTable, $"uspGetCustomerBy{criteria}",
                        parameterCustomer);
                    transactionWork.Execute(_tables.RegisterDataTable, $"uspGetRegisterBy{criteria}", parameterReg);
                    transactionWork.Execute(_tables.InvalidDataTable, $"uspGetInvalidBy{criteria}", parameterInv);
                    transactionWork.Execute(_tables.InvalidBenefitsDataTable, $"uspGetInvalidBenefitsBy{criteria}",
                        parameterInvBenefits);
                    transactionWork.Execute(_tables.AddressDataTable, $"uspGetAddressBy{criteria}", parameterAddr);
                    transactionWork.Commit();
                }

                WhereClasure(criteria, parameters, predicate);
            }
            catch (Exception)
            {
                transactionWork?.Rollback();
                throw;
            }
        }

        private static void WhereClasure(string criteria, object[] parameters, string predicate)
        {
            if (parameters.Length == 2 && string.Equals(predicate.ToUpper(), "BETWEEN"))
            {
                whereClasure = $"where {criteria} {predicate} @param1 AND @param2";
                //value = 
            }
            else
            {
                whereClasure = $"where {criteria} " + predicate + " @param";
                value = (IConvertible) parameters[0];
                dbType = "@param " + SqlTypeConversionKey(parameters[0].GetType().FullName);
            }
        }
    }
}

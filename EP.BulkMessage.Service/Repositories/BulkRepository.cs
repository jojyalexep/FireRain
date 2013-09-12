using EP.BulkMessage.Service.Entity;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Service.Repositories
{
    public class BulkRepository
    {
        private static BulkRepository repositoryInstance;

        private BulkRepository() { }

        static readonly object padlock = new object();

        public static BulkRepository Current
        {
            get
            {
                lock (padlock)
                {
                    if (repositoryInstance == null)
                    {
                        repositoryInstance = new BulkRepository();
                    }
                    return repositoryInstance;
                }
            }
        }


        public void InsertBulkSMS(List<Sms> smsList)
        {
            string connectionString = OracleConnectionString;
            try
            {
                DataTable smsTable = GetSmsTable(smsList);
                OracleBulkCopy bulkCopy = new OracleBulkCopy(connectionString);
                bulkCopy.ColumnMappings.Add("Id", "Id");
                bulkCopy.ColumnMappings.Add("ToNumber", "ToNumber");
                bulkCopy.ColumnMappings.Add("Content", "Content");
                bulkCopy.ColumnMappings.Add("StatusId", "StatusId");
                bulkCopy.ColumnMappings.Add("StatusDate", "StatusDate");
                bulkCopy.ColumnMappings.Add("CampaignId", "CampaignId");
                bulkCopy.DestinationTableName = "SMS";
                bulkCopy.WriteToServer(smsTable);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void InsertBulkEmail(List<Email> emailList)
        {
            string connectionString = OracleConnectionString;
            try
            {
                DataTable smsTable = GetEmailTable(emailList);
                OracleBulkCopy bulkCopy = new OracleBulkCopy(connectionString);
                bulkCopy.ColumnMappings.Add("Id", "Id");
                bulkCopy.ColumnMappings.Add("ToAddress", "ToAddress");
                bulkCopy.ColumnMappings.Add("Subject", "Subject");
                bulkCopy.ColumnMappings.Add("StatusId", "StatusId");
                bulkCopy.ColumnMappings.Add("StatusDate", "StatusDate");
                bulkCopy.ColumnMappings.Add("CampaignId", "CampaignId");
                bulkCopy.ColumnMappings.Add("Body", "Body");
                bulkCopy.DestinationTableName = "EMAIL";
                bulkCopy.WriteToServer(smsTable);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void UpdateBulkEmailStatus(List<Email> emailList)
        {
            string connectionString = OracleConnectionString;
            try
            {
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private DataTable GetEmailTable(List<Email> emailList)
        {
            DataTable emailTable = new DataTable();
            emailTable.Columns.Add("Id", typeof(int));
            emailTable.Columns.Add("ToAddress", typeof(string));
            emailTable.Columns.Add("Subject", typeof(string));
            emailTable.Columns.Add("StatusId", typeof(int));
            emailTable.Columns.Add("StatusDate", typeof(DateTime));
            emailTable.Columns.Add("CampaignId", typeof(int));
            emailTable.Columns.Add("Body", typeof(string));
            int count = GetMaxFromTable("EMAIL");
            foreach (var sms in emailList)
            {
                var row = emailTable.NewRow();
                row["Id"] = count;
                row["ToAddress"] = sms.ToAddress;
                row["Subject"] = sms.Subject;
                row["StatusId"] = sms.StatusId;
                row["StatusDate"] = sms.StatusDate;
                row["CampaignId"] = sms.CampaignId;
                row["Body"] = sms.Body;
                emailTable.Rows.Add(row);
                count += 1;

            }
            return emailTable;
        }

        private DataTable GetSmsTable(List<Sms> smsList)
        {
            DataTable smsTable = new DataTable();
            smsTable.Columns.Add("Id", typeof(int));
            smsTable.Columns.Add("ToNumber", typeof(string));
            smsTable.Columns.Add("Content", typeof(string));
            smsTable.Columns.Add("StatusId", typeof(int));
            smsTable.Columns.Add("StatusDate", typeof(DateTime));
            smsTable.Columns.Add("CampaignId", typeof(int));
            int count = GetMaxFromTable("SMS");
            foreach (var sms in smsList)
            {
                var row = smsTable.NewRow();
                row["Id"] = count;
                row["ToNumber"] = sms.ToNumber;
                row["Content"] = sms.Content;
                row["StatusId"] = sms.StatusId;
                row["StatusDate"] = sms.StatusDate;
                row["CampaignId"] = sms.CampaignId;
                smsTable.Rows.Add(row);
                count += 1;

            }
            return smsTable;
        }

        private int GetMaxFromTable(string table)
        {
            int max = 0;
            using (OracleConnection connection = new OracleConnection(OracleConnectionString))
            {
                connection.Open();
                var command = new OracleCommand("SELECT NVL(MAX(BTBL.ID), 0) AS MAX_VAL FROM " + table + " BTBL", connection);
                max = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
            }
            return max + 1;
        }





        private string OracleConnectionString
        {
            get
            {

                if (ConfigurationManager.ConnectionStrings["Oracle"] == null)
                    throw (new NullReferenceException("ConnectionString configuration is missing from you web.config."));

                // connection string not initialized
                string _connectionString = ConfigurationManager.ConnectionStrings["Oracle"].ConnectionString;

                // connection string present in web.config but ie empty
                if (String.IsNullOrEmpty(_connectionString))
                    throw (new NullReferenceException("ConnectionString configuration is missing from you web.config."));

                return _connectionString;

            }
        }
    }
}
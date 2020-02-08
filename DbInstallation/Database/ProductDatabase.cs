﻿using DbInstallation.Interfaces;
using NLog;
using System;
using static DbInstallation.Enums.EnumDbType;
using static DbInstallation.Enums.EnumOperation;

namespace DbInstallation.Database
{
    public class ProductDatabase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private bool _isConnectionDefined;

        public ProductDatabase()
        {
            _isConnectionDefined = false;
        }

        public bool SetConnection(ProductDbType dbType)
        {
            
            if(dbType == ProductDbType.None)
            {
                throw new InvalidOperationException("Invalid Database type option.");
            }
            else
            {
                return Connect(dbType);
            }
        }

        public ProductDbType GetDatabaseType(string dbType)
        {
            Console.Write(Environment.NewLine);
            if (dbType.ToUpper() == "O")
            {
                Logger.Info("Selected Oracle Database");
                Console.Write(Environment.NewLine);
                return ProductDbType.Oracle;
            }
            else if (dbType.ToUpper() == "S")
            {
                Logger.Info("Selected SQL Server Database");
                Console.Write(Environment.NewLine);
                return ProductDbType.SqlServer;
            }
            return ProductDbType.None;
        }

        public OperationType GetOperationType(string operation)
        {
            if (operation.ToUpper() == "I")
            {
                Logger.Info("Selected Install operation.");
                return OperationType.Install;
            }
            else if (operation.ToUpper() == "U")
            {
                Logger.Info("Selected Update operation.");
                return OperationType.Update;
            }
            return OperationType.None;
        }

        public void StartDatabaseOperation(ProductDbType dbType, OperationType operationType)
        {
            if (_isConnectionDefined)
            {
                if (operationType == OperationType.Install)
                {
                    InstallDatabase(dbType);
                }
                else if (operationType == OperationType.Update)
                {
                    UpdateDatabase(dbType);
                }
                else
                {
                    throw new InvalidOperationException("Invalid Database operation.");
                }
            }
            else
            {
                throw new Exception("Database connection was not defined.");
            }
        }

        private void InstallDatabase(ProductDbType dbType)
        {
            if(dbType == ProductDbType.Oracle)
            {
                //_oracleOperationFunctions.Install(); //TODO:
            }
            else if (dbType == ProductDbType.SqlServer)
            {
                //_sqlServerOperationFunctions.CheckDatabaseInstall(); //TODO:
            }
        }

        private void UpdateDatabase(ProductDbType dbType)
        {
            throw new NotImplementedException(); //TODO:
        }

        private bool Connect(ProductDbType dbType)
        {
            DatabaseProperties databaseProperties = RequestDbInputsProperties(dbType);
            IDatabaseFunctions databaseFunctions;
            if (dbType == ProductDbType.Oracle)
            {
                databaseFunctions = new OracleOperationFunctions(databaseProperties);
            }
            else
            {
                databaseFunctions = new SqlServerOperationFunctions(databaseProperties);
            }

            try
            {
                _isConnectionDefined = databaseFunctions.TestConnection();
                return _isConnectionDefined;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                throw ex;
            }
        }

        private DatabaseProperties RequestDbInputsProperties(ProductDbType dbType)
        {
            string dbUser = null;
            string dbPassword = null;

            if (dbType == ProductDbType.SqlServer)
            {
                bool isTrustedConnection = RequestSqlServerTrustedConnection();
                Console.WriteLine("Enter Sql Server Database Name:");
                string databaseName = Console.ReadLine();
                
                if (!isTrustedConnection)
                {
                    dbUser = RequestDatabaseUser();
                    dbPassword = RequestDatabasePassword();
                }

                Console.WriteLine("Enter the Server name:");
                string serverName = Console.ReadLine();
                return new DatabaseProperties(dbUser, dbPassword, serverName, databaseName, isTrustedConnection);
            }
            else if(dbType == ProductDbType.Oracle)
            {
                dbUser = RequestDatabaseUser();
                dbPassword = RequestDatabasePassword();

                Console.WriteLine("Enter data TABLESPACE:");
                string tablespaceData = Console.ReadLine().ToUpper();
                Console.WriteLine("Enter index TABLESPACE:");
                string tablespaceIndex = Console.ReadLine().ToUpper();
                Console.WriteLine("Enter the TNS connection string:");
                string tnsOrServerConnection = Console.ReadLine();
                return new DatabaseProperties(dbUser, dbPassword, tnsOrServerConnection, tablespaceData, tablespaceIndex);
            }
            else
            {
                throw new Exception("Database type is not defined.");
            }
        }

        private string RequestDatabaseUser()
        {
            Console.WriteLine("Enter database user:");
            return Console.ReadLine();
        }

        private string RequestDatabasePassword()
        {
            Console.WriteLine("Enter the password:");
            return Console.ReadLine();
        }

        private bool RequestSqlServerTrustedConnection()
        {
            Console.WriteLine("Choose authenticationi type:");
            Console.WriteLine("(W) Windows");
            Console.WriteLine("(U) Sql Server User");
            string authType = Console.ReadLine();

            if (authType.ToUpper() != "W" && authType.ToUpper() != "U")
            {
                throw new Exception($@"Invalid Sql Server type connection '{authType}'.");
            }
            return authType.ToUpper() == "W";
        }
    }
}
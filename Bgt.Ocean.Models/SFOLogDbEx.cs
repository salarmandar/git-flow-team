// --------------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="OceanEntitiesEx.cs" company="Aware Technology Solutions" by="Michael Bailey">
//   Copyright (c) 2018-2019.  All rights reserved.
// </copyright>
// <summary>
//   Defines the partial OceanEntities type to save changes the data into the SQL database (BulkCopy). 
// </summary>
// <tested>
//   v7.1   Tested on Tueday, May 21, 2019
//   v7.1.1 Tested on Friday, May 24, 2019
//   v7.1.2 27 May 2019
//   v7.1.3 28 May 2019
//   v7.1.4 29 May 2019
// </tested>
// --------------------------------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Bgt.Ocean.Models
{
    public partial class SFOLogDbEntities : DbContext
    {
        protected SqlBulkCopyOptions options = SqlBulkCopyOptions.FireTriggers;

        private string schema = "dbo";

        private MetadataWorkspace workspace = null;

        private List<AssociationType> fkList = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initSchema"></param>
        public SFOLogDbEntities(bool initSchema)
            : this()
        {
            workspace = ((IObjectContextAdapter)this).ObjectContext.MetadataWorkspace;

            fkList = workspace.GetItems<AssociationType>(DataSpace.CSpace).Where(a => a.IsForeignKey).ToList();

            if (initSchema)
            {
                SetEdmFunctionSchema();
            }
        }

        /// <summary>
        /// Bulk save changes
        /// </summary>
        /// <param name="options"></param>
        public int BulkSaveChanges(SqlBulkCopyOptions options = SqlBulkCopyOptions.FireTriggers)
        {
            this.options = options;

            ChangeTracker.DetectChanges();

            try
            {
                if (Configuration.ValidateOnSaveEnabled)
                {
                    var validationResults = GetValidationErrors();
                    if (validationResults.Any())
                    {
                        throw new DbEntityValidationException("DbEntityValidationException_ValidationFailed", validationResults);
                    }
                }

                var shouldDetectChanges = Configuration.AutoDetectChangesEnabled && !Configuration.ValidateOnSaveEnabled;
                var saveOptions = SaveOptions.AcceptAllChangesAfterSave |
                                  (shouldDetectChanges ? SaveOptions.DetectChangesBeforeSave : 0);

                PrepareToSaveChanges(saveOptions);

                return BulkSaveChangesToStore();
            }
            catch (UpdateException ex)
            {
                throw ex;
            }

        }

        private void PrepareToSaveChanges(SaveOptions options)
        {
            var internalContext = this.GetType().BaseType.GetField("_internalContext", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);

            if (internalContext != null)
            {
                var objectContext = internalContext.GetType().GetProperty("ObjectContext").GetValue(internalContext);

                if (objectContext != null)
                {
                    MethodInfo method = objectContext.GetType().GetMethod("PrepareToSaveChanges", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (method != null)
                    {
                        object[] parameters = new object[] { options };

                        method.Invoke(objectContext, parameters);
                    }
                    else
                    {
                        throw new Exception("Cannot get Method PrepareToSaveChanges");
                    }
                }
                else
                {
                    throw new Exception("Cannot get ObjectContext");
                }

            }
            else
            {
                throw new Exception("Cannot get InternalContext");
            }

        }

        private int BulkSaveChangesToStore()
        {
            int entriesAffected = ChangeTracker.Entries().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted).Count();

            var addedEntities = ChangeTracker.Entries().Where(x => x.State == EntityState.Added);
            var modifiedEntities = ChangeTracker.Entries().Where(x => x.State == EntityState.Modified);
            var deletedEntities = ChangeTracker.Entries().Where(x => x.State == EntityState.Deleted);

            try
            {
                // Bulk Update Entities
                BulkAddEntities();

                // Bulk Update Entities
                BulkUpdateEntities();

                // Bulk Delete Entities
                BulkDeleteEntities();
            }
            catch (Exception ex)
            {
                // Rollback Change Tracker
                addedEntities.Where(e => e.State == EntityState.Unchanged).ToList().ForEach(c => c.State = EntityState.Added);
                modifiedEntities.Where(e => e.State == EntityState.Unchanged).ToList().ForEach(c => c.State = EntityState.Modified);
                deletedEntities.Where(e => e.State == EntityState.Unchanged).ToList().ForEach(c => c.State = EntityState.Deleted);

                throw ex;
            }

            return entriesAffected;
        }

        /// <summary>
        /// Bulk add entities
        /// </summary>
        public void BulkAddEntities()
        {
            // Query Added Entries
            var entities = ChangeTracker.Entries().Where(x => x.State == EntityState.Added);

            try
            {
                foreach (Type type in entities.Select(o => o.Entity.GetType()).ToList().Distinct())
                {
                    AddEntity(type, entities);
                }
            }
            catch (Exception ex)
            {
                // Rollback State Flag
                entities.ToList()
                    .ForEach(c => c.State = EntityState.Added);

                throw ex;
            }
        }

        /// <summary>
        /// Add entity
        /// </summary>
        /// <param name="type"></param>
        /// <param name="addedEntities"></param>
        protected void AddEntity(Type type, IEnumerable<DbEntityEntry> addedEntities)
        {
            IEnumerable<DbEntityEntry> entities = addedEntities.Where(c => c.Entity.GetType() == type && c.State == EntityState.Added);

            if (entities.Any())
            {
                // Get Column Attributes and validate Primary Key
                DbEntityEntry modelEntity = entities.First();

                // Add ForiegnKey Entities before current Entity
                foreach (var fkType in GetForeignKeyAttributes(modelEntity).Select(k => k.Type))
                {
                    foreach (var entry in addedEntities.Where(e => e.Entity.GetType() == fkType).Distinct())
                    {
                        AddEntity(entry.Entity.GetType(), addedEntities);
                    }
                }

                // Add each Row to DataTable and Bulk Insert DataTable to SQL Server
                List<ColumnAttribute> columnAttributes = GetColumnAttributes(modelEntity);

                DataTable table = GetDataTable(columnAttributes);

                foreach (DbEntityEntry entry in entities)
                {
                    BulkAddRow(table, columnAttributes, entry);
                }

                BulkInsert(GetTableName(type), columnAttributes, table);

                table = null;
                columnAttributes = null;

                // Changes Added => Unchanged
                entities.Where(e => e.Entity.GetType() == type).ToList()
                    .ForEach(c => c.State = EntityState.Unchanged);
            }
        }

        /// <summary>
        /// Bulk update entities
        /// </summary>
        public void BulkUpdateEntities()
        {
            // Check for Changes in Repository
            ChangeTracker.DetectChanges();

            var entities = ChangeTracker.Entries().Where(x => x.State == EntityState.Modified);

            if (entities.Any())
            {
                // Derive List of Changes Entities
                try
                {
                    foreach (Type type in entities.Select(o => o.Entity.GetType()).ToList().Distinct())
                    {
                        List<ColumnAttribute> columnAttributes = null;

                        DataTable table = null;

                        foreach (var entry in entities.Where(e => e.Entity.GetType() == type))
                        {
                            if (null == columnAttributes)
                            {
                                columnAttributes = GetColumnAttributes(entities.Where(e => e.Entity.GetType() == type).First());
                                table = GetDataTable(columnAttributes);
                            }
                            BulkAddRow(table, columnAttributes, entry);
                        }

                        BulkUpdate(GetTableName(type), columnAttributes, table);
                    }

                    // Set Modified to Unchanged State 
                    entities.ToList()
                        .ForEach(c => c.State = EntityState.Unchanged);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Bulk delete entities
        /// </summary>
        public void BulkDeleteEntities()
        {
            // Query Added Entries
            var entities = ChangeTracker.Entries().Where(x => x.State == EntityState.Deleted);

            try
            {
                foreach (Type type in entities.Select(o => o.Entity.GetType()).ToList().Distinct())
                {
                    DeleteEntity(type, entities);
                }

            }
            catch (Exception ex)
            {
                // Rollback State Flag
                entities.ToList()
                    .ForEach(c => c.State = EntityState.Deleted);

                throw ex;
            }
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="type"></param>
        /// <param name="deletedEntities"></param>
        protected void DeleteEntity(Type type, IEnumerable<DbEntityEntry> deletedEntities)
        {
            string tableName = GetTableName(type);

            IEnumerable<DbEntityEntry> entities = deletedEntities
                .Where(c => ObjectContext.GetObjectType(c.Entity.GetType()) == ObjectContext.GetObjectType(type) && c.State == EntityState.Deleted);

            if (entities.Any())
            {
                // Get Column Attributes and validate Primary Key
                DbEntityEntry modelEntity = entities.First();

                // Add ForiegnKey Entities before current Entity
                foreach (ColumnAttribute entry in GetNavigationProperties(modelEntity))
                {
                    DeleteEntity(entry.Type, deletedEntities);
                }

                // Add each Row to DataTable and Bulk Insert DataTable to SQL Server
                List<ColumnAttribute> pkColumnAttributes = GetPrimaryKeyAttributes(entities.Where(o => ObjectContext.GetObjectType(o.Entity.GetType()) == ObjectContext.GetObjectType(type)).First());

                DataTable table = GetDataTable(pkColumnAttributes); ;

                foreach (DbEntityEntry entry in entities)
                {
                    BulkDeleteAddRow(table, pkColumnAttributes, entry.Entity);
                }

                BulkDelete(GetTableName(type), pkColumnAttributes, table);

                table = null;

                // Remove Deleted State Entries
                entities.ToList().ForEach(e => e.State = EntityState.Unchanged);
            }
        }

        /// <summary>
        /// Get data table
        /// </summary>
        /// <param name="columnAttributes"></param>
        /// <returns></returns>
        protected DataTable GetDataTable(List<ColumnAttribute> columnAttributes)
        {
            DataTable table = new DataTable();

            foreach (ColumnAttribute columnAttribute in columnAttributes)
            {
                table.Columns.Add(columnAttribute.Name, columnAttribute.Type);
            }

            return table;
        }

        /// <summary>
        /// Get delete data table
        /// </summary>
        /// <param name="columnAttributes"></param>
        /// <returns></returns>
        protected DataTable GetDeleteDataTable(List<ColumnAttribute> columnAttributes)
        {
            DataTable table = new DataTable();

            foreach (ColumnAttribute columnAttribute in columnAttributes.Where(e => e.PrimaryKey == true))
            {
                table.Columns.Add(columnAttribute.Name, columnAttribute.Type);
            }

            return table;
        }

        /// <summary>
        /// Bulk map columns
        /// </summary>
        /// <param name="bulk"></param>
        /// <param name="columnAttributes"></param>
        protected void BulkMapColumns(SqlBulkCopy bulk, List<ColumnAttribute> columnAttributes)
        {
            foreach (ColumnAttribute columnAttribute in columnAttributes)
            {
                bulk.ColumnMappings.Add(columnAttribute.Name, columnAttribute.Name);
            }
        }

        /// <summary>
        /// Bulk add row
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnAttributes"></param>
        /// <param name="entry"></param>
        protected void BulkAddRow(DataTable table, List<ColumnAttribute> columnAttributes, DbEntityEntry entry)
        {
            DataRow row = table.NewRow();

            foreach (var columnAttribute in columnAttributes)
            {
                row[columnAttribute.Name] = entry.CurrentValues[columnAttribute.Name] ?? DBNull.Value;
            }

            table.Rows.Add(row);
        }

        /// <summary>
        /// Bulk delete add row
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnAttributes"></param>
        /// <param name="entity"></param>
        protected void BulkDeleteAddRow(DataTable table, List<ColumnAttribute> columnAttributes, object entity)
        {
            DataRow newRow = table.NewRow();

            Type entityType = ObjectContext.GetObjectType(entity.GetType());

            if (entityType.BaseType != null && entityType.Namespace.Equals("System.Data.Entity.DynamicProxies"))
            {
                entityType = entityType.BaseType;
            }

            PropertyInfo info = null;

            for (int col = 0; col < columnAttributes.Count; col++)
            {
                if (columnAttributes[col].PrimaryKey)
                {
                    if ((info = entityType.GetProperty(columnAttributes[col].Name)) != null)
                    {
                        newRow[col] = info.GetValue(entity, null) ?? DBNull.Value;
                    }
                }
            }

            table.Rows.Add(newRow);
        }

        /// <summary>
        /// Bulk insert
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnAttributes"></param>
        /// <param name="table"></param>
        protected void BulkInsert(string tableName, List<ColumnAttribute> columnAttributes, DataTable table)
        {
            bool transactionScope = !(System.Transactions.Transaction.Current == null);

            SqlConnection conn = Database.Connection as SqlConnection;
            SqlTransaction transaction = null;

            if (transactionScope)
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
            }
            else
            {
                if (Database.Connection.State != ConnectionState.Open)
                {
                    throw new Exception("Database connection in invalid state");
                }
                if (Database.CurrentTransaction == null)
                {
                    throw new Exception("Unable to enlist in Transaction");
                }

                transaction = Database.CurrentTransaction.UnderlyingTransaction as SqlTransaction;
            }

            // BulkCopy the data in the DataTable to the target table
            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(conn, options, transaction))
            {
                sqlBulkCopy.DestinationTableName = String.Format("[{0}]", tableName);
                BulkMapColumns(sqlBulkCopy, columnAttributes);
                sqlBulkCopy.WriteToServer(table);
            }
        }

        /// <summary>
        /// Bulk update
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnAttributes"></param>
        /// <param name="table"></param>
        protected void BulkUpdate(string tableName, List<ColumnAttribute> columnAttributes, DataTable table)
        {
            bool transactionScope = !(System.Transactions.Transaction.Current == null);

            SqlConnection conn = Database.Connection as SqlConnection;
            SqlTransaction transaction = null;

            if (transactionScope)
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
            }
            else
            {
                if (conn.State != ConnectionState.Open)
                {
                    throw new Exception("Database connection in invalid state");
                }

                if (Database.CurrentTransaction == null)
                {
                    throw new Exception("Unable to enlist in Transaction");
                }

                transaction = Database.CurrentTransaction.UnderlyingTransaction as SqlTransaction;
            }

            // Build Column List for UPDATE Statement
            StringBuilder sbCols = new StringBuilder();

            foreach (ColumnAttribute columnAttribute in columnAttributes)
            {
                sbCols.Append(string.Format(" {0},", columnAttribute.Name));
            }

            // Remove Trailing ','
            sbCols.Replace(",", " ", sbCols.Length - 1, 1);

            // Create temp table
            SqlCommand cmd = new SqlCommand(String.Format("SELECT top 0 {0} INTO #{1} FROM {2}", sbCols.ToString(), tableName.Replace(' ', '_'), tableName), conn);

            // Create temp table
            //SqlCommand cmd = new SqlCommand(String.Format("SELECT top 0 * INTO #{0} FROM {1}", tableName.Replace(' ', '_'), tableName), conn);

            if (!transactionScope)
            {
                cmd.Transaction = transaction;
            }

            cmd.ExecuteNonQuery();

            // BulkCopy the data in the DataTable to the temp table
            using (var bulk = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, transaction))
            {
                bulk.BulkCopyTimeout = 0;
                bulk.DestinationTableName = String.Format("#{0}", tableName.Replace(' ', '_'));
                BulkMapColumns(bulk, columnAttributes);
                bulk.WriteToServer(table);
            }

            // Build Column List for UPDATE Statement
            StringBuilder sbColumns = new StringBuilder();
            String primaryKey = string.Empty;

            foreach (ColumnAttribute columnAttribute in columnAttributes)
            {
                if (columnAttribute.PrimaryKey)
                {
                    primaryKey = columnAttribute.Name;
                }
                else
                {
                    sbColumns.Append(string.Format(" t1.{0} = t2.{1},", columnAttribute.Name, columnAttribute.Name));
                }
            }

            // Remove Trailing ','
            sbColumns.Replace(",", " ", sbColumns.Length - 1, 1);

            cmd.CommandText = String.Format("UPDATE t1 SET {0} FROM {1} AS t1 INNER JOIN #{2} AS t2 ON t1.{3} = t2.{4}", sbColumns.ToString(), tableName, tableName.Replace(' ', '_'), primaryKey, primaryKey);

            if (!transactionScope)
            {
                cmd.Transaction = transaction;
            }

            cmd.ExecuteNonQuery();

            // Clean up the temp table
            cmd.CommandText = string.Format("DROP TABLE #{0}", tableName.Replace(' ', '_'));

            if (!transactionScope)
            {
                cmd.Transaction = transaction;
            }

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Bulk delete
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnAttributes"></param>
        /// <param name="table"></param>
        protected void BulkDelete(string tableName, List<ColumnAttribute> columnAttributes, DataTable table)
        {
            bool transactionScope = !(System.Transactions.Transaction.Current == null);
            SqlConnection conn = Database.Connection as SqlConnection;
            SqlTransaction transaction = null;

            if (transactionScope)
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
            }
            else
            {
                if (conn.State != ConnectionState.Open)
                {
                    throw new Exception("Database connection in invalid state");
                }

                if (Database.CurrentTransaction == null)
                {
                    throw new Exception("Unable to enlist in Transaction");
                }

                transaction = Database.CurrentTransaction.UnderlyingTransaction as SqlTransaction;
            }

            // Create Temporary Table
            if (columnAttributes.Where(c => c.PrimaryKey == true).Count() != 1)
            {
                throw new Exception("Unable to establish PrimaryKey");
            }

            string primaryKey = columnAttributes.Where(c => c.PrimaryKey == true).First().Name;
            string pkType = string.Empty;

            if (columnAttributes[0].Type == typeof(System.Guid))
            {
                pkType = "uniqueidentifier";
            }
            else if (columnAttributes[0].Type == typeof(System.Int32))
            {
                pkType = "int";
            }
            else if (columnAttributes[0].Type == typeof(System.String))
            {
                pkType = "varchar(max)";
            }
            else
            {
                throw new Exception("Unable to establish PrimaryKey Type");
            }

            SqlCommand cmd = new SqlCommand(String.Format("create table #{0} ( {1} {2} )", tableName.Replace(' ', '_'), primaryKey, pkType), conn);

            if (!transactionScope)
            {
                cmd.Transaction = transaction;
            }

            cmd.ExecuteNonQuery();

            // BulkCopy the data in the DataTable to the temp table
            using (var bulk = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default | SqlBulkCopyOptions.KeepIdentity, transaction))
            {
                bulk.BulkCopyTimeout = 0;
                bulk.DestinationTableName = String.Format("#{0}", tableName.Replace(' ', '_'));
                bulk.WriteToServer(table);
            }

            // Build Delete Statement
            cmd.CommandText = string.Format("DELETE t FROM {0} t INNER JOIN #{1} v ON t.{2} = v.{3}", tableName,
                                                                                                      tableName.Replace(' ', '_'),
                                                                                                      primaryKey,
                                                                                                      primaryKey);

            cmd.Transaction = transaction;
            cmd.ExecuteNonQuery();

            // Clean up the temp table
            cmd.CommandText = string.Format("DROP TABLE #{0}", tableName.Replace(' ', '_'));

            if (!transactionScope)
            {
                cmd.Transaction = transaction;
            }

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Get column attributes
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        protected List<ColumnAttribute> GetColumnAttributes(DbEntityEntry entry)
        {
            string tableName = GetTableName(entry.Entity.GetType());

            List<ColumnAttribute> columnAttributes = new List<ColumnAttribute>();

            // Get Primary Key Collection
            EntitySetBase setBase = ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager
                .GetObjectStateEntry(entry.Entity).EntitySet;

            string keyName = setBase.ElementType.KeyMembers
                .Select(k => k.Name).FirstOrDefault();

            foreach (string propertyName in entry.CurrentValues.PropertyNames)
            {
                PropertyInfo propertyInfo = entry.Entity.GetType().GetProperty(propertyName);
                Type propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                Boolean pk = false;
                Boolean fk = false;
                Boolean identity = false;

                // Derive PrimaryKey from DbEntity (FluentAPI)
                if (propertyInfo.Name == keyName)
                {
                    pk = true;
                }

                // Derive PrimaryKey from Model (Convention)
                if (propertyInfo.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
                    propertyInfo.Name.Equals("Guid", StringComparison.OrdinalIgnoreCase) ||
                    propertyInfo.Name.Equals(tableName + "Id", StringComparison.OrdinalIgnoreCase))
                {
                    pk = true;
                }

                // Derive PrimaryKey from Model (Annotations)
                foreach (var attribute in propertyInfo.GetCustomAttributes())
                {
                    if (attribute.GetType() == typeof(System.ComponentModel.DataAnnotations.KeyAttribute))
                    {
                        pk = true;
                    }
                    if (attribute.GetType() == typeof(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedAttribute))
                    {
                        identity = true;
                    }
                    if (attribute.GetType() == typeof(System.ComponentModel.DataAnnotations.Schema.ForeignKeyAttribute))
                    {
                        fk = true;
                    }
                }

                columnAttributes.Add(new ColumnAttribute { Name = propertyInfo.Name, Type = propertyType, PrimaryKey = pk, ForeignKey = fk, Identity = identity });
            }

            if (columnAttributes.Where(t => t.PrimaryKey == true).Count() != 1)
            {
                throw new Exception("Unable to derive PrimaryKey for Table: " + entry.Entity.GetType().Name);
            }

            return columnAttributes;
        }

        /// <summary>
        /// Get foreign key attributes
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        protected List<ColumnAttribute> GetForeignKeyAttributes(DbEntityEntry entry)
        {
            Type entityType = entry.Entity.GetType();
            string tableName = GetTableName(entityType);

            if (entityType.BaseType != null && entityType.Namespace.Equals("System.Data.Entity.DynamicProxies"))
            {
                entityType = entityType.BaseType;
            }

            List<ColumnAttribute> columnAttributes = new List<ColumnAttribute>();

            if (null != fkList)
            {
                foreach (var fk in fkList.Where(k => k.ReferentialConstraints[0].ToRole.Name.Equals(tableName)))
                {
                    var type = Assembly.GetExecutingAssembly()
                            .GetTypes()
                            .FirstOrDefault(t => t.Name == fk.ReferentialConstraints[0].FromRole.Name);

                    columnAttributes.Add(new ColumnAttribute { Name = fk.ReferentialConstraints[0].FromRole.Name, Type = type, PrimaryKey = false, ForeignKey = true, Identity = false });
                }
            }

            // From Model (Annotations)
            foreach (PropertyInfo propertyInfo in entityType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                foreach (var attribute in propertyInfo.GetCustomAttributes())
                {
                    if (attribute.GetType() == typeof(System.ComponentModel.DataAnnotations.Schema.ForeignKeyAttribute))
                    {
                        columnAttributes.Add(new ColumnAttribute { Name = ((ForeignKeyAttribute)attribute).Name, Type = propertyInfo.GetType(), PrimaryKey = false, ForeignKey = true, Identity = false });
                    }
                }
            }

            return columnAttributes;
        }

        /// <summary>
        /// Get foreign key attributes
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        protected List<ColumnAttribute> GetNavigationProperties(DbEntityEntry entry)
        {
            Type entityType = entry.Entity.GetType();
            string tableName = GetTableName(entityType);

            if (entityType.BaseType != null && entityType.Namespace.Equals("System.Data.Entity.DynamicProxies"))
            {
                entityType = entityType.BaseType;
            }

            List<ColumnAttribute> columnAttributes = new List<ColumnAttribute>();

            //  public virtual ICollection<TblBankCleanOutJobDelivery_NonbarCodeTemp> TblBankCleanOutJobDelivery_NonbarCodeTemp  

            // From Model 
            foreach (PropertyInfo pi in entityType.GetProperties())
            {
                Type t = pi.PropertyType;

                if (t.IsGenericType)
                {
                    //  t.GetGenericTypeDefinition()
                    Type te = t.GetGenericTypeDefinition();

                    bool implementICollection = te.GetType().GetInterfaces()
                            .Any(x => x.IsGenericType &&
                            x.GetGenericTypeDefinition() == typeof(ICollection<>));

                    if (te == typeof(ICollection<>))
                    {
                        Type elementType = t.GetGenericArguments()[0];

                        columnAttributes.Add(new ColumnAttribute { Name = pi.Name, Type = elementType, PrimaryKey = false, ForeignKey = true, Identity = false });
                    }
                }
            }

            return columnAttributes;
        }

        /// <summary>
        /// Get primary key attributes
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        protected List<ColumnAttribute> GetPrimaryKeyAttributes(DbEntityEntry entry)
        {
            string tableName = GetTableName(entry.Entity.GetType());
            List<ColumnAttribute> columnAttributes = new List<ColumnAttribute>();
            DbPropertyValues propertyValues = null;

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                propertyValues = entry.CurrentValues;
            }
            else
            {
                propertyValues = entry.GetDatabaseValues();
            }

            // Get Primary Key Collection
            EntitySetBase setBase = ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager
                .GetObjectStateEntry(entry.Entity).EntitySet;

            string keyName = setBase.ElementType.KeyMembers.Select(k => k.Name).FirstOrDefault();

            foreach (string propertyName in propertyValues.PropertyNames)
            {
                PropertyInfo propertyInfo = entry.Entity.GetType().GetProperty(propertyName);
                Type propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                Boolean pk = false;
                Boolean fk = false;
                Boolean identity = false;

                // Derive PrimaryKey from DbEntity (FluentAPI)
                if (propertyInfo.Name == keyName)
                {
                    pk = true;
                }

                // Derive PrimaryKey from Model (Convention)
                if (propertyInfo.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
                    propertyInfo.Name.Equals("Guid", StringComparison.OrdinalIgnoreCase) ||
                    propertyInfo.Name.Equals(tableName + "Id", StringComparison.OrdinalIgnoreCase))
                {
                    pk = true;
                }

                // Derive PrimaryKey from Model (Annotations)
                foreach (var attribute in propertyInfo.GetCustomAttributes())
                {
                    if (attribute.GetType() == typeof(System.ComponentModel.DataAnnotations.KeyAttribute))
                    {
                        pk = true;
                    }
                    if (attribute.GetType() == typeof(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedAttribute))
                    {
                        identity = true;
                    }
                    if (attribute.GetType() == typeof(System.ComponentModel.DataAnnotations.Schema.ForeignKeyAttribute))
                    {
                        fk = true;
                    }
                }

                if (pk)
                {
                    columnAttributes.Add(new ColumnAttribute { Name = propertyInfo.Name, PrimaryKey = true, Type = propertyType, ForeignKey = fk, Identity = identity });
                }
            }

            if (columnAttributes.Count != 1)
            {
                throw new Exception("Unable to derive PrimaryKey for Table: " + entry.Entity.GetType().Name);
            }

            return columnAttributes;
        }

        /// <summary>
        /// Get table name
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected string GetTableName(Type type)
        {
            string tableName = string.Empty;
            Type entType = type;

            if (entType.BaseType != null && entType.Namespace == "System.Data.Entity.DynamicProxies")
            {
                entType = entType.BaseType;
            }

            var metadata = ((IObjectContextAdapter)this).ObjectContext.MetadataWorkspace;
            var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));
            var entityType = metadata
                .GetItems<EntityType>(DataSpace.OSpace)
                .Single(e => objectItemCollection.GetClrType(e) == entType);

            var entitySet = metadata
                .GetItems<EntityContainer>(DataSpace.CSpace)
                .Single()
                .EntitySets
                .Single(s => s.ElementType.Name == entityType.Name);

            var mapping = metadata.GetItems<EntityContainerMapping>(DataSpace.CSSpace)
                .Single()
                .EntitySetMappings
                .Single(s => s.EntitySet == entitySet);

            var table = mapping
                .EntityTypeMappings.Single()
                .Fragments.Single()
                .StoreEntitySet;

            tableName = (string)table.MetadataProperties["Table"].Value ?? table.Name;

            return tableName;
        }

        /// <summary>
        /// Schema
        /// </summary>
        public string Schema
        {
            get
            {
                return schema;
            }
        }

        /// <summary>
        /// Set EDM function schema
        /// </summary>
        private void SetEdmFunctionSchema()
        {
            string staging = ConfigurationManager.AppSettings["EnvSTG"];

            if (!string.IsNullOrEmpty(staging))
            {
                schema = Convert.ToBoolean(staging) ? "stg" : "dbo";
            }

            foreach (EdmFunction function in workspace.GetItems<EdmFunction>(DataSpace.SSpace))
            {
                MetadataProperty builtInAttribute = function.MetadataProperties.FirstOrDefault(p => p.Name == "BuiltInAttribute");

                if (builtInAttribute != null && Convert.ToBoolean(builtInAttribute.Value.ToString()) == false)
                {
                    FieldInfo schemaField = function.GetType().GetRuntimeFields().Where(a => a.Name.Equals("_schemaName")).FirstOrDefault();

                    if (!function.Schema.Equals(schema, StringComparison.OrdinalIgnoreCase))
                    {
                        schemaField.SetValue(function, schema);
                    }
                }
            }
        }
    }

}

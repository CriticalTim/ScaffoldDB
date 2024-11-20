using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.CodeAnalysis;
using ScaffoldDB.Data;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics.CodeAnalysis;

namespace ScaffoldDB.Services
{
    public class DatabaseSchemaService
    {
        private readonly IServiceProvider _serviceProvider;

        public DatabaseSchemaService()
        {
            // Configure the EF Core scaffolding services
            _serviceProvider = new ServiceCollection()
                .AddEntityFrameworkDesignTimeServices()
                .AddEntityFrameworkSqlServer()
                .BuildServiceProvider();
        }

        public List<TableSchema> ScaffoldDatabaseSchema(string connectionString)
        {
            var scaffolder = _serviceProvider.GetRequiredService<IReverseEngineerScaffolder>();

            var options = new ModelCodeGenerationOptions
            {
                SuppressOnConfiguring = true,
                SuppressConnectionStringWarning = true
            };

            // Scaffold the database model
            var scaffoldedModel = scaffolder.ScaffoldModel(
                connectionString,
                new DatabaseModelFactoryOptions(),
                new ModelReverseEngineerOptions(),
                options);

            var databaseModel = scaffoldedModel.ContextFile;
            var tables = new List<TableSchema>();

            // Extract tables and columns
            foreach (var table in databaseModel.Code)
            {
                var tableSchema = new TableSchema
                {
                    //TableName = table.Name,
                    //Columns = table.Columns.Select(col => new ColumnSchema
                    //{
                    //    ColumnName = col.Name,
                    //    DataType = col.StoreType
                    //}).ToList()
                };

                tables.Add(tableSchema);
            }

            return tables;
        }
    }

   
}

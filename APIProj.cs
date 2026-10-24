using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Dapper;

namespace USS.Tool.GenCRUD
{
    public class APIProj
    {
        public static string APIProjFile
        {
            get
            {
                if (string.IsNullOrEmpty(field))
                {
                    field = SolutionInfo.args.FirstOrDefault(x => x.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase));

                    if (field == null)
                    {
                        foreach (var file in Directory.GetFiles(Environment.CurrentDirectory, "*.csproj", SearchOption.AllDirectories))
                        {
                            field = file;
                            break;
                        }
                    }

                }
                return field;
            }
        }
        public static string APIProjDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(field))
                {
                    field = Path.GetDirectoryName(APIProjFile);
                }
                return field?? Environment.CurrentDirectory;
            }
        }

        public static string ParentDir
        {
            get
            {
                if (string.IsNullOrEmpty(field))
                {
                    field = Directory.GetParent(APIProjDirectory)?.FullName ?? "";
                }
                return field;
            }
        }

        public static string RootNamespace { get => Directory.GetParent(APIProj.APIProjDirectory).Name; }
        
        public static String SettingFile
        {
            get
            {
                if (string.IsNullOrEmpty(field))
                {
                    field = Path.Combine(APIProjDirectory, "appsettings.json");
                }
                return field;
            }
        }

        public static string DefaultConnection
        {
            get
            {
                if (string.IsNullOrEmpty(field))
                {
                    IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile(SettingFile, optional: false, reloadOnChange: true)
                .Build();
                  field = config.GetConnectionString("DefaultConnection");
                }
                return field??string.Empty;
            }
        }
        public static bool IsValid
        {
            get
            {
                if (APIProjFile == null)
                {
                    Console.WriteLine("No .csproj file found.");
                    return false;
                }
                else
                {
                    if (!File.Exists(APIProjFile))
                    {
                        Console.WriteLine("Invalid file path. Please provide a valid .csproj file.");
                        return false;
                    }
                    Console.WriteLine($"CS Project File: {APIProjFile}");

                    if (!File.Exists(SettingFile))
                    {
                        Console.WriteLine("Setting file is not Found");
                        return false;
                    }
                    Console.WriteLine($"Setting File: {SettingFile}");

                    if (string.IsNullOrEmpty(DefaultConnection))
                    {
                        Console.WriteLine("Default Connection is not found");
                        return field;
                    }
                    Console.WriteLine(DefaultConnection);

                }
                return true;
            }
        }

        public static List<SQLTable> tables
        {
            get
            {
                if (field == null)
                {
                    var con = new SqlConnection(DefaultConnection);
                    var resultTables = con.Query<SQLTable>("SELECT name as TableName FROM sys.tables ORDER BY name;").ToList();
                    foreach (var t in resultTables)
                    {
                        var qry = $"SELECT c.name AS Name,ty.name AS DataType,c.max_length AS DataSize,c.is_nullable AS IsNullable FROM sys.tables AS t INNER JOIN sys.all_columns AS c ON t.object_id = c.object_id INNER JOIN sys.types AS ty ON c.user_type_id = ty.user_type_id where t.name = '{t.TableName}' order by c.column_id";
                        var resultFields = con.Query<SQLFieldInfo>(qry).ToList();
                        t.Fields = new List<SQLFieldInfo>();
                        foreach (var f in resultFields)
                        {
                            t.Fields.Add(f);
                        }
                    }
                    field = resultTables;
                }

                return field;
            }
        }

    }
}

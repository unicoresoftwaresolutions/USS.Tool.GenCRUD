using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text.Json;


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
                return new List<SQLTable>();
            }
        }

    }
}

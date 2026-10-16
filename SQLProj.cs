using System.Xml.Linq;

namespace USS.Tool.GenCRUD
{
    internal static class SQLProj
    {
        private static readonly string[] args;

        static SQLProj()
        {
            args = Environment.GetCommandLineArgs().Skip(1).ToArray();
        }

        
        public static string SQLProjFile 
        { 
            get 
            {
                if(string.IsNullOrEmpty(field))
                {
                    field = args.FirstOrDefault(x => x.EndsWith(".sqlproj", StringComparison.OrdinalIgnoreCase));

                    if (field == null)
                    {
                        foreach (var file in Directory.GetFiles(Environment.CurrentDirectory, "*.sqlproj", SearchOption.AllDirectories))
                        {
                            field = file;
                            field = file;
                            break;
                        }
                    }

                }
                return field;
            }
        }

        public static string sqlProjFileDir
        {
            get
            {
                if (string.IsNullOrEmpty(field))
                {
                    field = Path.GetDirectoryName(SQLProjFile);
                }
                return field??"";
            }
        }
        public static string ParentDir
        {
            get
            {
                if (string.IsNullOrEmpty(field))
                {
                    field = Directory.GetParent(sqlProjFileDir)?.FullName ?? "";
                }
                return field;
            }
        }

        public static XDocument SqlProjDocument
        {
            get
            {
                if (field == null)
                {
                    try
                    {
                        field = XDocument.Load(SQLProjFile);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading .sqlproj file: {ex.Message}");
                    }
                }
                return field;
            }
        }

        public static List<string> SqlFiles
        {
            get
            {
                if (field == null)
                {
                    field = new List<string>();

                    var argSqlFiles = args.Where(x => x.EndsWith(".sql", StringComparison.OrdinalIgnoreCase)).ToList();

                    try
                    {
                        var projSqlFiles = SqlProjDocument.Descendants()
                                  .Where(e => e.Name.LocalName == "Build" || e.Name.LocalName == "None")
                                  .Select(e => e.Attribute("Include")?.Value)
                                  .Where(path => !string.IsNullOrWhiteSpace(path) && path.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
                                  .ToList();
                        if (argSqlFiles.Count == 0)
                        {
                            field = projSqlFiles.Select(sqlfile => Path.Combine(sqlProjFileDir, sqlfile)).ToList();
                        }
                        else
                        {
                            foreach (var sqlFile in argSqlFiles)
                            {
                                var sf = projSqlFiles.FirstOrDefault(x => x.EndsWith(sqlFile.Contains("\\") ? sqlFile : $"\\{sqlFile}"));

                                if (sf == null)
                                {
                                    Console.WriteLine($"Warning: The SQL file '{sqlFile}' is not included in the .sqlproj file.");
                                }
                                else
                                {
                                    field.Add(Path.Combine(sqlProjFileDir, sf));
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading .sqlproj file: {ex.Message}");
                    }

                }
                return field;
            }
        }

        public static string SQLRootNameSpace
        {
            get => SqlProjDocument.Descendants().Where(x => x.Name.LocalName == "RootNamespace").FirstOrDefault()?.Value??string.Empty;
        }

        public static string RootNamespace { get => SQLRootNameSpace.Substring(0, SQLRootNameSpace.Length - 4); }
        public static bool IsValid
        {
            get
            {
                if (SQLProjFile == null)
                {
                    Console.WriteLine("No .sqlproj file found.");
                    return false;
                }
                else
                {
                    if (!File.Exists(SQLProjFile))
                    {
                        Console.WriteLine("Invalid file path. Please provide a valid .sqlproj file.");
                        return false;
                    }
                    Console.WriteLine($"SQL Project File: {SQLProjFile}");

                    List<String> SqlFiles = SQLProj.SqlFiles;
                    if(SqlFiles.Count == 0)
                    {
                        Console.WriteLine("No SQL files found in the .sqlproj file.");
                        return false;
                    }
                    Console.WriteLine($"SQL Files :");

                    foreach (var sqlFile in SqlFiles)
                    {
                        Console.WriteLine(sqlFile);
                    }
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
                    field = new List<SQLTable>();
                    foreach (var sqlfile in SqlFiles)
                    {
                        SQLTable table = new SQLTable(sqlfile);
                        field.Add(table);
                    }
                }
                return field;
            }
        }
    }
}

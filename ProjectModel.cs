using USS.Tool.GenCRUD.Templates;

namespace USS.Tool.GenCRUD
{
    public class ProjectModel
    {
        public static ProjectInfo project;
        public SQLTable Table { get; set; }
        public ProjectModel(SQLTable table)
        {
            this.Table = table;
        }
        public string FilePathModel
        {
            get
            {
                if (field == null)
                {
                    field = $"{project.Path}\\Models";
                }
                return field;
            }
        }

        public string FilePathDataInterface
        {
            get
            {
                if (field == null)
                {
                    field = $"{project.RootPath}\\{RootNamespace}.Data.Interface";
                }
                return field;
            }
        }

        public string FilePathBusinessInterface
        {
            get
            {
                if (field == null)
                {
                    field = $"{project.RootPath}\\{RootNamespace}.Business.Interface";
                }
                return field;
            }
        }

        public string FilePathBusiness
        {
            get
            {
                if (field == null)
                {
                    field = $"{project.RootPath}\\{RootNamespace}.Business";
                }
                return field;
            }
        }

        public string FilePathData
        {
            get
            {
                if (field == null)
                {
                    field = $"{project.RootPath}\\{RootNamespace}.Data";
                }
                return field;
            }
        }

        public string FilePathService
        {
            get
            {
                if (field == null)
                {
                    field = $"{project.RootPath}\\{RootNamespace}.Service";
                }
                return field;
            }
        }

        public string FilePathController
        {
            get
            {
                if (field == null)
                {
                    field = $"{project.RootPath}\\{RootNamespace}.ApiService\\Controllers";
                }
                return field;
            }
        }


        public string FilePathComponent
        {
            get
            {
                if (field == null)
                {
                    field = $"{project.RootPath}\\{RootNamespace}.Web\\Components\\Pages";
                }
                return field;
            }
        }

        public string FileNameModel
        {
            get
            {
                if(field==null)
                {
                    field = $"{FilePathModel}\\{Table.TableName}.cs";
                }
                return field;
            }
        }


        public string FileNameDataInterface
        {
            get
            {
                if (field == null)
                {
                    field = $"{FilePathDataInterface}\\I{Table.TableName}Data.cs";
                }
                return field;
            }
        }


        public string FileNameBusinessInterface
        {
            get
            {
                if (field == null)
                {
                    field = $"{FilePathBusinessInterface}\\I{Table.TableName}Business.cs";
                }
                return field;
            }
        }
        public string FileNameBusiness
        {
            get
            {
                if (field == null)
                {
                    field = $"{FilePathBusiness}\\{Table.TableName}Business.cs";
                }
                return field;
            }
        }
        public string FileNameData
        {
            get
            {
                if (field == null)
                {
                    field = $"{FilePathData}\\{Table.TableName}Data.cs";
                }
                return field;
            }
        }
        public string FileNameService
        {
            get
            {
                if (field == null)
                {
                    field = $"{FilePathService}\\{Table.TableName}Service.cs";
                }
                return field;
            }
        }

        public string FileNameController
        {
            get
            {
                if (field == null)
                {
                    field = $"{FilePathController}\\{Table.TableName}Controller.cs";
                }
                return field;
            }
        }

        public string FileNameComponent
        {
            get
            {
                if (field == null)
                {
                    field = $"{FilePathComponent}\\{Table.TableName}Grid.razor";
                }
                return field;
            }
        }

        public string RootNamespace
        {
            get
            {
                if (field == null)
                {
                    field = $"{project.Namespace}";
                }
                return field;
            }
        }

        public string Namespace
        {
            get
            {
                if (field == null)
                {
                    field = $"{project.ProjectName}";
                }
                return field;
            }
        }

        public string ClassName
        {
            get
            {
                if (field == null)
                {
                    field = $"{Table.TableName}";
                }
                return field;
            }
        }

        public List<SQLFieldInfo> Fields
        {
            get
            {
                if (field == null)
                {
                    field = Table.Fields.Where(x=> x.Name != "Id").ToList();
                }
                foreach (var fieldType in SQLFieldInfo.Types.Where(x=> field.Any(f => f.DataType.ToLower() == x.DataType.ToLower())).ToList())
                {
                    foreach (var item in field.Where(x => x.DataType.ToLower() == fieldType.DataType.ToLower()).ToList())
                    {
                        item.DataType = fieldType.FieldType;
                    }
                }
                return field;
            }
        }

        public List<SQLFieldInfo> References
        {
            get
            {
                return Fields.Where(x => x.HasForeignKey).ToList();
            }
        }

        public List<SQLTable> LookUpTables { get; set; } = new List<SQLTable>();

        public string ModelClass
        {
            get
            {
                if (field == null)
                {
                    ModelTemplate model = new ModelTemplate(this);
                    field = model.TransformText();
                }
                return field;
            }
        }

        public bool GenerateModel(bool IsOverRide = false)
        {
            if (!Directory.Exists(FilePathModel))
            {
                Directory.CreateDirectory(FilePathModel);
            }
            if (File.Exists(FileNameModel) && !IsOverRide)
            {
                return false;
            }
            File.WriteAllText(FileNameModel, ModelClass);
            return true;
        }


        public string DataInterface
        {
            get
            {
                if (field == null)
                {
                    DataInterfaceTemplate model = new DataInterfaceTemplate(this);
                    field = model.TransformText();
                }
                return field;
            }
        }
        public string BusinessInterface
        {
            get
            {
                if (field == null)
                {
                    BusinessInterfaceTemplate model = new BusinessInterfaceTemplate(this);
                    field = model.TransformText();
                }
                return field;
            }
        }

        public string Business
        {
            get
            {
                if (field == null)
                {
                    BusinessTemplate model = new BusinessTemplate(this);
                    field = model.TransformText();
                }
                return field;
            }
        }

        public string Data
        {
            get
            {
                if (field == null)
                {
                    DataTemplate model = new DataTemplate(this);
                    field = model.TransformText();
                }
                return field;
            }
        }

        public string Service
        {
            get
            {
                if (field == null)
                {
                    ServiceTemplate model = new ServiceTemplate(this);
                    field = model.TransformText();
                }
                return field;
            }
        }


        public string Controller
        {
            get
            {
                if (field == null)
                {
                    ControllerTemplate model = new ControllerTemplate(this);
                    field = model.TransformText();
                }
                return field;
            }
        }

        public string Component
        {
            get
            {
                if (field == null)
                {
                    GridComponentTemplate model = new GridComponentTemplate(this);
                    field = model.TransformText();
                }
                return field;
            }
        }

        public string ComponentCodeBehind
        {
            get
            {
                if (field == null)
                {
                    GridComponentCodeTemplate model = new GridComponentCodeTemplate(this);
                    field = model.TransformText();
                }
                return field;
            }
        }


        public bool GenerateDataInterface(bool IsOverRide = false)
        {
            if (!Directory.Exists(FilePathDataInterface))
            {
                Directory.CreateDirectory(FilePathDataInterface);
            }
            if (File.Exists(FileNameDataInterface) && !IsOverRide)
            {
                return false;
            }
            File.WriteAllText(FileNameDataInterface, DataInterface);
            return true;
        }
        public bool GenerateBusinessInterface(bool IsOverRide = false)
        {
            if (!Directory.Exists(FilePathBusinessInterface))
            {
                Directory.CreateDirectory(FilePathBusinessInterface);
            }
            if (File.Exists(FileNameBusinessInterface) && !IsOverRide)
            {
                return false;
            }
            File.WriteAllText(FileNameBusinessInterface, BusinessInterface );
            return true;
        }

        public bool GenerateBusiness(bool IsOverRide = false)
        {
            if (!Directory.Exists(FilePathBusiness))
            {
                Directory.CreateDirectory(FilePathBusiness);
            }
            if (File.Exists(FileNameBusiness) && !IsOverRide)
            {
                return false;
            }
            File.WriteAllText(FileNameBusiness, Business);
            return true;
        }
        public bool GenerateData(bool IsOverRide = false)
        {
            if (!Directory.Exists(FilePathData))
            {
                Directory.CreateDirectory(FilePathData);
            }
            if (File.Exists(FileNameData) && !IsOverRide)
            {
                return false;
            }
            File.WriteAllText(FileNameData, Data);
            return true;
        }
        public bool GenerateService(bool IsOverRide = false)
        {
            if (!Directory.Exists(FilePathService))
            {
                Directory.CreateDirectory(FilePathService);
            }
            if (File.Exists(FileNameService) && !IsOverRide)
            {
                return false;
            }
            File.WriteAllText(FileNameService, Service);
            return true;
        }


        public bool GenerateController(bool IsOverRide = false)
        {
            if (!Directory.Exists(FilePathController))
            {
                Directory.CreateDirectory(FilePathController);
            }
            if (File.Exists(FileNameController) && !IsOverRide)
            {
                return false;
            }
            File.WriteAllText(FileNameController, Controller);
            return true;
        }

        public bool GenerateComponent(bool IsOverRide = false)
        {
            if (!Directory.Exists(FilePathComponent))
            {
                Directory.CreateDirectory(FilePathComponent);
            }
            if (File.Exists(FileNameComponent) && !IsOverRide)
            {
                return false;
            }
            File.WriteAllText(FileNameComponent, Component);
            File.WriteAllText($"{FileNameComponent}.cs", ComponentCodeBehind);
            return true;
        }
    }
}

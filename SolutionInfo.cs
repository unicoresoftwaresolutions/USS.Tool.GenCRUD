using System;
using System.Collections.Generic;
using System.Text;
using USS.Tool.GenCRUD.Templates;

namespace USS.Tool.GenCRUD
{
    public static class SolutionInfo
    {

        public static List<string> args
        {
            get => Environment.GetCommandLineArgs().Skip(1).ToList();            
        }

        static SolutionInfo()
        {
            IsAPIProj = APIProj.IsValid;
            IsSQLProj = SQLProj.IsValid;
        }
        public static string Name 
        {
            get
            {
                return IsAPIProj ? APIProj.RootNamespace : SQLProj.RootNamespace; ;
            }
        }
        public static string SolutionPath
        {
            get 
            { 
                return IsAPIProj? APIProj.ParentDir : SQLProj.ParentDir;
            }
        }
        public static string SolutionFile { get; set; }
        public static bool IsAPIProj;

        public static bool IsSQLProj;

        public static List<ProjectInfo> Projects 
        {
            get
            { 
                if(field== null)
                {
                    field = new List<ProjectInfo>();
                    field.Add(new ProjectInfo(ProjectNames.Model));
                    field.Add(new ProjectInfo(ProjectNames.DataInterface));
                    field.Add(new ProjectInfo(ProjectNames.Data));
                    field.Add(new ProjectInfo(ProjectNames.BusinessInterface));
                    field.Add(new ProjectInfo(ProjectNames.Business));
                    field.Add(new ProjectInfo(ProjectNames.ApiService));
                    field.Add(new ProjectInfo(ProjectNames.Service));
                    field.Add(new ProjectInfo(ProjectNames.Web));
                }
                return field;
            }
        } 

        public static List<ProjectModel> ProjectModels
        {
            get
            {
                if (field == null)
                {
                    ProjectModel.project = Projects.FirstOrDefault(x=> x.Name==ProjectNames.Model);
                    field = new List<ProjectModel>();

                    var tables = IsAPIProj ? APIProj.tables : SQLProj.tables;

                    tables.ForEach(table =>
                    {
                        field.Add(new ProjectModel(table));
                    });
                    var RefFields = field.Where(x => x.Table.Fields.Any(f => f.HasForeignKey)).ToList();
                    foreach (var projModel in RefFields)
                    {
                        if (projModel.LookUpTables == null) projModel.LookUpTables = new List<SQLTable>();
                        foreach (var reffield in projModel.References)
                        {
                            var proj = field.FirstOrDefault(x => x.ClassName == reffield.Reference.TableName);
                            if (proj == null) continue;
                            if (proj.LookUpTables == null) proj.LookUpTables = new List<SQLTable>();
                            else
                            {
                                if (proj.LookUpTables.Any(x => x.TableName == projModel.Table.TableName))
                                {
                                    continue;
                                }
                            }
                            proj.LookUpTables.Add(projModel.Table);
                        }
                    }
                }
                
                return field;
            }
        }

        public static void Generate(bool IsOverRide = false)
        {
            foreach (var projModel in ProjectModels)
            {
                projModel.GenerateModel(IsOverRide);
                projModel.GenerateDataInterface(IsOverRide);
                projModel.GenerateBusinessInterface(IsOverRide);
                projModel.GenerateData(IsOverRide);
                projModel.GenerateBusiness(IsOverRide);
                projModel.GenerateService(IsOverRide);
                projModel.GenerateController(IsOverRide);
                projModel.GenerateComponent(IsOverRide);
                GenerateAPIServiceDI(true);
                GenerateUIServiceDI(true);
                GenerateMainDBContext(true);
            }
        }

        public static void GenerateAPIServiceDI(bool IsOverRide = false)
        {
            APIServiceDITemplate apiServiceDITemplate = new APIServiceDITemplate(ProjectModels);
            string content = apiServiceDITemplate.TransformText();
            string filePath = Path.Combine(SolutionInfo.Projects.FirstOrDefault(x => x.Name == ProjectNames.ApiService).Path, "ServiceDI.cs");
            if (File.Exists(filePath) && !IsOverRide)
            {
                return;
            }
            File.WriteAllText(filePath, content);
        }

        public static void GenerateUIServiceDI(bool IsOverRide = false)
        {
            UIServiceDITemplate uiServiceDITemplate = new UIServiceDITemplate(ProjectModels);
            string content = uiServiceDITemplate.TransformText();
            string filePath = Path.Combine(SolutionInfo.Projects.FirstOrDefault(x => x.Name == ProjectNames.Web).Path, "ServiceDI.cs");
            if (File.Exists(filePath) && !IsOverRide)
            {
                return;
            }
            File.WriteAllText(filePath, content);
        }

        public static void GenerateMainDBContext(bool IsOverRide = false)
        {
            MainDBContextTemplate mainDBContextTemplate = new MainDBContextTemplate(ProjectModels);
            string content = mainDBContextTemplate.TransformText();
            string filePath = Path.Combine(SolutionInfo.Projects.FirstOrDefault(x => x.Name == ProjectNames.Data).Path, "MainDBContext.cs");
            if (File.Exists(filePath) && !IsOverRide)
            {
                return;
            }
            File.WriteAllText(filePath, content);
        }
    }
}

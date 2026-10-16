namespace USS.Tool.GenCRUD.Templates
{
    public partial class APIServiceDITemplate
    {
        private readonly List<ProjectModel> models;
        private readonly string RootNamespace;
        public APIServiceDITemplate(List<ProjectModel> models)
        {
            this.models = models;
            RootNamespace = models.FirstOrDefault()?.RootNamespace ?? "USS";
        }
    }
}

namespace USS.Tool.GenCRUD.Templates
{
    public partial class UIServiceDITemplate
    {
        private readonly List<ProjectModel> models;
        private readonly string RootNamespace;
        public UIServiceDITemplate(List<ProjectModel> models)
        {
            this.models = models;
            RootNamespace = models.FirstOrDefault()?.RootNamespace ?? "USS";
        }
    }
}

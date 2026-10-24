using System;
using System.Collections.Generic;
using System.Text;

namespace USS.Tool.GenCRUD.Templates
{
    public partial class GridModelMenuTemplate
    {
        private readonly List<ProjectModel> models;
        private readonly string RootNamespace;
        public GridModelMenuTemplate(List<ProjectModel> models)
        {
            this.models = models;
            RootNamespace = models.FirstOrDefault()?.RootNamespace ?? "USS";
        }
    }
}

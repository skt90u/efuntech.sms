using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.CodeGenerator
{
    public class CrudApiControllerGenerator : IJob
    {
        public string PortalDir { get; set; }
        public bool Overwrite { get; set; }
        public SchemaInfo SchemaInfo { get; set; }

        private List<string> CreateFileContent()
        {
            var lines = new List<string>();

            string controllerName = SchemaInfo.ApiControllerName;
            var modelName = SchemaInfo.ModelName;
            var schemaName = SchemaInfo.SchemaName;
            var properties = SchemaInfo.Properties;
            var searchableProperties = SchemaInfo.SearchableProperties;
            var idTypeString = SchemaInfo.IdTypeString;

            lines.Add(string.Format("using EFunTech.Sms.Portal.Models;"));
            lines.Add(string.Format("using EFunTech.Sms.Schema;"));
            lines.Add(string.Format("using System.Linq;"));
            lines.Add(string.Format("using EFunTech.Sms.Portal.Controllers.Common;"));
            lines.Add(string.Format("using EFunTech.Sms.Portal.Models.Common;"));
            lines.Add(string.Format("using JUtilSharp.Database;"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("using System.Collections.Generic;"));
            lines.Add(string.Format("using LinqKit;"));
            lines.Add(string.Format("using System;"));
            
            lines.Add("");
            lines.Add("namespace EFunTech.Sms.Portal.Controllers");
            lines.Add("{");

            lines.Add(string.Format("public class {0} : CrudApiController<SearchTextCriteriaModel, {1}, {2}, {3}>",
                controllerName,
                modelName,
                schemaName,
                idTypeString
                ));
            lines.Add("{");

            // Constructor
            lines.Add(string.Format("public {0}(IUnitOfWork unitOfWork, ILogService logService) : base(unitOfWork, logService) {{ }}", controllerName));
            lines.Add("");

            // DoGetList
            lines.Add(string.Format("protected override IOrderedQueryable<{0}> DoGetList(SearchTextCriteriaModel criteria)", schemaName));
            lines.Add("{");

            lines.Add(string.Format("IQueryable<{0}> result = CurrentUser.{0}s.AsQueryable();", schemaName));
            lines.Add("");

            lines.Add(string.Format("            var predicate = PredicateBuilder.True<{0}>();", schemaName));
            lines.Add(string.Format(""));

            lines.Add(string.Format("            var searchText = criteria.SearchText;"));
            lines.Add(string.Format("            if (!string.IsNullOrEmpty(searchText))"));
            lines.Add(string.Format("            {{"));
            lines.Add(string.Format("                var innerPredicate = PredicateBuilder.False<{0}>();", schemaName));
            lines.Add(string.Format(""));
            for (int i = 0; i < searchableProperties.Count; i++)
            {
                var property = searchableProperties[i];
                var propertyName = property.Name;
                lines.Add(string.Format("                innerPredicate = innerPredicate.Or(p => !string.IsNullOrEmpty(p.{0}) && p.{0}.Contains(searchText));", propertyName));
            }
            lines.Add(string.Format(""));
            lines.Add(string.Format("                predicate = predicate.And(innerPredicate);"));
            lines.Add(string.Format("            }}"));

            lines.Add(string.Format("            result = result.AsExpandable().Where(predicate);"));
            lines.Add("");

            lines.Add(string.Format("return result.OrderByDescending(p => p.Id);", schemaName));

            lines.Add("}");
            lines.Add("");

            // DoGet
            lines.Add(string.Format("protected override {0} DoGet({1} id)", schemaName, idTypeString));
            lines.Add("{");
            lines.Add(string.Format("return CurrentUser.{0}s.Where(p => p.Id == id).FirstOrDefault();", schemaName));
            lines.Add("}");
            lines.Add("");

            // DoCreate
            lines.Add(string.Format("protected override {0} DoCreate({1} model, {2} entity, out {3} id)", schemaName, modelName, schemaName, idTypeString));
            lines.Add("{");
            lines.Add(string.Format("entity = new {0}();", schemaName));
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var modelField = string.Format("model.{0}", property.Name);

                if (propertyName == "Id") continue;
                if (propertyName == "CreatedUser") modelField = "CurrentUser";
                if (propertyName == "UpdatedTime") modelField = "DateTime.UtcNow";
                if (propertyName == "UpdatedUser") modelField = "CurrentUser";
                if (propertyName == "UpdatedUserName") modelField = "CurrentUser.UserName";

                if (propertyName == "string" && modelField.StartsWith("model."))
                {
                    modelField = modelField + ".Trim()";
                }
                lines.Add(string.Format("entity.{0} = {1};", propertyName, modelField));
            }
            lines.Add("");

            lines.Add(string.Format("entity = this.repository.Insert(entity);"));
            lines.Add(string.Format("id = entity.Id;"));
            lines.Add("");

            lines.Add(string.Format("return entity;"));
            lines.Add("}");
            lines.Add("");

            // DoUpdate
            lines.Add(string.Format("protected override void DoUpdate({0} model, {1} id, {2} entity)", modelName, idTypeString, schemaName));
            lines.Add("{");
            lines.Add(string.Format("if (!CurrentUser.{0}s.Any(p => p.Id == id)) return;", schemaName));
            lines.Add("");
            if (properties.Any(p => p.Name == "UpdatedTime"))
            {
                lines.Add(string.Format("entity.UpdatedTime = DateTime.UtcNow;"));
                lines.Add("");
            }
            if (properties.Any(p => p.Name == "UpdatedUser"))
            {
                lines.Add(string.Format("entity.UpdatedUser = CurrentUser;"));
                lines.Add("");
            }
            if (properties.Any(p => p.Name == "UpdatedUserName"))
            {
                lines.Add(string.Format("entity.UpdatedUserName = CurrentUser.UserName;"));
                lines.Add("");
            }
            lines.Add(string.Format("this.repository.Update(entity);"));
            lines.Add("}");
            lines.Add("");

            // DoRemove(單筆)
            lines.Add(string.Format("protected override void DoRemove({0} id, {1} entity)", idTypeString, schemaName));
            lines.Add("{");
            lines.Add(string.Format("if (!CurrentUser.{0}s.Any(p => p.Id == id)) return;", schemaName));
            lines.Add("");
            lines.Add(string.Format("this.repository.Delete(entity);"));
            lines.Add("}");
            lines.Add("");

            // DoRemove(多筆)
            lines.Add(string.Format("protected override void DoRemove(List<{0}> ids, List<{1}> entities)", idTypeString, schemaName));
            lines.Add("{");
            lines.Add(string.Format("if (!CurrentUser.{0}s.Any(p => ids.Contains(p.Id))) return;", schemaName));
            lines.Add("");
            lines.Add(string.Format("this.repository.Delete(p => ids.Contains(p.Id));"));
            lines.Add("}");
            lines.Add("");

            lines.Add("}"); // class

            lines.Add("}"); // namespace

            return lines;
        }

        public void Execute()
        {
            var lines = CreateFileContent();
            var filePath = SchemaInfo.GetApiControllerFilePath(PortalDir);
            Utils.WriteToCSharpFile(filePath, lines, Overwrite);
        }
    }
}

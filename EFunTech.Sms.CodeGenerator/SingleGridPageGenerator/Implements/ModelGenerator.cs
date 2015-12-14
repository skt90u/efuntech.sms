using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EFunTech.Sms.CodeGenerator
{
    public class ModelGenerator : IJob
    {
        public string PortalDir { get; set; }
        public bool Overwrite { get; set; }
        public SchemaInfo SchemaInfo { get; set; }

        private void WritePropertyAttributes(List<string> lines, SchemaPropertyInfo property)
        {
            
        }

        private List<string> CreateFileContent()
        {
            var lines = new List<string>();

            var properties = SchemaInfo.Properties;
            var modelName = SchemaInfo.ModelName;

            lines.Add("using EFunTech.Sms.Schema;");
            lines.Add("using System;");
            lines.Add("");
            lines.Add("namespace EFunTech.Sms.Portal.Models");
            lines.Add("{");

            lines.Add(string.Format("public class {0}", modelName));
            lines.Add("{");
            foreach (var property in properties)
            {
                if (!property.IsModelProperty) continue;

                if (property.IsSchemaType)
                {
                    var generator = new ModelGenerator
                    {
                        SchemaInfo =  new SchemaInfo { 
                            SchemaType = property.PropertyType,
                        },
                        PortalDir = PortalDir,
                        Overwrite = Overwrite,
                    };
                    generator.Execute();
                }

                string propertyType = property.PropertyTypeString;
                string propertyName = property.Name;
                WritePropertyAttributes(lines, property);
                lines.Add(string.Format("public {0} {1} {{ get; set; }}", propertyType, propertyName));
                lines.Add("");
            }
            lines.Add("}"); // class

            lines.Add("}"); // namespace

            return lines;
        }

        public void Execute()
        {
            var lines = CreateFileContent();
            var filePath = SchemaInfo.GetModelFilePath(PortalDir);
            Utils.WriteToCSharpFile(filePath, lines, Overwrite);
        }
    }
}

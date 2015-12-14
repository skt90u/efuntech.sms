using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.CodeGenerator
{
    public class AutoMapperProfileGenerator : IJob
    {
        public string PortalDir { get; set; }
        public bool Overwrite { get; set; }
        public SchemaInfo SchemaInfo { get; set; }

        private List<string> CreateFileContent()
        {
            var lines = new List<string>();

            var properties = SchemaInfo.Properties;
            var profileName = SchemaInfo.ProfileName;
            var modelName = SchemaInfo.ModelName;
            var schemaName = SchemaInfo.SchemaName;

            lines.Add("using AutoMapper;");
            lines.Add("using EFunTech.Sms.Schema;");
            lines.Add("using System.Linq;");
            lines.Add("");
            lines.Add("namespace EFunTech.Sms.Portal.Models.Mapper");
            lines.Add("{");

            lines.Add(string.Format("public class {0} : Profile", profileName));
            lines.Add("{");

            lines.Add("protected override void Configure()");
            lines.Add("{");

            lines.Add(string.Format("CreateMap<{0}, {1}>()", schemaName, modelName));
            foreach (var property in properties)
            {
                if (!property.IsModelProperty) continue;

                if (property.IsSchemaType)
                {
                    var generator = new AutoMapperProfileGenerator
                    {
                        SchemaInfo = new SchemaInfo
                        {
                            SchemaType = property.PropertyType,
                        },
                        PortalDir = PortalDir,
                        Overwrite = Overwrite,
                    };
                    generator.Execute();
                }
                lines.Add(string.Format("                .ForMember(dst => dst.{0}, opt => opt.MapFrom(src => src.{0}))", property.Name));
            }
            lines.Add("                ;");

            lines.Add("");

            lines.Add(string.Format("CreateMap<{0}, {1}>()", modelName, schemaName));
            foreach (var property in properties)
            {
                if (!property.IsModelProperty) continue;
                lines.Add(string.Format("                .ForMember(dst => dst.{0}, opt => opt.MapFrom(src => src.{0}))", property.Name));
            }
            lines.Add("                ;");

            lines.Add("}"); // Configure

            lines.Add("}"); // class

            lines.Add("}"); // namespace

            return lines;
        }

        public void Execute()
        {
            var lines = CreateFileContent();
            var filePath = SchemaInfo.GetProfileFilePath(PortalDir);
            Utils.WriteToCSharpFile(filePath, lines, Overwrite);
        }
    }
}

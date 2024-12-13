using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace KaydenMiller.ApiTools
{
    public readonly record struct JsonConverterToGenerate
    {
        public readonly string Name;
        public readonly string FullyQualifiedTypeName;

        public JsonConverterToGenerate(string name)
        {
            Name = name;
            FullyQualifiedTypeName = name;
        }
    }
    
    public static class SourceGeneratorHelper
    {
        public const string Attribute = @"
namespace KaydenMiller.ApiTools 
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class WithJsonConverterAttribute : System.Attribute 
    
    }
}
        ";
        
        public static string GenerateExtensionClass(JsonConverterToGenerate converterToGenerate)
        {
            var sb = new StringBuilder();
            sb.Append($$"""
namespace KaydenMiller.ApiTools
{
    public static partial class {{converterToGenerate.Name}}Converter : JsonConverter
    {
        public override {{converterToGenerate.FullyQualifiedTypeName}} Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) 
        {
            return {{converterToGenerate.FullyQualifiedTypeName}}.Parse(reader.GetString()!, null);
        }
        
        public override void Write(Utf8JsonWriter writer, {{converterToGenerate.FullyQualifiedTypeName}} value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
""");

            return sb.ToString();
        }
    }

    [Generator]
    public class JsonConverterGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // add the marker attribute to the compilation
            context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
                "WithJsonConverterAttribute.g.cs",
                SourceText.From(SourceGeneratorHelper.Attribute, Encoding.UTF8)));
            
            // Do a simple filter for classes
            // IncrementalValueProvider<JsonConverterToGenerate?> converterToGenerate = context.SyntaxProvider
            //     .CreateSyntaxProvider(
            //         predicate: static (s, _) => IsSyntaxTargetForGeneration(s), // select enums with attributes
            //         transform: static (ctx, _) =>
            //             GetSemanticTargetForGeneration(ctx)) // select enums with the [SpecificAttribute]
            //     .Where(static m => m is not null); // filter out errors that we don't care about
            
            // If you're targeting the .NET 7 SDK, use this version instead:
            IncrementalValuesProvider<JsonConverterToGenerate?> converterToGenerate = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    "KaydenMiller.ApiTools.WithJsonConverterAttribute",
                    predicate: static (s, _) => true,
                    transform: static (ctx, _) => GetEnumToGenerate(ctx.SemanticModel, ctx.TargetNode))
                .Where(static m => m is not null);
            
            // Generate source code for each class found
            context.RegisterSourceOutput(converterToGenerate, 
                static (spc, source) => Execute(source, spc));
        }
        
        private static void Execute(JsonConverterToGenerate? converterToGenerate,
            SourceProductionContext context)
        {
            if (converterToGenerate is { } value)
            {
                // generate the source code and add it to the output
                string result = SourceGeneratorHelper.GenerateExtensionClass(value);
                
                context.AddSource($"JsonConverter.{value.Name}.g.cs", SourceText.From(result, Encoding.UTF8, SourceHashAlgorithm.Sha1));
            }
        }
        
        private static JsonConverterToGenerate? GetEnumToGenerate(SemanticModel semanticModel, SyntaxNode enumDeclarationSyntax)
        {
            // Get the semantic representation of the enum syntax
            if (semanticModel.GetDeclaredSymbol(enumDeclarationSyntax) is not INamedTypeSymbol enumSymbol)
            {
                // something went wrong
                return null;
            }

            // Get the full type name of the enum e.g. Colour, 
            // or OuterClass<T>.Colour if it was nested in a generic type (for example)
            string enumName = enumSymbol.ToString();

            // Get all the members in the enum
            ImmutableArray<ISymbol> enumMembers = enumSymbol.GetMembers();
            var members = new List<string>(enumMembers.Length);

            // Get all the fields from the enum, and add their name to the list
            foreach (ISymbol member in enumMembers)
            {
                if (member is IFieldSymbol field && field.ConstantValue is not null)
                {
                    members.Add(member.Name);
                }
            }

            // Create an EnumToGenerate for use in the generation phase
            enumsToGenerate.Add(new JsonConverterToGenerate(enumName, members));

            foreach (ISymbol member in enumMembers)
            {
                if (member is IFieldSymbol field && field.ConstantValue is not null)
                {
                    members.Add(member.Name);
                }
            }

            return new JsonConverterToGenerate(enumName, members);
        }

    }
}



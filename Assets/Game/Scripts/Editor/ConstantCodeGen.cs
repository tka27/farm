using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Game.Scripts.Editor
{
    public static class ConstantCodeGen
    {
        private const string DefaultNameSpace = "Game.Scripts.Generated";
        private const string PathPrefix = "Game/Scripts/Generated";

        private static List<string> GetAllTags() => InternalEditorUtility.tags.ToList();

        private static List<string> GetAllLayers()
        {
            var layers = new List<string>();

            for (var i = 0; i < 32; i++)
            {
                layers.Add(LayerMask.LayerToName(i));
            }

            return layers;
        }

        private static void ImitateStaticClass(CodeTypeDeclaration typeDeclaration)
        {
            typeDeclaration.TypeAttributes |= TypeAttributes.Sealed;

            typeDeclaration.Members.Add(
                new CodeConstructor
                {
                    Attributes = MemberAttributes.Private | MemberAttributes.Final
                }
            );
        }

        private static CodeCompileUnit GenerateClassWithConstants(
            string name,
            IReadOnlyList<string> constants,
            bool isIds = false
        )
        {
            var compileUnit = new CodeCompileUnit();
            var codeNamespace = new CodeNamespace(DefaultNameSpace);
            var className = new CodeTypeDeclaration(name);

            ImitateStaticClass(className);

            for (var i = 0; i < constants.Count; i++)
            {
                var constantName = constants[i];
                if (string.IsNullOrEmpty(constantName))
                {
                    continue;
                }

                var constField = isIds
                    ? GenerateConstant(constantName, i)
                    : GenerateConstant(constantName, constantName);

                className.Members.Add(constField);
            }

            codeNamespace.Types.Add(className);
            compileUnit.Namespaces.Add(codeNamespace);

            return compileUnit;
        }

        private static CodeMemberField GenerateConstant<T>(string name, T value)
        {
            name = name.Replace(" ", "");

            var constField = new CodeMemberField(
                typeof(T),
                name
            );

            constField.Attributes &= ~MemberAttributes.AccessMask;
            constField.Attributes &= ~MemberAttributes.ScopeMask;
            constField.Attributes |= MemberAttributes.Public;
            constField.Attributes |= MemberAttributes.Const;

            constField.InitExpression = new CodePrimitiveExpression(value);

            return constField;
        }

        private static void WriteIntoFile(string fullPath, CodeCompileUnit code)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? string.Empty);

            using var stream = new StreamWriter(fullPath, append: false);
            using var codeProvider = new CSharpCodeProvider();

            var writer = new IndentedTextWriter(stream);

            codeProvider.GenerateCodeFromCompileUnit(code, writer, new CodeGeneratorOptions());
        }

        private static void GenerateFile(in string fileName, in IReadOnlyList<string> constants, in bool isIds = false)
        {
            var filePath = PathPrefix + "/" + fileName;
            var fullPath = Path.Combine(Application.dataPath, filePath);
            var className = Path.GetFileNameWithoutExtension(fullPath);

            var code = GenerateClassWithConstants(className, constants, isIds);
            WriteIntoFile(fullPath, code);

            AssetDatabase.ImportAsset("Assets/" + filePath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
        }

        [MenuItem("Window/Generator/Generate All Constants")]
        private static void GenerateAllConstants()
        {
            GenerateLayersConstantFile();
            GenerateLayersIdsConstantFile();
            GenerateTagConstantFile();
        }

        [MenuItem("Window/Generator/Generate Layers Constants")]
        private static void GenerateLayersConstantFile() => GenerateFile("Layers.cs", GetAllLayers());

        [MenuItem("Window/Generator/Generate LayersIds Constants")]
        private static void GenerateLayersIdsConstantFile() => GenerateFile("LayersIds.cs", GetAllLayers(), true);

        [MenuItem("Window/Generator/Generate Tag Constants")]
        private static void GenerateTagConstantFile() => GenerateFile("Tags.cs", GetAllTags());
    }
}
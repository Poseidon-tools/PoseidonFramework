namespace Poseidon.StateMachine.Editor
{
    using System.IO;
    using System.Text;

    public class StateFileGenerator
    {
        public static void GenerateStateFile(string filePath, string namespaceName, string className, string stateType, 
            string enumValue, params string[] usings)
        {
            // Write out our file
            using StreamWriter writer = new StreamWriter(filePath);
            writer.WriteLine($"namespace {namespaceName}");
            writer.WriteLine("{");

            foreach (string usage in usings)
            {
                writer.WriteLineIdent(1,$"using {usage};");
            }
            
            writer.WriteLine();

            // Write out the class
            writer.WriteLineIdent(1,$"public class {className}");
            writer.WriteLineIdent(1,"{");
                
            // Override StateType
            writer.WriteLineIdent(2,$"public override {stateType} StateType => {stateType}.{enumValue};");

            writer.WriteLine();

            // OnEnter
            writer.WriteLineIdent(2,"public override void OnEnter()");
            writer.WriteLineIdent(2,"{");
            writer.WriteLine();
            writer.WriteLineIdent(2,"}");
                
            writer.WriteLine();

            // OnExit
            writer.WriteLineIdent(2,"public override void OnExit()");
            writer.WriteLineIdent(2,"{");
            writer.WriteLine();
            writer.WriteLineIdent(2,"}");
            
            writer.WriteLine();

            // End of class
            writer.WriteLineIdent(1,"}");
            writer.WriteLine();

            // End of namespace
            writer.WriteLine("}");
            writer.WriteLine();
        }
    }

    public static class StreamWriterExtension
    {
        private static readonly StringBuilder stringBuilder = new StringBuilder();

        public static void WriteLineIdent(this StreamWriter writer, int ident, string value)
        {
            writer.WriteLine($"{GetIndent(ident)}{value}");
        }
        
        private static string GetIndent(int count)
        {
            stringBuilder.Clear();
            
            for (int i = 0; i < count; i++)
            {
                stringBuilder.Append("\t");
            }

            return stringBuilder.ToString();
        }
    }
}

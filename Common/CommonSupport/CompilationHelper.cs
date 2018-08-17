using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CSharp;
using System.Reflection;
using System.CodeDom.Compiler;

namespace CommonSupport
{
    public static class CompilationHelper
    {

        /// <summary>
        ///  This will compile sources using the tools integrated in the .NET framework.
        /// </summary>
        /// <param name="sourceCode"></param>
        /// <param name="resultMessagesAndRows"></param>
        /// <returns>Will return null if compilation was not successful.</returns>
        public static Assembly CompileSourceToAssembly(string sourceCode, out Dictionary<string, int> resultMessagesAndRows)
        {
            resultMessagesAndRows = new Dictionary<string, int>();

            CSharpCodeProvider codeProvider = new CSharpCodeProvider();

            System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;

            // this will add references to all the assemblies we are using.
            foreach (Assembly assembly in ReflectionHelper.GetApplicationEntryAssemblyAndReferencedAssemblies())
            {
                parameters.ReferencedAssemblies.Add(assembly.Location);
            }

            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, sourceCode);
            
            if (results.Errors.Count > 0)
            {
                foreach (CompilerError error in results.Errors)
                {
                    resultMessagesAndRows.Add("Line " + error.Line.ToString() + ": (" + error.ErrorNumber.ToString() + ")" + error.ErrorText, error.Line);
                }
                return null;
            }
            else
            {
                resultMessagesAndRows.Add("Compiled succesfully", -1);
                return results.CompiledAssembly;
            }
        }

        //This is a compilation done the CSScriptLibrary way
        //try
        //{
        //    Assembly assembly = CSScriptLibrary.CSScript.LoadCode(source, null, true);
        //    ListViewItem item2 = new ListViewItem("Compilation Succesfull", "ok");
        //    this.listView1.Items.Add(item2);
        //    return assembly;
        //}
        //catch (Exception ex)
        //{
        //    foreach(String line in ex.Message.Split(new char[] {}))
        //    {
        //        ListViewItem item = new ListViewItem(line, "error");
        //        this.listView1.Items.Add(item);
        //    }
        //    return null;
        //}


    }
}

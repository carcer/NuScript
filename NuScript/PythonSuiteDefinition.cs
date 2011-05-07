using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace NuScript
{
    public abstract class PythonSuiteDefinition : ScriptSuiteDefinition
    {
        private readonly LanguageSetup _languageSetup = Python.CreateLanguageSetup(null);

        public override LanguageSetup LanguageSetup
        {
            get { return _languageSetup; }
        }

        protected override string ScriptFilePattern
        {
            get { return "*.py"; }
        }
    }
}

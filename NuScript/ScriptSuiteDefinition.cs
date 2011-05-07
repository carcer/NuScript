using System.IO;
using Microsoft.Scripting.Hosting;
using NUnit.Framework;

namespace NuScript
{
	public abstract class ScriptSuiteDefinition
	{
		protected ScriptSuiteDefinition()
		{
			var context = new ScriptContext();
            context.IncludeAssembly(typeof(Assert).Assembly);

            this.ScriptContext = context;
		}

		public ScriptContext ScriptContext { get; private set; }

        public bool EnableDebugging { get; set; }

		public string[] FindScripts()
		{
			// NOTE: Doesn't include scripts in sub-directories. This allows helper scripts that don't correspond to individual tests.
			return Directory.GetFiles(this.ScriptPath, this.ScriptFilePattern, SearchOption.TopDirectoryOnly);
		}

        public abstract LanguageSetup LanguageSetup { get; }
        protected abstract string ScriptPath { get; }
        protected abstract string ScriptFilePattern { get; }
	}
}
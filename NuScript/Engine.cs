using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Scripting.Hosting;

namespace NuScript
{
	/// <summary>
    /// Scripting engine base class
    /// </summary>
    public class Engine
    {
        /// <summary>
        /// Initialises an instance
        /// </summary>
        /// <param name="languageSetup">Scripting language configuration object</param>
        /// <param name="enableDebugging">Indicates whether script debugging should be enabled</param>
        public Engine(LanguageSetup languageSetup, bool enableDebugging)
        {
            ScriptRuntimeSetup setup = new ScriptRuntimeSetup();
            setup.LanguageSetups.Add(languageSetup);
            setup.DebugMode = enableDebugging;

            var runtime = new ScriptRuntime(setup);
            var engine = runtime.GetEngine(setup.LanguageSetups[0].Names[0]);

            _engine = engine;
            _scope = _engine.CreateScope();
        }

        /// <summary>
        /// Gets or sets the value of a script variable
        /// </summary>
        /// <param name="name">The variable name</param>
        public object this[string name]
        {
            get
            {
                return GetVariable(name);
            }

            set
            {
                SetVariable(name, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether script debugging is enabled
        /// </summary>
        public bool IsDebuggingEnabled
        {
            get { return _engine.Runtime.Setup.DebugMode; }
        }

        /// <summary>
        /// Executes the specified script fragment
        /// </summary>
        /// <param name="script">A script fragment to execute</param>
        public void ExecuteScriptText(string script)
        {
            ScriptSource source = _engine.CreateScriptSourceFromString(script);
            source.Execute(_scope);
        }

        /// <summary>
        /// Executes the specified script file
        /// </summary>
        /// <param name="path">The script file to execute</param>
        public void ExecuteScriptFile(string path)
        {
            string previousCurrentDirectory = Environment.CurrentDirectory;

            try
            {
                string fullScriptPath = Path.GetFullPath(path);
				string scriptDirectory = Path.GetDirectoryName(fullScriptPath);
				if (scriptDirectory == null)
				{
					throw new ArgumentException("Invalid script path.", "path");
				}

            	Environment.CurrentDirectory = scriptDirectory;

                ScriptSource source = _engine.CreateScriptSourceFromFile(fullScriptPath, Encoding.UTF8);
                source.Execute(_scope);
            }
            finally
            {
                Environment.CurrentDirectory = previousCurrentDirectory;
            }
        }

        /// <summary>
        /// Gets the value of the specified script variable
        /// </summary>
        /// <param name="name">The name of the script variable</param>
        /// <returns>The value of the script variable, or <c>null</c> if the variable doesn't exist</returns>
        public object GetVariable(string name)
        {
            object value;
            if (!_scope.TryGetVariable(name, out value))
            {
                value = null;
            }

            return value;
        }

        /// <summary>
        /// Sets the value of the specified script variable
        /// </summary>
        /// <param name="name">The name of the script variable</param>
        /// <param name="value">The value to assign to the script variable</param>
        public void SetVariable(string name, object value)
        {
            _scope.SetVariable(name, value);
        }

        /// <summary>
        /// Loads the specified assembly into the scripting engine.
        /// </summary>
        /// <param name="assembly">The assembly to load.</param>
        public void LoadAssembly(Assembly assembly)
        {
            _engine.Runtime.LoadAssembly(assembly);
        }

		/// <summary>
		/// Sets the context for scripts launched from the engine.
		/// </summary>
		/// <param name="scriptContext">Script context.</param>
		public virtual void Initialise(ScriptContext scriptContext)
		{
			foreach (var variable in scriptContext.Variables)
			{
				SetVariable(variable.Key, variable.Value);
			}

			foreach (var assembly in scriptContext.Assemblies)
			{
				LoadAssembly(assembly);
			}
		}

		protected ScriptEngine ScriptEngine
		{
			get { return _engine; }
		}

		private readonly ScriptEngine _engine;
		private readonly ScriptScope _scope;
	}
}
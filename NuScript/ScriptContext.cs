using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace NuScript
{
    /// <summary>
    /// The variables and assembly references to be made available to a script.
    /// </summary>
    public class ScriptContext
    {
        /// <summary>
        /// Sets the value of a script variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The value of the variable.</param>
        public void SetVariable(string name, object value)
        {
            _variables[name] = value;
        }

        /// <summary>
        /// Includes an assembly reference for use by the script (ignoring any duplicates).
        /// </summary>
        /// <param name="assembly">The assembly to include.</param>
        public void IncludeAssembly(Assembly assembly)
        {
            if (!_assemblies.Contains(assembly))
            {
                _assemblies.Add(assembly);
            }
        }

		/// <summary>
		/// Variable names and values that are shared with the scripts.
		/// </summary>
		// TODO: Make this a read-only copy somehow.
		public Dictionary<string, object> Variables
		{
			get { return _variables; }
		}

		/// <summary>
		/// Assemblies that are loaded into the script context.
		/// </summary>
    	public ReadOnlyCollection<Assembly> Assemblies
    	{
    		get {return new ReadOnlyCollection<Assembly>(_assemblies);}
    	}

    	private readonly Dictionary<string, object> _variables = new Dictionary<string, object>();
        private readonly List<Assembly> _assemblies = new List<Assembly>();
    }
}

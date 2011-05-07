using System;
using System.Collections.Generic;
using NUnit.Core;
using NUnit.Core.Extensibility;

namespace NuScript
{
    [NUnitAddin(Name = "NuScript suite builder.", Description = "Builds test suites from DLR scripts.")]
    [SuiteBuilder]
    public class ScriptSuiteBuilder : ISuiteBuilder, IAddin
    {
        public bool CanBuildFrom(Type type)
        {
            return type.IsSubclassOf(typeof(ScriptSuiteDefinition)) && !type.IsAbstract;
        }

        public Test BuildFrom(Type type)
        {
            TestFixture testSuite;
            if (CanBuildFrom(type))
            {
                var definition = (PythonSuiteDefinition)Reflect.Construct(type);
                testSuite = LoadTestScripts(definition);
            }
            else
            {
                testSuite = null;
            }

            return testSuite;
        }

        protected TestFixture LoadTestScripts(ScriptSuiteDefinition definition)
        {
            var engine = new Engine(definition.LanguageSetup, definition.EnableDebugging);
            engine.Initialise(definition.ScriptContext);

            var tests = new List<Test>();
            IList<string> scripts = definition.FindScripts();

            var testFixture = new TestFixture(definition.GetType());
            foreach (var script in scripts)
            {
                var test = new ScriptTest(script, engine);
                test.Fixture = testFixture;
                testFixture.Add(test);
            }

            return testFixture;
        }

        /// <summary>
        /// Registers this type as a SuiteBuilder for NUnit tests.
        /// </summary>
        public bool Install(IExtensionHost host)
        {
            IExtensionPoint suiteBuilders = host.GetExtensionPoint("SuiteBuilders");
            suiteBuilders.Install(this);
            return true;
        }
    }
}

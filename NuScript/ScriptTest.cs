using System;
using System.IO;
using NUnit.Core;
using NUnit.Framework;

namespace NuScript
{
    public class ScriptTest : Test
    {
		private readonly string _scriptPath;
		private readonly Engine _engine;

		public ScriptTest(string scriptPath, Engine engine)
			: base(Path.GetFileName(scriptPath))
		{
			_scriptPath = scriptPath;
			_engine = engine;
		}

		public override TestResult Run(EventListener listener, ITestFilter filter)
		{
			// TODO: Implement logic required for filtering.

			listener.TestStarted(this.TestName);
            long startTime = DateTime.Now.Ticks;

			var result = new TestResult(this);

			try
			{
				_engine.ExecuteScriptFile(_scriptPath);
				result.Success();
			}
			catch (AssertionException assertEx)
			{
				result.SetResult(ResultState.Failure, assertEx.Message, assertEx.StackTrace, FailureSite.Test);
			}
			catch (InconclusiveException inconclusiveEx)
			{
				result.SetResult(ResultState.Inconclusive, inconclusiveEx.Message, inconclusiveEx.StackTrace);
			}
			catch (Exception ex)
			{
				result.Error(ex);
			}
			finally
			{
                long stopTime = DateTime.Now.Ticks;
                double time = ((double)(stopTime - startTime)) / (double)TimeSpan.TicksPerSecond;
                result.Time = time;
                
                listener.TestFinished(result);
			}

			return result;
		}

		public override string TestType
		{
			get { return this.GetType().ToString(); }
		}

		public override object Fixture { get; set; }
	}
}

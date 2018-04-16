using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMD.Util.Tasks
{
	/// <summary>
	/// Progress changed delegate
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public delegate void ProgressHandler(Object sender, TaskProgressArgs args);

	/// <summary>
	/// Interface for tasks handled by the TaskHandler
	/// </summary>
	public interface ITask
	{
		/// <summary>
		/// Progress changed eventhandler
		/// </summary>
		event ProgressHandler OnProgressChanged;

    /// <summary>
    /// Set to cancel current task
    /// </summary>
    bool UserCancel { get; set; }

		/// <summary>
		/// Execute function which contains the work to be executed
		/// </summary>
		/// <param name="owner"></param>
		/// <returns></returns>
		TaskResult Execute(TaskHandler owner);
	}
}

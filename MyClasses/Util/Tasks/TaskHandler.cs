using AMD.Util.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMD.Util.Tasks
{
	/// <summary>
	/// The result of an executed task
	/// </summary>
	public enum TaskResult
	{
		/// <summary>
		/// Success of a task
		/// </summary>
		Success,

		/// <summary>
		/// Error in a task
		/// </summary>
		Error,

		/// <summary>
		/// User cancel
		/// </summary>
		UserCancel
	}

	/// <summary>
	/// Class for handling sequential execution of tasks
	/// </summary>
	public class TaskHandler : Queue<ITask>
	{
		#region Eventhandlers
		/// <summary>
		/// Overall progress changed delegate
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public delegate void OverallProgressHandler(object sender, OverallTaskProgressArgs args);
		/// <summary>
		/// Overall progress eventhandler
		/// </summary>
		public event OverallProgressHandler OnOverallProgressChanged;
		private void OverallProgressChanged(TaskProgressArgs args)
		{
			int overAllDynProgress = (int)(progressOverall + (100d / startNumOfTasks) * args.Progress / 100d);
			OnOverallProgressChanged?.Invoke(this, new OverallTaskProgressArgs(args.Message, overAllDynProgress, args.Progress));
		}

		/// <summary>
		/// All tasks finished delegate
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public delegate void FinishedHandler(object sender, FinishedArgs args);
		/// <summary>
		/// All tasks finished eventhandler
		/// </summary>
		public event FinishedHandler OnFinished;
		private void Finished(TaskResult result, String message, params object[] args)
		{
			log.WriteToLog(LogMsgType.Debug, "{0} tasks finished", startNumOfTasks);
			OnFinished?.Invoke(this, new FinishedArgs(String.Format(message, args), result));
		}
    #endregion // Eventhandlers

    #region Public members
    private bool _userCancel;
		/// <summary>
		/// Set this to safely cancel the task execution
		/// </summary>
		public bool UserCancel
    {
      get
      {
        return _userCancel;
      }
      set
      {
        if (task != null)
        {
          task.UserCancel = value;
        }
        _userCancel = value;
      }
    }

    /// <summary>
    /// Is true if task handler is executing tasks
    /// </summary>
    public bool IsExecuting { get; private set; }
    #endregion // Public members

    #region Private members
    private int progressOverall;
		private int startNumOfTasks;
    private LogWriter log;
    private ITask task;
    #endregion // Private members

    #region Constructors
    /// <summary>
    /// Zero argument constructor
    /// </summary>
    public TaskHandler()
		{
			log = LogWriter.Instance;
		}

		/// <summary>
		/// Constructor taking a collection of tasks and adds them to the task queue
		/// </summary>
		/// <param name="Tasks"></param>
		public TaskHandler(IEnumerable<ITask> Tasks) : this()
		{
			this.Add(Tasks);
		}
		#endregion // Constructors

		#region Functions
		/// <summary>
		/// Add a task to the taks queue
		/// </summary>
		/// <param name="task"></param>
		public void Add(ITask task)
		{
      task.OnProgressChanged += Task_OnProgressChanged;
			Enqueue(task);
			log.WriteToLog(LogMsgType.Debug, "Task added: {0}", task.GetType());
		}

    private void Task_OnProgressChanged(object sender, TaskProgressArgs args)
    {
      OverallProgressChanged(args);
    }

    /// <summary>
    /// Adds a collection of tasks to the task queue
    /// </summary>
    /// <param name="Tasks"></param>
    public void Add(IEnumerable<ITask> Tasks)
		{
			foreach (ITask task in Tasks)
			{
				Add(task);
			}
		}

		/// <summary>
		/// Executes all the tasks in the task queue
		/// </summary>
		/// <returns></returns>
		public TaskResult ExecuteTasks()
		{
      IsExecuting = true;

      TaskResult result = TaskResult.Success;

			startNumOfTasks = Count;

      log.WriteToLog(LogMsgType.Debug, "Executing {0} task{1}", startNumOfTasks, startNumOfTasks > 1 ? "s" : "");

			UserCancel = false;

      progressOverall = 0;

      while (Count > 0)
			{
				task = Dequeue();

				result = task.Execute(this);

				progressOverall = (startNumOfTasks - Count) * 100 / startNumOfTasks;
        task.OnProgressChanged -= Task_OnProgressChanged;

				if (UserCancel)
        {
          result = TaskResult.UserCancel;
          Finished(result, "User canceled the operation on task \"{0}\"", task.GetType().Name);
          break;
				}

				if (result == TaskResult.Error)
				{
					Clear();
					Finished(result, "Error executing task \"{0}\"", task.GetType().Name);
          break;
				}
			}
      if (result == TaskResult.Success)
      {
        Finished(result, "{0} tasks finished successfully!", startNumOfTasks);
      }
      task = null;
      IsExecuting = false;
			return result;
		}
		#endregion // Functions
	}

	public class FinishedArgs : EventArgs
	{
		/// <summary>
		/// Result of the task execution
		/// </summary>
		public TaskResult Result { get; set; }
		/// <summary>
		/// Finish message
		/// </summary>
		public String Message { get; set; }

		/// <summary>
		/// 2 argument constructor
		/// </summary>
		/// <param name="message"></param>
		/// <param name="result"></param>
		public FinishedArgs(String message, TaskResult result)
		{
			this.Result = result;
			this.Message = message;
		}
	}

	public class TaskProgressArgs : EventArgs
	{
		/// <summary>
		/// Progress in percentage
		/// </summary>
		public int Progress { get; set; }
		/// <summary>
		/// Progress message
		/// </summary>
		public String Message { get; set; }

		/// <summary>
		/// 2 argument constructor
		/// </summary>
		/// <param name="message"></param>
		/// <param name="progress"></param>
		public TaskProgressArgs(String message, int progress)
		{
			this.Message = message;
			this.Progress = progress;
		}
	}

	public class OverallTaskProgressArgs : EventArgs
	{
		/// <summary>
		/// Overall progress in percentage
		/// </summary>
		public int ProgressOverall { get; set; }
		/// <summary>
		/// Individual progress in percentage
		/// </summary>
		public int ProgressIndividual { get; set; }
		/// <summary>
		/// Progress message
		/// </summary>
		public String Message { get; set; }

		/// <summary>
		/// 3 argument constructor
		/// </summary>
		/// <param name="message"></param>
		/// <param name="progressOverall"></param>
		/// <param name="progressIndividual"></param>
		public OverallTaskProgressArgs(String message, int progressOverall, int progressIndividual)
		{
			this.Message = message;
			this.ProgressIndividual = progressIndividual;
			this.ProgressOverall = progressOverall;
		}
	}
}

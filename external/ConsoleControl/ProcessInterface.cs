using System.Text;
using System.Diagnostics;
using System.ComponentModel;

namespace ConsoleControlAPI
{
    /// <summary>
    /// A ProcessEventHandler is a delegate for process input/output events.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="ProcessEventArgs"/> instance containing the event data.</param>
    public delegate void ProcessEventHandler(object sender, ProcessEventArgs args);

    /// <summary>
    /// A class the wraps a process, allowing programmatic input and output.
    /// </summary>
    public class ProcessInterface: IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessInterface"/> class.
        /// </summary>
        public ProcessInterface()
        {
            //  Configure the output worker.
            _outputWorker.WorkerReportsProgress = true;
            _outputWorker.WorkerSupportsCancellation = true;
            _outputWorker.DoWork += outputWorker_DoWork;
            _outputWorker.ProgressChanged += outputWorker_ProgressChanged;

            //  Configure the error worker.
            _errorWorker.WorkerReportsProgress = true;
            _errorWorker.WorkerSupportsCancellation = true;
            _errorWorker.DoWork += errorWorker_DoWork;
            _errorWorker.ProgressChanged += errorWorker_ProgressChanged;
        }

        /// <summary>
        /// Handles the ProgressChanged event of the outputWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.ProgressChangedEventArgs"/> instance containing the event data.</param>
        private void outputWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            //  We must be passed a string in the user state.
            if (e.UserState is string state)
            {
                //  Fire the output event.
                FireProcessOutputEvent(state);
            }
        }

        /// <summary>
        /// Handles the DoWork event of the outputWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.DoWorkEventArgs"/> instance containing the event data.</param>
        private void outputWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (_outputReader is null)
                return;
            
            while (_outputWorker.CancellationPending == false)
            {
                //  Any lines to read?
                int count;
                char[] buffer = new char[1024];
                do
                {
                    StringBuilder builder = new();
                    count = _outputReader.Read(buffer, 0, 1024);
                    builder.Append(buffer, 0, count);
                    _outputWorker.ReportProgress(0, builder.ToString());
                } while (count > 0);

                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// Handles the ProgressChanged event of the errorWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.ProgressChangedEventArgs"/> instance containing the event data.</param>
        private void errorWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            //  The userstate must be a string.
            if (e.UserState is string state)
            {
                //  Fire the error event.
                FireProcessErrorEvent(state);
            }
        }

        /// <summary>
        /// Handles the DoWork event of the errorWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.DoWorkEventArgs"/> instance containing the event data.</param>
        private void errorWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (_errorReader is null)
                return;
            
            while (_errorWorker.CancellationPending == false)
            {
                //  Any lines to read?
                int count;
                char[] buffer = new char[1024];
                do
                {
                    StringBuilder builder = new();
                    count = _errorReader.Read(buffer, 0, 1024);
                    builder.Append(buffer, 0, count);
                    _errorWorker.ReportProgress(0, builder.ToString());
                } while (count > 0);

                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// Runs a process.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="arguments">The arguments.</param>
        public void StartProcess(string fileName, string arguments)
        {
            //  Create the process start info.
            ProcessStartInfo processStartInfo = new(fileName, arguments);
            StartProcess(processStartInfo);
        }

        /// <summary>
        /// Runs a process.
        /// </summary>
        /// <param name="processStartInfo"><see cref="ProcessStartInfo"/> to pass to the process.</param>
        public void StartProcess(ProcessStartInfo processStartInfo)
        {
            //  Set the options.
            processStartInfo.UseShellExecute = false;
            processStartInfo.ErrorDialog = false;
            processStartInfo.CreateNoWindow = true;

            //  Specify redirection.
            processStartInfo.RedirectStandardError = true;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;

            //  Create the process.
            Process = new Process();
            Process.EnableRaisingEvents = true;
            Process.StartInfo = processStartInfo;
            Process.Exited += currentProcess_Exited;

            //  Start the process.
            try
            {
                Process.Start();
            }
            catch (Exception e)
            {
                //  Trace the exception.
                Trace.WriteLine("Failed to start process " + processStartInfo.FileName + " with arguments '" + processStartInfo.Arguments + "'");
                Trace.WriteLine(e.ToString());
                return;
            }

            //  Store name and arguments.
            ProcessFileName = processStartInfo.FileName;
            ProcessArguments = processStartInfo.Arguments;

            //  Create the readers and writers.
            _inputWriter = Process.StandardInput;
            _outputReader = TextReader.Synchronized(Process.StandardOutput);
            _errorReader = TextReader.Synchronized(Process.StandardError);

            //  Run the workers that read output and error.
            _outputWorker.RunWorkerAsync();
            _errorWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Stops the process.
        /// </summary>
        public void StopProcess()
        {
            //  Handle the trivial case.
            if (IsProcessRunning == false)
                return;

            //  Kill the process.
            Process?.Kill();
        }

        /// <summary>
        /// Handles the Exited event of the currentProcess control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void currentProcess_Exited(object? sender, EventArgs e)
        {
            //  Fire process exited.
            if (Process != null)
                FireProcessExitEvent(Process.ExitCode);

            //  Disable the threads.
            _outputWorker.CancelAsync();
            _errorWorker.CancelAsync();
            _inputWriter = null;
            _outputReader = null;
            _errorReader = null;
            Process = null;
            ProcessFileName = null;
            ProcessArguments = null;
        }

        /// <summary>
        /// Fires the process output event.
        /// </summary>
        /// <param name="content">The content.</param>
        private void FireProcessOutputEvent(string content)
        {
            //  Get the event and fire it.
            OnProcessOutput?.Invoke(this, new ProcessEventArgs(content));
        }

        /// <summary>
        /// Fires the process error output event.
        /// </summary>
        /// <param name="content">The content.</param>
        private void FireProcessErrorEvent(string content)
        {
            //  Get the event and fire it.
            OnProcessError?.Invoke(this, new ProcessEventArgs(content));
        }

        /// <summary>
        /// Fires the process input event.
        /// </summary>
        /// <param name="content">The content.</param>
        private void FireProcessInputEvent(string content)
        {
            //  Get the event and fire it.
            OnProcessInput?.Invoke(this, new ProcessEventArgs(content));
        }

        /// <summary>
        /// Fires the process exit event.
        /// </summary>
        /// <param name="code">The code.</param>
        private void FireProcessExitEvent(int code)
        {
            //  Get the event and fire it.
            OnProcessExit?.Invoke(this, new ProcessEventArgs(code));
        }

        /// <summary>
        /// Writes the input.
        /// </summary>
        /// <param name="input">The input.</param>
        public void WriteInput(string input)
        {
            if (IsProcessRunning && _inputWriter is not null)
            {
                _inputWriter.WriteLine(input);
                _inputWriter.Flush();
            }
        }

        /// <summary>Finalizes an instance of the <see cref="ProcessInterface"/> class.</summary>
        ~ProcessInterface()
        {
            Dispose(true);
        }

        /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
        /// <param name="native">
        ///   <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected void Dispose(bool native)
        {
            _outputWorker.Dispose();
            _errorWorker.Dispose();
            if (Process != null)
            {
                Process.Dispose();
                Process = null;
            }
            if (_inputWriter != null)
            {
                _inputWriter.Dispose();
                _inputWriter = null;
            }
            if (_outputReader != null)
            {
                _outputReader.Dispose();
                _outputReader = null;
            }
            if (_errorReader != null)
            {
                _errorReader.Dispose();
                _errorReader = null;
            }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The input writer.
        /// </summary>
        private StreamWriter? _inputWriter;
        
        /// <summary>
        /// The output reader.
        /// </summary>
        private TextReader? _outputReader;
        
        /// <summary>
        /// The error reader.
        /// </summary>
        private TextReader? _errorReader;
        
        /// <summary>
        /// The output worker.
        /// </summary>
        private readonly BackgroundWorker _outputWorker = new();
        
        /// <summary>
        /// The error worker.
        /// </summary>
        private readonly BackgroundWorker _errorWorker = new();

        /// <summary>
        /// Occurs when process output is produced.
        /// </summary>
        public event ProcessEventHandler? OnProcessOutput;

        /// <summary>
        /// Occurs when process error output is produced.
        /// </summary>
        public event ProcessEventHandler? OnProcessError;

        /// <summary>
        /// Occurs when process input is produced.
        /// </summary>
        public event ProcessEventHandler? OnProcessInput;

        /// <summary>
        /// Occurs when the process ends.
        /// </summary>
        public event ProcessEventHandler? OnProcessExit;
        
        /// <summary>
        /// Gets a value indicating whether this instance is process running.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is process running; otherwise, <c>false</c>.
        /// </value>
        public bool IsProcessRunning
        {
            get
            {
                try
                {
                    return Process is {HasExited: false};
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the internal process.
        /// </summary>
        public Process? Process { get; private set; }

        /// <summary>
        /// Gets the name of the process.
        /// </summary>
        /// <value>
        /// The name of the process.
        /// </value>
        public string? ProcessFileName { get; private set; }

        /// <summary>
        /// Gets the process arguments.
        /// </summary>
        public string? ProcessArguments { get; private set; }
    }
}

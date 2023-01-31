namespace JackTheVideoRipper.models;

public abstract class TaskForm : Form
{
    public TaskForm()
    {
        SubscribeEvents();
    }
    
    protected void SubscribeEvents()
    {
        Shown += async (_, _) =>
        {
            Application.DoEvents();
            await GetPrimaryTask().ContinueWith(GetContinuationTask);
        };
    }

    public abstract Task GetPrimaryTask();

    public void GetContinuationTask(Task task)
    {
        Close();
    }
}
namespace JackTheVideoRipper;

public enum ProcessStatus
{
    // Created, but not queued
    Created,
    
    // Place into work queue
    Queued,
    
    // Currently running
    Running,
    
    // Process finished (Error, Stopped, Cancelled, Succeeded)
    Completed,
    
    // Process errored out
    Error,
    
    // System stopped process
    Stopped,
    
    // User cancelled process
    Cancelled,
    
    // Process succeeded
    Succeeded
}
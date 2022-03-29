namespace Akytos.ProjectManagement;

internal interface IProjectManager
{
    AkytosProject CurrentProject { get; }
    IEnumerable<AkytosProject> GetPreviousProjects();
    IEnumerable<string> ValidateProjectParameters(string projectName, string projectDirectory);
    bool LoadLastOpenedProject();
    void LoadProject(AkytosProject project);
    void CreateNewProject(string projectName, string projectDirectory);
}
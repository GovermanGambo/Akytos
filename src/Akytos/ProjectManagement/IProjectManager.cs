namespace Akytos.ProjectManagement;

internal interface IProjectManager
{
    AkytosProject CurrentProject { get; }
    IEnumerable<AkytosProject> GetPreviousProjects();
    bool LoadLastOpenedProject();
    void LoadProject(AkytosProject project);
    void CreateNewProject(string projectName, string projectDirectory);
}
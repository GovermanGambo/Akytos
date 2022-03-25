namespace Akytos.ProjectManagement;

internal interface IProjectManager
{
    IEnumerable<AkytosProject> GetPreviousProjects();
    bool LoadLastOpenedProject();
    void LoadProject(AkytosProject project);
    void CreateNewProject(string projectName, string projectDirectory);
}
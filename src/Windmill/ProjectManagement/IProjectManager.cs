using System;
using System.Collections.Generic;
using Akytos;

namespace Windmill.ProjectManagement;

internal interface IProjectManager
{
    event Action? ProjectChanged;
    AkytosProject CurrentProject { get; }
    IEnumerable<AkytosProject> GetPreviousProjects();
    IEnumerable<string> ValidateProjectParameters(string projectName, string projectDirectory, bool ignoreNonExisting = false);
    bool LoadLastOpenedProject();
    void LoadProject(AkytosProject project);
    void CreateNewProject(string projectName, string projectDirectory);
}
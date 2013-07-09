using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using warpspeed.Models;
using warpspeed.Helpers;

namespace warpspeed.Helpers
{
    public class ProgressHelper
    {
        public int PrepareToUpdateProgressOfSubProject(SubProject subProject)
        {
            int _result=0;
            try
            {
                double subProjectPercentComplete = 0;
                if (subProject.Tasks.Count() > 0)
                {
                    subProjectPercentComplete = 100 * subProject.Tasks.Where(t => t.Status == 1).Count() / subProject.Tasks.Count();
                }
                _result = Int32.Parse(Math.Round(subProjectPercentComplete).ToString());
            }
            catch (Exception ex)
            {

            }
            return _result;
        }
        public int PrepareToUpdateProgressOfProject(Project project)
        {
            int _result = 0;
            try
            {
                double sumSubProjectPercentComplete = 0;
                double projectPercentComplete = 0;
                if (project.SubProjects.Count() > 0)
                {
                    foreach (SubProject subProject in project.SubProjects)
                    {
                        sumSubProjectPercentComplete += double.Parse((subProject.PercentComplete ?? 0).ToString());
                    }
                    projectPercentComplete = 100 * sumSubProjectPercentComplete / (100 * project.SubProjects.Count());
                }
                _result = Int32.Parse(Math.Round(projectPercentComplete).ToString());
            }
            catch (Exception ex)
            {
                
            }
            return _result;
        }
    }
}
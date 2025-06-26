using System.Collections.Generic;

namespace Supor.Process.Entity.OutDto
{
    public class TaskOutDto
    {
        public bool Success { get; set; }

        /// <summary>
        /// Guid-InsId
        /// </summary>
        public Dictionary<string, string> GuidInsId { get; set; }

        public TaskOutDto()
        {
            Success = false;
        }
    }
}

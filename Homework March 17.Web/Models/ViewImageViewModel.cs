using Homework_March_17.Data;

namespace Homework_March_17.Web.Models
{
    public class ViewImageViewModel
    {
        public Image image { get; set; }
        public bool CorrectCredentials { get; set; }
        public string Message { get; set; }
    }
}
